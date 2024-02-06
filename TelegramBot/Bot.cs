using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TelegramBot
{
    public class Bot
    {
        private List<ShoppingList> shoppingList = new List<ShoppingList>();

        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message.Text;
            await ChatKeyboardAsync(botClient, update.Message, cancellationToken);
            
            switch (message)
            {
                case ("Очистить список"):
                    await ClearShoppingListAsync(botClient, update.Message, cancellationToken);
                    break;
                
                case ("Показать список"):
                     await ShowShoppingListAsync(botClient, update.Message, cancellationToken);
                     break;
                
                default:
                    WritingToFile(update);
                    break;
            }
        }
        public void WritingToFile(Update update)
        {
            shoppingList.Add(new ShoppingList
                {id = shoppingList.Count + 1, product = update.Message.Text, isBought = false }); // Добавляем новый элемент в список покупок

            string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
            string fileContent = File.ReadAllText(filePath);

            foreach (var item in shoppingList)
            {
                string newItem = $"{item.id} : {item.product} - {(item.isBought ? "куплено" : "не куплено")}";
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

        private async Task ShowShoppingListAsync(ITelegramBotClient botClient, Message updateMessage, CancellationToken cancellationToken)
        {
            Console.WriteLine("Метод ShowShoppingListAsync вызван.");
            string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
            await botClient.SendTextMessageAsync(updateMessage.Chat.Id, File.ReadAllText(filePath), cancellationToken: cancellationToken);
        }

        public async Task ClearShoppingListAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            Console.WriteLine("Метод ClearShoppingListAsync вызван.");
            string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
            File.WriteAllText(filePath, "Список покупок:\n\r");
            await botClient.SendTextMessageAsync(message.Chat.Id, "Список очищен", cancellationToken: cancellationToken);
        }

        public Task ChatKeyboardAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Очистить список"),
                    new KeyboardButton("Показать список")
                }
            });
            return Task.CompletedTask;
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }
}
