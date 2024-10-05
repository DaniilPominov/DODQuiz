using DODQuiz.API.Extensions;
using DODQuiz.Infrastructure;
using System.Net;
using System.Net.Sockets;
namespace DODQuiz.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddPersistance();
            builder.Services.AddSecurity();
            builder.Services.AddQuizGame();
            builder.Services.AddApiAuth(builder.Configuration);
            var app = builder.Build();

            string LocalIp = LocalIPAddress();
            app.Urls.Add("http://" + LocalIp + ":5072");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseFileServer();
            app.MapControllers();

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(20)
            };

            app.UseWebSockets(webSocketOptions);
            //app.UseRouting();
            app.Run();
        }
        static string LocalIPAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
                if (endPoint != null)
                {
                    return endPoint.Address.ToString();
                }
                else
                {
                    return "127.0.0.1";
                }
            }
        }
    }
}
