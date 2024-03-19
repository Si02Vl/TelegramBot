using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TelegramBot
{
    public class TelegramBotProgram
    {
        //public string dataFolderPath = "C:/Users/Si02/RiderProjects/TelegramBot_Si02/TelegramBot/Data/";
        public string dataFolderPath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/Data/";
        public List<ShoppingList> shoppingList = new();

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
                //IsDataFileExistOrCreate(update);

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
                        await WriteToFile(update, botClient, cancellationToken);
                        break;
                }
            }
        } // (ОК!)

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        } //для обработки ошибок (ОК!)

        private async Task WriteToFile(Update update, ITelegramBotClient botClient, 
            CancellationToken cancellationToken)
        {
            shoppingList.Add(new ShoppingList
            {
                Product = update.Message.Text,
                IsBought = false,
                ChatId = update.Message.Chat.Id
            });

            var dataFile = await File.ReadAllTextAsync($"{dataFolderPath}{update.Message.Chat.Id}_DataFile.txt", 
                cancellationToken);

            foreach (var item in shoppingList)
            {
                var newItem = $"{item.Product}";
                var chatId = $"{item.ChatId}";
                var isBought = $"{item.IsBought}";

                try //убрать проверку по содержанию, просто ПЕРЕЗАПИСЫВАТЬ из класса в файл
                {
                    if (!dataFile.Contains(newItem))
                    {
                        dataFile += $"{newItem}, ChatID = {chatId} , Bought = {isBought}\n";
                        await File.WriteAllTextAsync($"{dataFolderPath}{update.Message.Chat.Id}_DataFile.txt",
                            dataFile,
                            cancellationToken);
                        await UserMessageDelete(botClient, update.Message, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при записи в файл: " + ex.Message);
                }
            }
        } 

        public async Task ShowShoppingListAsync(ITelegramBotClient botClient, Message updateMessage, //нужна проверка по chatId
            CancellationToken cancellationToken)
        {
            string[] lines = File.ReadAllLines($"{dataFolderPath}{updateMessage.Chat.Id}_DataFile.txt");

            var onlyProductsNamesForMessage = lines.Select(line => 
            {
                string firstWord = line.Split(',').First();
                return firstWord;
            }).ToArray();
            
            List<string> productListForThisChatId = new List<string>();
            
            if (File.ReadAllText($"{dataFolderPath}{updateMessage.Chat.Id}_DataFile.txt") != "")
            {
                //совпадения по chatId пишем в новый список и потом отправляем в сообщение
                
                foreach (string product in onlyProductsNamesForMessage)
                {
                    if (product == updateMessage.Chat.Id.ToString())
                    {
                        productListForThisChatId.Add(product);
                    }
                }
                 
                await botClient.SendTextMessageAsync( 
                    updateMessage.Chat.Id,
                    $"<u><b>Список покупок:\n\r</b></u>" + string.Join("\n\r", onlyProductsNamesForMessage),
                    cancellationToken: cancellationToken,
                    replyMarkup: Keyboards.CreateInlineKeyboardFromShoppingListFile($"{dataFolderPath}{updateMessage.Chat.Id}_DataFile.txt"),
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
        } //вывод списка (OK!)

        private async Task ClearShoppingListAsync(ITelegramBotClient botClient, Update update, Message message,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Вызван метод очистки списка.");
            File.WriteAllText($"{dataFolderPath}{update.Message.Chat.Id}_DataFile.txt", "");
            await botClient.SendTextMessageAsync(message.Chat.Id, "Список очищен.",
                cancellationToken: cancellationToken);
            shoppingList.Clear();
        } //очистка файла (OK!)

        private async Task UserMessageDelete(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);
        } //удаление сообщений в чате (ОК!)

        
        public async Task DeletePurchasedItems(ITelegramBotClient botClient, Message message,
            CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var lines = File.ReadAllLines($"{dataFolderPath}{message.Chat.Id}_DataFile.txt").Where(l => !l.Contains("<s>")).ToArray();
            File.WriteAllLines($"{dataFolderPath}{message.Chat.Id}_DataFile.txt", lines);
            await ShowShoppingListAsync(botClient, message, cancellationToken);
        } 

        /*public void IsDataFileExistOrCreate(Update update)
        {
            string fileName = $"{update.Message.Chat.Id}_DataFile.txt";
            string filePath = Path.Combine(dataFolderPath, fileName);

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                Console.WriteLine("New Data File Created");
            }
            else
            {
                Console.WriteLine("The File Exists");
            }
        } */
    }
}