using CSharpFunctionalExtensions;
using DODQuiz.Application.Abstract.Services;
using DODQuiz.Core.Entyties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "user")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private static List<WebSocket> _sockets = new List<WebSocket>();
        private static WebSocket _adminSocket { get; set; }
        private static Dictionary<WebSocket, Guid> _socketToUser = new Dictionary<WebSocket, Guid>();
        private static Dictionary<WebSocket, Guid> _socketToAdmin = new Dictionary<WebSocket, Guid>();
        private readonly IGameService _gameService;
        private readonly IConfiguration _configuration;

        private static int _timeRemaining; //seconds
        private static Timer _timer;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [Authorize(Policy = "admin")]
        [HttpPost("RemoveUser")]
        public async Task<ActionResult> RemoveUser(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _gameService.RemoveUserFromGame(userId, cancellationToken);
            return Ok(result.Value);
        }
        [Authorize(Policy = "admin")]
        [HttpPost("StartRound")]
        public async Task<ActionResult> StartRound(int? remainingTime, CancellationToken cancellation)
        {
            _timeRemaining = remainingTime ?? 300;
            if (_timer == null)
            {
                _timer = new Timer(OnTimerElapsed, null, 0, 1000);
            }
            else
            {
                _timer.Dispose();
                _timer = new Timer(OnTimerElapsed, null, 0, 1000);
            }
            var result = await _gameService.StartRound(cancellation);
            if (result.IsError)
            {
                return BadRequest();
            }
            await SendQuestionsUpdate(cancellation);
            await SendUserStatuses(cancellation);
            return Ok(result.Value);
        }
        private async void OnTimerElapsed(object state)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining--;
                await SendRemainingTime(CancellationToken.None);
            }
            else
            {
                _timer.Dispose();
            }
        }
        [HttpGet("Timer")]
        public IActionResult GetTimeRemaining()
        {
            // Верните оставшееся время
            if (_timeRemaining > 0)
            {
                return Ok(_timeRemaining);
            }
            return Ok(0);
        }

        [Authorize(Policy = "admin")]
        [HttpPost("EndRound")]
        public async Task<ActionResult> EndRound()
        {
            return Ok();
        }


        [HttpGet("GetCategories")]
        public async Task<ActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await _gameService.GetQuestionsCategories(cancellationToken);
            if (result.IsError)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }
        [HttpGet("GetIp")]
        public async Task<ActionResult> GetIp(CancellationToken cancellationToken)
        {
            return Ok();
        }
        [HttpPut("ChangeUserStatus")]
        public async Task<ActionResult> ChangeUserStatus(string code, CancellationToken cancellationToken)
        {
            var id = Guid.Parse(HttpContext.User.Claims.Where(c => c.Type == "UserId").FirstOrDefault().Value);
            var result = await _gameService.ChangeUserStatus(id, code, cancellationToken);
            if (result.IsError)
            {
                return BadRequest(result);
            }
            await SendUserStatuses(cancellationToken);
            return Ok(result.Value);
        }
        private async Task Send(string message)
        {
            var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
        [Authorize(Policy = "admin")]
        [HttpGet("/wsadmin")]
        public async Task GetAdmin()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("admin socket open");
                var userId = Guid.Parse(HttpContext.User.Claims.Where(c => c.Type == "UserId").FirstOrDefault().Value);
                _socketToAdmin[webSocket] = userId;
                await HandleAdminWebSocketConnection(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("socket open");
                var userId = Guid.Parse(HttpContext.User.Claims.Where(c => c.Type == "UserId").FirstOrDefault().Value);
                _socketToUser[webSocket] = userId;
                await HandleWebSocketConnection(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        private async Task HandleAdminWebSocketConnection(WebSocket webSocket)
        {
            _adminSocket = webSocket;
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    await Task.Delay(1000); // Ждем, пока веб-сокет открыт
                }
            }
            finally
            {
                _adminSocket = null;
                _socketToAdmin.Remove(webSocket);
                Console.WriteLine("socket closed");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Closed by the WebSocketHandler", CancellationToken.None);
            }
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            _sockets.Add(webSocket);
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    await Task.Delay(1000); // Ждем, пока веб-сокет открыт
                }
            }
            finally
            {
                _sockets.Remove(webSocket);
                _socketToUser.Remove(webSocket);
                Console.WriteLine("socket closed");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Closed by the WebSocketHandler", CancellationToken.None);
            }
        }
        private async Task SendUserStatuses(CancellationToken cancellationToken)
        {
            var statuseswrap = await _gameService.GetUsersStatuses(cancellationToken);
            var statuses = statuseswrap.Value;
            if (_adminSocket != null)
            {
                var socket = _adminSocket;
                if (socket.State == WebSocketState.Open)
                {
                    var messagePrepare = new ConcurrentDictionary<string, string>();
                    messagePrepare.TryAdd("timer", _timeRemaining.ToString());
                    var statusesmessage = JsonSerializer.Serialize(statuses);
                    messagePrepare.TryAdd("statuses", statusesmessage);

                    if (statuses.Count > 0)
                    {
                        messagePrepare.TryAdd("all-win", "false");
                        if (statuses.All(s => s.Value == true))
                        {
                            messagePrepare.TryUpdate("all-win", "true", "false");
                        }
                        //foreach (var stat in statuses)
                        //{
                        //    if (!stat.Value)
                        //    {
                        //        messagePrepare.TryUpdate("all-win", "false", "true");
                        //        break;
                        //    }
                        //}
                    }

                    var message = JsonSerializer.Serialize(messagePrepare);
                    var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
        private async Task SendRemainingTime(CancellationToken cancellationToken)
        {
            var messagePrepare = new Dictionary<string, string>();
            messagePrepare["timer"] = _timeRemaining.ToString();
            var message = JsonSerializer.Serialize(messagePrepare);
            var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
            foreach (var socket in _sockets)
            {
                if (socket != null && socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            if (_adminSocket != null && _adminSocket.State == WebSocketState.Open)
                await _adminSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        private async Task SendQuestionsUpdate(CancellationToken cancellationToken)
        {
            var questionsWrap = await _gameService.GetUserToQuestion(cancellationToken);
            var questions = questionsWrap.Value;


            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var userId = _socketToUser[socket];

                    var user = questions.Keys.Where(u => u.Id == userId).FirstOrDefault();
                    var messagePrepare = new Dictionary<string, string>();
                    string message;
                    try
                    {
                        var userQuestion = JsonSerializer.Serialize(questions[user]);
                        messagePrepare["question"] = userQuestion;
                        message = JsonSerializer.Serialize(messagePrepare);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Players List was null" + ex.ToString());
                        message = "";
                    }
                    var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private static async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        [HttpGet("MyQuestion")]
        public async Task<ActionResult> GetMyQuestion(CancellationToken cancellationToken)
        {
            var questions = _gameService.GetUserToQuestion(cancellationToken);
            var userId = HttpContext.User.Claims.ToList().Where(c=> c.Type== "UserId").FirstOrDefault();
            var users = await _gameService.GetAllUsers(cancellationToken);
            var user =  users.Value.ToList().Where(u => u.Id == Guid.Parse(userId.Value)).FirstOrDefault();
            await questions;
            var result = questions.Result.Value.TryGetValue(user, out var answer);
            if (answer != null) {
                return Ok(JsonSerializer.Serialize(answer));
            }
            return BadRequest($"User {userId} not found in users collection {result}");
        }
    }
}
