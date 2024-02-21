using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TelegramBot
{
    public class TelegramBotProgram
    {
        public string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/Data/DataFile.txt";    
        public string dataFolderPath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/Data/";
        
        public List<ShoppingList> _shoppingList = new();
       
        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            if (update.CallbackQuery != null)
            {
                InlineKeyboardHandler.InlineKeyboardDataGetting(update.CallbackQuery);
                await InlineKeyboardHandler.InlineKeyboardActionAsync(update.CallbackQuery, botClient,
                    chatId: update.CallbackQuery.From.Id);
                
            }
            
            if (update.Message != null)
            {
                var message = update.Message.Text;
                var chatOrGroupId = update.Message.Chat.Id;
                IsDataFileExist(dataFolderPath, chatOrGroupId);
                
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

                    case ("Удалить купленное из списка"):
                        await DeletePurchasedItems(botClient, update.Message, update.CallbackQuery,
                            cancellationToken);
                        break;

                    default:
                        await WritingToFile(update, botClient, cancellationToken, chatOrGroupId);
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

        private async Task WritingToFile(Update update, ITelegramBotClient botClient,
            CancellationToken cancellationToken, long chatOrGroupId)
        {
            if (update.Message != null)
                if (update.Message.Text != null)
                    _shoppingList.Add(new ShoppingList
                    {
                        Product = update.Message.Text,
                        IsBought = false
                    });

            string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/Data/" + $"{chatOrGroupId}_DataFile.txt";
            string dataFile = await File.ReadAllTextAsync(filePath, cancellationToken);
 
            foreach (var item in _shoppingList)
            {
                string newItem = $"{item.Product}";
                if (!dataFile.Contains(newItem))
                {
                    dataFile += newItem + "\n";
                }
            }

            try
            {
                await File.WriteAllTextAsync(filePath, dataFile, cancellationToken);

                await UserMessageDelete(botClient, update.Message, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при записи в файл: " + ex.Message);
            }
        }

        public async Task ShowShoppingListAsync(ITelegramBotClient botClient, Message updateMessage, ///////////////читает файл по глобальной переменной, переделать
            CancellationToken cancellationToken)
        {
            string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/Data/" + $"{chatOrGroupId}_DataFile.txt";
            if (File.ReadAllText(filePath) != "")
            {
                await botClient.SendTextMessageAsync(
                    updateMessage.Chat.Id,
                    $"<u><b>Список покупок:\n\r</b></u>" + File.ReadAllText(filePath),
                    cancellationToken: cancellationToken,
                    replyMarkup: Keyboards.CreateInlineKeyboardFromShoppingListFile(filePath, _shoppingList),
                    parseMode: ParseMode.Html);

                Console.WriteLine("Вызван метод показа списка покупок.");
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    updateMessage.Chat.Id,
                    $"Список покупок пуст",
                    cancellationToken: cancellationToken);
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

        private async Task UserMessageDelete(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);
        }

        public async Task DeletePurchasedItems(ITelegramBotClient botClient, Message message,
            CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var lines = File.ReadAllLines(filePath).Where(l => !l.Contains("<s>")).ToArray();
            File.WriteAllLines(filePath, lines);
            await ShowShoppingListAsync(botClient, message, cancellationToken);
        }

        public void IsDataFileExist(string dataFolderPath, long chatOrGroupId)
        {
            string fileName = $"{chatOrGroupId}_DataFile.txt";
            string[] files = Directory.GetFiles(dataFolderPath);
    
            if (!files.Contains(fileName))
            {
                File.Create(Path.Combine(dataFolderPath, fileName)).Close();
            }
            else
            {
                Console.WriteLine("Файл существует");
            }
        }
    }
}
