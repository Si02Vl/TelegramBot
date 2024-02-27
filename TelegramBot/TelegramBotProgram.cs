using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TelegramBot
{
    public class TelegramBotProgram
    {
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
                IsDataFileExistOrCreate(update);
                
                switch (message)
                {
                    case ("/start"):
                        await Keyboards.CreateChatKeyboardAsync(botClient, update.Message, cancellationToken);
                        break;

                    case ("Очистить список"):
                        await ClearShoppingListAsync(botClient, update, update.Message, cancellationToken);
                        break;

                    case ("Показать список"):
                        await ShowShoppingListAsync(botClient, update.Message, cancellationToken);
                        break;

                    case ("Удалить купленное из списка"):
                        await DeletePurchasedItems(botClient, update.Message, update.CallbackQuery,
                            cancellationToken);
                        break;

                    default:
                        await WritingToFile(update, botClient, cancellationToken, update.Message.Chat.Id);
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

            string dataFile = await File.ReadAllTextAsync($"{dataFolderPath}{update.Message.Chat.Id}_DataFile.txt", cancellationToken);
 
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
                await File.WriteAllTextAsync($"{dataFolderPath}{update.Message.Chat.Id}_DataFile.txt", dataFile, cancellationToken);

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
            if (File.ReadAllText($"{dataFolderPath}{updateMessage.Chat.Id}_DataFile.txt") != "")
            {
                await botClient.SendTextMessageAsync(
                    updateMessage.Chat.Id,
                    $"<u><b>Список покупок:\n\r</b></u>" + File.ReadAllText($"{dataFolderPath}_DataFile.txt"),
                    cancellationToken: cancellationToken,
                    replyMarkup: Keyboards.CreateInlineKeyboardFromShoppingListFile($"{dataFolderPath}_DataFile.txt", _shoppingList),
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

        private async Task ClearShoppingListAsync(ITelegramBotClient botClient, Update update, Message message,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Вызван метод очистки списка.");
            File.WriteAllText($"{dataFolderPath}{update.Message.Chat.Id}_DataFile.txt", "");
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
            var lines = File.ReadAllLines(dataFolderPath).Where(l => !l.Contains("<s>")).ToArray();
            File.WriteAllLines(dataFolderPath, lines);
            await ShowShoppingListAsync(botClient, message, cancellationToken);
        }
        
        public void IsDataFileExistOrCreate(Update update)
        {
            string fileName = $"{update.Message.Chat.Id}_DataFile.txt";
            string[] files = Directory.GetFiles(dataFolderPath);

            if (files.Contains(fileName))
            {
                File.Create(Path.Combine(dataFolderPath, fileName)).Close();
                Console.WriteLine("New Data File Created");
            }
            else
            {
                Console.WriteLine("The File Exists");
            }
        } // проверка на существование файла (OK!)
    }
}
