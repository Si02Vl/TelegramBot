using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TelegramBot
{
    public class Bot
    {
        private List<ShoppingList> shoppingList = new List<ShoppingList>(); // Список покупок как поле класса

        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            shoppingList.Add(new ShoppingList { product = update.Message.Text, isBought = false }); // Добавляем новый элемент в список покупок

            string filePath = "C:/Users/Si02/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
            string fileContent = File.ReadAllText(filePath);

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

        public Task ClearShoppingListAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            Console.WriteLine("Метод ClearShoppingListAsync вызван.");
            string filePath = "C:/Users/Si02/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
            File.WriteAllText(filePath, "Список покупок:\n\r");

            // Обновляем клавиатуру после очистки списка
            ControlShoppingListKeyboardAsync(botClient, message, cancellationToken);

            return ControlShoppingListKeyboardAsync(botClient, message, cancellationToken);
        }

        public async Task ControlShoppingListKeyboardAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Очистить список"),
                    new KeyboardButton("Показать список")
                }
            });

            await botClient.SendTextMessageAsync(chatId, "Выберите действие:", replyMarkup: keyboard, cancellationToken: cancellationToken);
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        public class Program
        {
            static async Task Main()
            {
                var botToken = "6958296449:AAFdDLvwL2sxEH4GU-Vo0wj-JsQOb6BDVQw";
                var botClient = new TelegramBotClient(botToken);

                var bot = new Bot();
                botClient.StartReceiving(new DefaultUpdateHandler(bot.MessageUpdateAsync, bot.HandleErrorAsync));

                Console.WriteLine("Бот запущен. Нажмите любую клавишу для выхода.");
                await Task.Delay(-1);
                Console.ReadKey();
            }
        }
    }
}