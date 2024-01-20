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
                    //Запускаем бота, добавляем новые кнопки в меню
                    case "/start":
                        var replyKeyboard = new ReplyKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new KeyboardButton("Стартуем!"),
                                new KeyboardButton("Джип в Москве"),
                                new KeyboardButton("Вдох-выдох, упал-отжался!"),
                            },
                            new []
                            {
                                new KeyboardButton("Список команд"),
                                new KeyboardButton("Удаляюсь, не хочу стартовать!"),
                                new KeyboardButton("Просто кнопка")
                            }
                        });
                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            "Это бот Наталья-морская пехота! \r\nЯ сейчас сяду за руль, а ты вылетишь отсюда!!!",
                            replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                        break;

                    //Кнопки
                    case "Стартуем!":
                        string imagePath = $"C:\\Users\\Si02\\RiderProjects\\TelegramBot_Si02\\TelegramBot\\Pictures\\PictureStart.png";
                        using (var photoStream = System.IO.File.OpenRead(imagePath))
                        {
                            var photo = new Telegram.Bot.Types.InputFileStream(photoStream, imagePath);
                            await botClient.SendPhotoAsync(message.Chat.Id, photo,
                                cancellationToken: cancellationToken);
                        }
                        await MessgeDeleteMethod(botClient, cancellationToken, message);
                        break;
                    
                    case "Джип в Москве":
                        await botClient.SendTextMessageAsync(message.Chat.Id, 
                            "Джип в Москве пока не прогрет",
                            cancellationToken: cancellationToken);
                        break;
                    
                    case "Вдох-выдох, упал-отжался!":
                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            $"Пользователь {message.Chat.FirstName} смолвил:" + $"\r\n{message.Text}",
                            cancellationToken: cancellationToken);
                        await MessgeDeleteMethod(botClient, cancellationToken, message);
                        break;

                    case "Список команд":
                        string responseTextHelp = "Список команд: " + "\r\n/start ";
                        await botClient.SendTextMessageAsync(message.Chat.Id, responseTextHelp,
                            cancellationToken: cancellationToken);
                        await MessgeDeleteMethod(botClient, cancellationToken, message);
                        break;
                    
                    case "Удаляюсь, не хочу стартовать!":
                        await botClient.SendTextMessageAsync(message.Chat.Id, 
                            "Выхода нет!",
                            cancellationToken: cancellationToken);
                        break;
                    
                    // case "memberDelete":
                    //     if (message.Text == "Удаляюсь, не хочу стартовать!")
                    //     {
                    //         // Удаляем пользователя из группы
                    //         await botClient.BanChatMemberAsync(message.MessageId, message.From.Id, cancellationToken: cancellationToken);
                    //     }
                    //     break;

                    default:
                        if (sender.IsBot == false && message.Text != null)
                        {
                            string text = message.Text;
                            string textLower = text.ToLower();
                            string replaсedBadWorld = text;
                            bool containsBadWord = false;
                            var inlineKeyboard = InlineKeyboardMethod(botClient, cancellationToken, message);
                            
                            // Список матерных слов
                            string[] badWords;
                            string filePath = "C:\\Users\\Si02\\RiderProjects\\TelegramBot_Si02\\TelegramBot\\Files\\words.txt";
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
                                await MessgeDeleteMethod(botClient, cancellationToken, message);
                            }
                            else
                            {
                                // Отправить исходное сообщение
                                await botClient.SendTextMessageAsync(message.Chat.Id,
                                    $"Пользователь {message.Chat.FirstName} смолвил:" + $"\r\n{text}", replyMarkup: inlineKeyboard,
                                    cancellationToken: cancellationToken);
                                await MessgeDeleteMethod(botClient, cancellationToken, message);
                            }
                        }
                        break;
                }
            }
        }

        //Инлайн клавиатура к сообщениям
        private static InlineKeyboardMarkup InlineKeyboardMethod(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] // Первая строка инлайн-клавиатуры
                {
                    InlineKeyboardButton.WithCallbackData("Кнопка 1", "button1"),
                    InlineKeyboardButton.WithCallbackData("Кнопка 2", "button2"),
                },
                new[] // Вторая строка инлайн-клавиатуры
                {
                    InlineKeyboardButton.WithCallbackData("Кнопка 3", "button3"),
                    InlineKeyboardButton.WithCallbackData("Кнопка 4", "button4"),
                }
            });
            return inlineKeyboard;
        }

        //Удаление сообщений
        private static async Task MessgeDeleteMethod(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message)
        {
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);
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
