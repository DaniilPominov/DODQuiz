using DODQuiz.Application.Abstract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text.Json;
using System.Net.WebSockets;
using DODQuiz.Core.Entyties;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "user")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private static List<WebSocket> _sockets = new List<WebSocket>();
        private static WebSocket _adminSocket {  get; set; }
        private static Dictionary<WebSocket,Guid> _socketToUser = new Dictionary<WebSocket,Guid>();
        private static Dictionary<WebSocket, Guid> _socketToAdmin = new Dictionary<WebSocket, Guid>();
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [Authorize(Policy = "admin")]
        [HttpPost("StartRound")]
        public async Task<ActionResult> StartRound(CancellationToken cancellation)
        {
            var result = await _gameService.StartRound(cancellation);
            if (result.IsError)
            {
                return BadRequest();
            }
            await SendQuestionsUpdate(cancellation);
            return Ok(result.Value);
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
                        var message = JsonSerializer.Serialize(statuses);
                        var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));
                        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
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

                    var user = questions.Keys.Where(u=> u.Id == userId).FirstOrDefault();
                    string message;
                    try
                    {
                        message = JsonSerializer.Serialize(questions[user]);
                    }
                    catch(Exception ex) 
                    { 
                        Console.WriteLine(ex.ToString());
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
    }
}
