using System;
using System.Collections;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TelegramBot
{
    public class Bot
    {
        //Обработка сообщений
        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            //Создаем список покупок
            var shoppingList = new List<ShoppingList>
            {
                new ShoppingList { product = update.Message.Text, isBought = false }
            };

            // Запись сообщения в файл
            string filePath = "C:/Users/Si02/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt"; // путь к файлу для записи
            string fileContent = File.ReadAllText(filePath);
            //string newItem = update.Message.Text;

            foreach (var item in shoppingList)
            {
                string newItem = $"{item.product} - {(item.isBought ? "куплено" : "не куплено")}";
                if (!fileContent.Contains(newItem))
                {
                    fileContent += newItem + "\n";
                }
            }
            
            try
            {
                File.WriteAllText(filePath, fileContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при записи в файл: " + ex.Message);
            }
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
