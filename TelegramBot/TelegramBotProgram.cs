using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TelegramBot
{
    public class TelegramBotProgram
    {
        private readonly string _filePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "shoppingListData.txt");
        private readonly List<ShoppingList> _shoppingList = new ();
        
        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            if (update.CallbackQuery != null)
            {
                InlineKeyboardHandler.InlineKeyboardDataGetting(update.CallbackQuery);
                await InlineKeyboardHandler.InlineKeyboardActionAsync(update.CallbackQuery, botClient, chatId: update.CallbackQuery.From.Id);
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
                        WritingToFile(update, botClient, cancellationToken);
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

        private void WritingToFile(Update update,ITelegramBotClient botClient, CancellationToken cancellationToken) 
        {
            if (update.Message != null)
                if (update.Message.Text != null)
                    _shoppingList.Add(new ShoppingList
                    {
                        Product = update.Message.Text,
                        IsBought = false
                    });

            string fileContent = File.ReadAllText(_filePath);

            foreach (var item in _shoppingList)
            {
                string newItem = $"{item.Product}"; //- {(item.isBought ? "куплено" : "не куплено")};
                if (!fileContent.Contains(newItem))
                {
                    fileContent += newItem + "\n";
                }
            }
            try
            {
                File.WriteAllText(_filePath, fileContent);

                this.UserMessageDelete(botClient, update.Message, cancellationToken);
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
            if (File.ReadAllText(_filePath) != "")
            {
                await botClient.SendTextMessageAsync(updateMessage.Chat.Id,
                    $"Список покупок:\n\r" + File.ReadAllText(_filePath),
                    cancellationToken: cancellationToken, 
                    replyMarkup: Keyboards.CreateInlineKeyboardFromShoppingListFile(_filePath));
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
            File.WriteAllText(_filePath, "");
            await botClient.SendTextMessageAsync(message.Chat.Id, "Список очищен.",
                cancellationToken: cancellationToken);
        }
        private async Task UserMessageDelete(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);
        }
    }
}
