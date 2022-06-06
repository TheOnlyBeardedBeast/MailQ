using MailQ.Extensions;
using MailQ.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddLiteDatabase();
builder.Services.AddMemoryCache();
builder.Services.AddMailQ();

var app = builder.Build();

app.MapGrpcService<MailComService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
