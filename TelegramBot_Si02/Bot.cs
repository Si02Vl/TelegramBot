using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;

class Program
{
    static async Task Main()
    {
        var botToken = "6612286833:AAEOSOuC0GppURuDZdUN2KkUzwrGUSy5lFQ";
        var botClient = new TelegramBotClient(botToken);

        botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));

        Console.WriteLine("Bot started. Press any key to exit.");
        Console.ReadKey();

        //await botClient.StopReceivingAsync();
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { } message && message.From is { } sender)
        {
            if (message.Text == @"\Bot")
            {
                string text = "Привет, я бот! Команда Bot была распознана. Приступаю к уничтожению человечества! 3..2..1..";
                await botClient.SendTextMessageAsync(message.Chat.Id, text, cancellationToken: cancellationToken);
            }
            else if (message.Chat is { } chat && chat.Type == ChatType.Group && sender.IsBot == false)
            {
                string text = $"Привет, я бот! {sender.FirstName} сказал: {message.Text}";
                await botClient.SendTextMessageAsync(chat.Id, text, cancellationToken: cancellationToken);
            }
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}