using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;


class Program
{
    static async Task Main()
    {
        var botToken = "6958296449:AAFdDLvwL2sxEH4GU-Vo0wj-JsQOb6BDVQw";
        var botClient = new TelegramBotClient(botToken);

        botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));

        Console.WriteLine("Bot started. Press any key to exit.");
        Console.ReadKey();
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
                    string imagePath = $"Pictures/Picture.png";
                    using (var photoStream = System.IO.File.OpenRead(imagePath))
                    {
                        var photo = new Telegram.Bot.Types.InputFileStream(photoStream,"Picture.png");
                        await botClient.SendPhotoAsync(message.Chat.Id, photo, cancellationToken: cancellationToken);
                    }
                    break;
                
                case "Привет, бот!":
                    string responseTextGreeting = "И тебе привет, кожанный мешок!";
                    await botClient.SendTextMessageAsync(message.Chat.Id, responseTextGreeting, cancellationToken: cancellationToken);
                    break;
                
                case "/start":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Стартуем!")
                        }
                    });
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Привет! Хозяин что-то пытается, работаю на морально-волевых, а вы подопытные)", replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                    break;
                
                case "Стартуем!":
                    string responseButton = "Еще раз нажмешь - нажму красную кнопку!";
                    await botClient.SendTextMessageAsync(message.Chat.Id, responseButton, cancellationToken: cancellationToken);
                    break;
                
                case "/help":
                    string responseTextHelp = "Список команд: " +  "\r\n/Picture " + "\r\n/start " + "\r\n/sendphoto " + "\r\n/Bot ";
                    await botClient.SendTextMessageAsync(message.Chat.Id, responseTextHelp, cancellationToken: cancellationToken);
                    break;
                
                case "/sendphoto":
                    string imagePathForButton = "Pictures/PictureForButton.png";
                    using (var photoStream = System.IO.File.OpenRead(imagePathForButton))
                    {
                        var photo = new Telegram.Bot.Types.InputFileStream(photoStream,"PictureForButton.png");
                        await botClient.SendPhotoAsync(message.Chat.Id, photo, cancellationToken: cancellationToken);
                    }
                    break;  
                
                default:
                    if (sender.IsBot == false && message.Chat.Type == ChatType.Group)
                    {
                        string matureText = message.Text;
                        // Список матерных слов
                        string[] badWords = { "буй", "звезда", "Игорь" };
                        bool containsBadWord = false;

                        // Проверка наличия матерных слов в сообщении
                        foreach (var word in badWords)
                        {
                            if (matureText.Contains(word))
                            {
                                // Заменить матерное слово на желаемый текст
                                matureText = matureText.Replace(word, "Этот кожанный ругнулся!");
                                containsBadWord = true;
                            }
                            else;
                            {
                                // Отправить сообщение с обработанным текстом
                                await botClient.SendTextMessageAsync(message.Chat.Id, matureText, cancellationToken: cancellationToken);
                            }
                        }
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