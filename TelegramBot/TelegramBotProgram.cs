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
        } // (ОК!)

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        } //для обработки ошибок (ОК!)

        //все чаты/группы добавляются в список экземпляров класса ShoppingList и данные пишутся потом беспорядочно в файл данных
        //или задать в классе поле chatId и по этому полю сортировать в файлы
        private async Task WritingToFile(Update update, ITelegramBotClient botClient, 
            CancellationToken cancellationToken, long chatOrGroupId) //long chatOrGroupId не нужен???
        {
            if (update.Message != null)
                if (update.Message.Text != null)
                    shoppingList.Add(new ShoppingList
                    {
                        Product = update.Message.Text,
                        IsBought = false,
                        ChatId = update.Message.Chat.Id
                    });

            string dataFile = await File.ReadAllTextAsync($"{dataFolderPath}{update.Message.Chat.Id}_DataFile.txt", 
                cancellationToken);

            foreach (var item in shoppingList) // тут все ок
            {
                string newItem = $"{item.Product}";
                string chatId = $"{item.ChatId}";
                string isBought = $"{item.IsBought}";

                try
                {
                    if (!dataFile.Contains(newItem))
                    {
                        dataFile += $"Product = {newItem}, ChatID = {chatId} , Bought = {isBought}\n";
                    }
                    if (item.ChatId == update.Message.Chat.Id)
                    {
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

        public async Task ShowShoppingListAsync(ITelegramBotClient botClient, Message updateMessage,
            CancellationToken cancellationToken)
        {
            if (File.ReadAllText($"{dataFolderPath}{updateMessage.Chat.Id}_DataFile.txt") != "")
            {
                await botClient.SendTextMessageAsync( 
                    updateMessage.Chat.Id,
                    $"<u><b>Список покупок:\n\r</b></u>" + File.ReadAllText($"{dataFolderPath}{updateMessage.Chat.Id}_DataFile.txt"),
                    cancellationToken: cancellationToken,
                    replyMarkup: Keyboards.CreateInlineKeyboardFromShoppingListFile($"{dataFolderPath}{updateMessage.Chat.Id}_DataFile.txt",
                        shoppingList),
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
        } //очистка файла (OK!) но разобраться с {dataFolderPath}{update.Message.Chat.Id}

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
        } //в классе обработчика клавиатур не правильный адрес!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        public void IsDataFileExistOrCreate(Update update)
        {
            string fileName = $"{update.Message.Chat.Id}_DataFile.txt";
            string[] files = Directory.GetFiles(dataFolderPath);

            if (!files.Contains(fileName)) //всегда выполняется почему?
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