using DODQuiz.Application.Abstract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Text.Json;
using System.Net.WebSockets;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "user")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private static List<WebSocket> _sockets = new List<WebSocket>();
        private static Dictionary<WebSocket,Guid> _socketToUser = new Dictionary<WebSocket,Guid>();
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("GetMyQuestion")]
        public async Task<ActionResult> GetMyQuestions()
        {
            var id = HttpContext.User.Claims.Where(c => c.Type == "UserId").FirstOrDefault();
            return Ok();
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

        [HttpGet("ActiveUsers")]
        public async Task<ActionResult> GetActiveUsers()
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
        public async Task Send(string message)
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

        public async Task SendQuestionsUpdate(CancellationToken cancellationToken)
        {
            var questionsWrap = await _gameService.GetUserToQuestion(cancellationToken);
            var questions = questionsWrap.Value;
            

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var userId = _socketToUser[socket];

                    var user = questions.Keys.Where(u=> u.Id == userId).FirstOrDefault();
                    var message = JsonSerializer.Serialize(questions[user]);
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
