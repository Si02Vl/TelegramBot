using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TelegramBot
{
    public class TelegramBotProgram
    {
        string filePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "shoppingListData.txt");
        private List<ShoppingList> shoppingList = new ();
        
        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            if (update.CallbackQuery != null)
            {
                InlineKeyboardHandler.InlineKeyboardDataGetting(update.CallbackQuery);
                InlineKeyboardHandler.InlineKeyboardActionAsync(update.CallbackQuery);
            }

            if (update.Message != null)
            {
                var message = update.Message.Text;

                switch (message)
                {
                    case ("/start"):
                        await Keyboards.CreateChatKeyboardAsync(botClient, update.Message, cancellationToken);
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

        private void WritingToFile(Update update)
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
                    cancellationToken: cancellationToken, 
                    replyMarkup: Keyboards.CreateInlineKeyboardFromShoppingListFile(filePath));
            }
            else
            {
                await botClient.SendTextMessageAsync(updateMessage.Chat.Id,
                    $"Список покупок пуст", cancellationToken: cancellationToken);
            }
        }

        private async Task ClearShoppingListAsync(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Вызван метод очистки списка.");
            File.WriteAllText(filePath, "");
            await botClient.SendTextMessageAsync(message.Chat.Id, "Список очищен.",
                cancellationToken: cancellationToken);
        }
    }
}
