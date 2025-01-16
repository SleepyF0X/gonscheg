using Gonscheg;
using Gonscheg.Extensions;
using Gonscheg.Handlers;
using Telegram.Bot;

var botClient = new BotClient();
await botClient.StartClientAsync(UpdateHandler.Handle, ErrorHandler.Handle);

//Add Extensions here
new PereclichkaExtension(botClient, UpdateHandler.ChatIds).StartExtension();
new ShodkaExtension(botClient, UpdateHandler.ChatIds).StartExtension();
await Task.Delay(-1);