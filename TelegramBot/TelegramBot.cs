using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;
using TelegramBot = TelegramBot.Keyboard;


namespace TelegramBot
{
    public class TelegramBot
    {
        string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
        //string filePath = "C:/Users/Si02/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
        private List<ShoppingList> shoppingList = new List<ShoppingList>();

        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            if (update.Message != null)
            {
                var message = update.Message.Text;

                switch (message)
                {
                    case ("/start"):
                        await Keyboard.CreateChatKeyboardAsync(botClient, update.Message, cancellationToken);
                        break;

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
        }
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
        public void WritingToFile(Update update)
        {
            if (update.Message != null)
                if (update.Message.Text != null)
                    shoppingList.Add(new ShoppingList
                    {
                        product = update.Message.Text,
                        isBought = false
                    });

            string fileContent = File.ReadAllText(filePath);

            foreach (var item in shoppingList)
            {
                string newItem = $"{item.product}"; //- {(item.isBought ? "куплено" : "не куплено")};
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
        private async Task ShowShoppingListAsync(ITelegramBotClient botClient, Message updateMessage,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Вызван метод показа списка покупок.");
            if (File.ReadAllText(filePath) != "")
            {
                await botClient.SendTextMessageAsync(updateMessage.Chat.Id,
                    $"Список покупок:\n\r" + File.ReadAllText(filePath),
                    cancellationToken: cancellationToken, replyMarkup: Keyboard.CreateInlineKeyboardFromShoppingListFile(filePath));
            }
            else
            {
                await botClient.SendTextMessageAsync(updateMessage.Chat.Id,
                    $"Список покупок пуст", cancellationToken: cancellationToken);
            }
        }
        public async Task ClearShoppingListAsync(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Вызван метод очистки списка.");
            File.WriteAllText(filePath, "");
            await botClient.SendTextMessageAsync(message.Chat.Id, "Список очищен.",
                cancellationToken: cancellationToken);
        }
    }
}
