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

        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
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
                    
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Выберите товар", 
                        replyMarkup: InlineKeyboardFromTextFile("C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt"), 
                        cancellationToken: cancellationToken);
                    
                    //CreateInlineKeyboardFromTextFile("C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt");
                    break;

                default:
                    WritingToFile(update);
                    break;
            }
        }

        public void WritingToFile(Update update)
        {
            shoppingList.Add(new ShoppingList
            {
                id = shoppingList.Count + 1, 
                product = update.Message.Text, 
                isBought = false
            });

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

        private async Task ShowShoppingListAsync(ITelegramBotClient botClient, Message updateMessage,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Метод ShowShoppingListAsync вызван.");
            string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
            await botClient.SendTextMessageAsync(updateMessage.Chat.Id, File.ReadAllText(filePath),
                cancellationToken: cancellationToken);
        }

        public async Task ClearShoppingListAsync(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Метод ClearShoppingListAsync вызван.");
            string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
            File.WriteAllText(filePath, "Список покупок:\n\r");
            await botClient.SendTextMessageAsync(message.Chat.Id, "Список очищен",
                cancellationToken: cancellationToken);
        }

        public Task ChatKeyboardAsync(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
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

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        /*public Task InlineKeyboard(ITelegramBotClient botClient, CallbackQuery callbackQuery,
            CancellationToken cancellationToken)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Куплено", "Bought"),
                    }
                });
            
            return Task.CompletedTask;
        }*/

        /*private InlineKeyboardMarkup InlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] // Первая строка инлайн-клавиатуры
                {
                    InlineKeyboardButton.WithCallbackData("Кнопка 1", "button1"),
                },
                new[] // Вторая строка инлайн-клавиатуры
                {
                    InlineKeyboardButton.WithCallbackData("Кнопка 3", "button3"),
                }
            });
        }*/

        public InlineKeyboardMarkup InlineKeyboardFromTextFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            
            var buttons = lines.Select(line => new[]
            {
                InlineKeyboardButton.WithCallbackData(line, $"button_{line.Replace(" ", "_")}_data")
            }).ToArray();

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
