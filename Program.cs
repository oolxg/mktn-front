using MakeathonBot.Handlers;
using Telegram.Bot;

var botToken = System.Environment.GetEnvironmentVariable("TG_BOT_TOKEN")!;

var botClient = new TelegramBotClient(botToken);

botClient.StartReceiving(
    UpdateHandler.HandleUpdateAsync,
    PollingErrorHandler.HandleErrorAsync
);

Console.WriteLine("Started receiving updates from Telegram.");

while (true);