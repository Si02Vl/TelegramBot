using System.ComponentModel;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class Bot
    {
        //Обработка сообщений
        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            //получаем сообщение
            if (update.Message is { } message && message.From is { } sender &&
                message.Text != null | message.Chat.Type == ChatType.Private)
            {
                //Создаем список покупок
                var shoppingList = new List<ShoppingList>
                {
                    new ShoppingList { product = message.Text, isBought = false }
                };
                
                switch (message.Text)
                {
                    //Запускаем бота, добавляем новые кнопки в меню
                    case "/start":
                        var replyKeyboard = new ReplyKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new KeyboardButton("Очистить список"),
                            },
                            new[]
                            {
                                new KeyboardButton("Список команд"),
                            }
                        });
                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            "Это бот Наталья-морская пехота! \r\nЯ сейчас сяду за руль, а ты вылетишь отсюда!!!",
                            replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                        break;


                    default:
                        if (sender.IsBot == false && message.Text != null)
                        {
                            string text = message.Text;
                            var inlineKeyboard = InlineKeyboardMethod(botClient, cancellationToken, message);
                            {
                                string messageToChat = "Список покупок:\n";
                                foreach (var item in shoppingList)
                                {
                                    messageToChat += $"{item.product} - {(item.isBought ? "куплено" : "не куплено")}\n";
                                }
                                await botClient.SendTextMessageAsync(message.Chat.Id, messageToChat, cancellationToken: cancellationToken); 
                                // Отправить исходное сообщение
                                // await botClient.SendTextMessageAsync(message.Chat.Id,
                                //     $"Пользователь {message.Chat.FirstName} смолвил:" + $"\r\n{text}",
                                //     replyMarkup: inlineKeyboard,
                                //     cancellationToken: cancellationToken);
                                // await MessgeDeleteMethod(botClient, cancellationToken, message);
                            }
                        }
                        break;
                }
            }
        }

        //Инлайн клавиатура к сообщениям
        private static InlineKeyboardMarkup InlineKeyboardMethod(ITelegramBotClient botClient,
            CancellationToken cancellationToken, Message message)
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
        private static async Task MessgeDeleteMethod(ITelegramBotClient botClient, CancellationToken cancellationToken,
            Message message)
        {
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);
        }

        //Обработка ошибок
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        public class ShoppingList
        {
            public int id {get; set;}
            public string product {get; set;}
            public bool isBought {get; set;}
        }

        public class Program
        {
            static async Task Main()
            {
                var botToken = "6958296449:AAFdDLvwL2sxEH4GU-Vo0wj-JsQOb6BDVQw";
                var botClient = new TelegramBotClient(botToken);

                var bot = new Bot();
                botClient.StartReceiving(new DefaultUpdateHandler(bot.MessageUpdateAsync, bot.HandleErrorAsync));

                Console.WriteLine("Bot started. Press any key to exit.");
                await Task.Delay(-1);
                Console.ReadKey();
            }
        }
    }
}
