using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using System.IO;

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
            switch (message.Text)
            {
                case "/Bot":
                    string responseText = "Привет, я бот! Команда /Bot была распознана. Приступаю к уничтожению человечества! 3..2..1..";
                    await botClient.SendTextMessageAsync(message.Chat.Id, responseText, cancellationToken: cancellationToken);
                    break;
                
                case "/Picture":
                    string imagePath = $"C:\\Users\\Si02\\RiderProjects\\TelegramBot_Si02\\Picture.png";
                    using (var photoStream = System.IO.File.OpenRead(imagePath))
                    {
                        var photo = new Telegram.Bot.Types.InputFileStream(photoStream,"Picture.png");
                        await botClient.SendPhotoAsync(message.Chat.Id, photo, cancellationToken: cancellationToken);
                    }
                    break;
                
                default:
                    if (message.Chat is { } chat && chat.Type == ChatType.Group && sender.IsBot == false)
                    {
                        string text = $"Привет, я бот! {sender.FirstName} сказал: {message.Text}";
                        await botClient.SendTextMessageAsync(chat.Id, text, cancellationToken: cancellationToken);
                    }
                    break;
            }
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}