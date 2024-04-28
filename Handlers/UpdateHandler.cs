using System.Text;
using MakeathonBot.API;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MakeathonBot.Handlers
{
    public class UpdateHandler
    {
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
        {
            if (update.Type is not UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            if (message is null)
            {
                return;
            }
            
            Console.WriteLine($"Received a message from {message.Chat.Id}");

            switch (message.Text)
            {
                case "/start":
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "[Type a message to start dialog with DriveMate AI]",
                        cancellationToken: cancellationToken,
                        parseMode: ParseMode.Markdown
                    );
                    return;
                case "/history":
                    await HandleHistoryCommandAsync(botClient, message, cancellationToken);
                    return;
                case "/reset":
                    return;
                default:
                    await HandleMessageAsync(botClient, message, cancellationToken);
                    return;
            }
        }
        
        private static async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken = default)
        {
            if (message.Text is null)
            {
                return;
            }
                        
            await botClient.SendChatActionAsync(
                chatId: message.Chat.Id,
                chatAction: ChatAction.Typing,
                cancellationToken: cancellationToken
            );

            try
            {
                var response = await ApiClient.SendUserMessageAsync(message.Text, message.Chat.Id);
            
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: response,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception e)
            {
                await Task.Delay(3000);

                try
                {
                    var response = await ApiClient.SendUserMessageAsync(message.Text, message.Chat.Id);
            
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: response,
                        cancellationToken: cancellationToken
                    );
                } 
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Sorry, I'm not available right now. Please try again just a moment later:(",
                        cancellationToken: cancellationToken
                    );
                }
            }
            
        }
        
        private static async Task ResetChatAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken = default)
        {
            await ApiClient.ResetChatHistoryAsync(message.Chat.Id);
            
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Chat history has been reset."
            );
        }
        
        private static async Task HandleHistoryCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken = default)
        {

            // convert response to a string
            var responseBuilder = new StringBuilder();
            foreach (var item in await ApiClient.GetChatHistoryAsync(message.Chat.Id))
            {
                responseBuilder.AppendLine($"*{item["role"]}*: {item["text"]}");
            }
            
            var response = responseBuilder.ToString();
            
            await botClient.SendChatActionAsync(
                chatId: message.Chat.Id,
                chatAction: ChatAction.Typing,
                cancellationToken: cancellationToken
            );
            
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: response,
                cancellationToken: cancellationToken
            );
        }
    }
}