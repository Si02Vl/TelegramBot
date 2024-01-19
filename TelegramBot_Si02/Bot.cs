using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot_Si02
{
    class Bot
    {
        //Обработка сообщений
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { } message && message.From is { } sender &&
                message.Text != null | message.Chat.Type == ChatType.Private)
            {
                switch (message.Text)
                {
                    //Запускаем бота, добавляем новые кнопки
                    case "/start":
                        var replyKeyboard = new ReplyKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new KeyboardButton("Стартуем!"),
                                new KeyboardButton("Джип в Москве"),
                                new KeyboardButton("Вдох-выдох, упал-отжался!"),
                                new KeyboardButton("Список команд")
                            }
                        });
                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            "Это бот Наталья-морская пехота! \r\nЯ сейчас сяду за руль, а ты вылетишь отсюда!!!",
                            replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                        break;


                    //Кнопки                
                    // case "Стартуем!":
                    //     await botClient.SendTextMessageAsync(message.Chat.Id, 
                    //         "Еще раз нажмешь - нажму красную кнопку!",
                    //         cancellationToken: cancellationToken);
                    //     break;

                    case "Джип в Москве":
                        await botClient.SendTextMessageAsync(message.Chat.Id, 
                            "Джип в Москве",
                            cancellationToken: cancellationToken);
                        break;
                    
                    case "Список команд":
                        string responseTextHelp = "Список команд: " + "\r\n/picture " + "\r\n/start " +
                                                  "\r\n/sendphoto " + "\r\n/bot ";
                        await botClient.SendTextMessageAsync(message.Chat.Id, responseTextHelp,
                            cancellationToken: cancellationToken);
                        break;

                    //Команды
                    case "/bot":
                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            "Команда /bot была распознана. Приступаю к уничтожению человечества! 3..2..1..",
                            cancellationToken: cancellationToken);
                        break;

                    case "Стартуем!":
                        string imagePath = $"Pictures/PictureStart.png";
                        using (var photoStream = System.IO.File.OpenRead(imagePath))
                        {
                            var photo = new Telegram.Bot.Types.InputFileStream(photoStream, "Picture.png");
                            await botClient.SendPhotoAsync(message.Chat.Id, photo,
                                cancellationToken: cancellationToken);
                        }

                        break;


                    default:
                        if (sender.IsBot == false && message.Text != null)
                        {
                            string text = message.Text;
                            string textLower = text.ToLower();
                            string replaсedBadWorld = text;
                            bool containsBadWord = false;

                            // Список матерных слов
                            string[] badWords;
                            string filePath = "C:\\Users\\Si02\\RiderProjects\\TelegramBot_Si02\\words.txt";
                            badWords = System.IO.File.ReadAllLines(filePath);

                            foreach (var word in badWords)
                            {
                                // Поиск матерных слов
                                if (textLower.Contains(word))
                                {
                                    // Заменить матерное слово на желаемый текст
                                    replaсedBadWorld = textLower.Replace(word, " *тут был мат* ");
                                    containsBadWord = true;
                                    break;
                                }
                            }

                            if (containsBadWord)
                            {
                                // Отправить сообщение с замененными матерными словами
                                await botClient.SendTextMessageAsync(message.Chat.Id,
                                    $"Пользователь {message.Chat.FirstName} смолвил:" + $"\r\n{replaсedBadWorld}",
                                    cancellationToken: cancellationToken);
                                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                                    cancellationToken: cancellationToken);
                            }
                            else
                            {
                                // Отправить исходное сообщение
                                await botClient.SendTextMessageAsync(message.Chat.Id,
                                    $"Пользователь {message.Chat.FirstName} смолвил:" + $"\r\n{text}",
                                    cancellationToken: cancellationToken);
                                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                                    cancellationToken: cancellationToken);
                            }
                        }
                        break;
                }
            }
        }
        //Обработка ошибок
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }
    
    class Program
    {
        static async Task Main()
        {
            var botToken = "6958296449:AAFdDLvwL2sxEH4GU-Vo0wj-JsQOb6BDVQw";
            var botClient = new TelegramBotClient(botToken);

            var bot = new Bot();
            botClient.StartReceiving(new DefaultUpdateHandler(bot.HandleUpdateAsync, bot.HandleErrorAsync));

            Console.WriteLine("Bot started. Press any key to exit.");
            await Task.Delay(-1);
            Console.ReadKey();
        }
    }
}