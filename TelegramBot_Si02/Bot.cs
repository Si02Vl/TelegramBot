using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
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

        //await botClient.StopPollAsync();
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { } message && message.From is { } sender)
        {
            if (message.Chat is { } chat && chat.Type == ChatType.Group && sender.IsBot == false)
            {
                string text = $"Привет, я бот! Вы сказали: {message.Text}";
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