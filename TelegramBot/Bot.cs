using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TelegramBot
{
    public class Bot
    {
        string filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingList.txt";
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
                        await ChatKeyboardAsync(botClient, update.Message, cancellationToken);
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
            Console.WriteLine("Метод показа списка покупок вызван.");
            await botClient.SendTextMessageAsync(updateMessage.Chat.Id,
                $"Список покупок:\n\r" + File.ReadAllText(filePath),
                cancellationToken: cancellationToken, replyMarkup: InlineKeyboardFromTextFile(filePath));
        }

        public async Task ClearShoppingListAsync(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Метод очистки списка вызван.");
            File.WriteAllText(filePath, "");
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
            return botClient.SendTextMessageAsync(message.Chat.Id, "Выберите действие на клавиатуре " +
                                                                   "или введите покупки отдельными сообщениями.",
                replyMarkup: keyboard, cancellationToken: cancellationToken);
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

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
