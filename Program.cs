using MakeathonBot.Handlers;
using Telegram.Bot;

const string botToken = "7012923654:AAGjUfGdayBJ9LS5PxZZFZD_zWWmYHZceMw";

var botClient = new TelegramBotClient(botToken);

botClient.StartReceiving(
    UpdateHandler.HandleUpdateAsync,
    PollingErrorHandler.HandleErrorAsync
);

Console.WriteLine("Started receiving updates from Telegram.");

while (true);