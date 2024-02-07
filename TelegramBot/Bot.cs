using System.Diagnostics;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
                    
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Выберите товар:", 
                            replyMarkup: InlineKeyboardFromTextFile(filePath), 
                            cancellationToken: cancellationToken);
                        //await HandleCallbackQueryAsync(botClient, update.CallbackQuery, null, null, update.Message.Chat.Id);
                        break;

                    default:
                        WritingToFile(update);
                        break;
                }
            }
        }

        public void WritingToFile(Update update)
        {
            shoppingList.Add(new ShoppingList
            {
                product = update.Message.Text, 
                isBought = false
            });

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
            await botClient.SendTextMessageAsync(updateMessage.Chat.Id, $"Список покупок:\n\r" + File.ReadAllText(filePath),
                cancellationToken: cancellationToken, parseMode: ParseMode.Markdown);
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
            return botClient.SendTextMessageAsync(message.Chat.Id, "Выберите действие:", replyMarkup: keyboard, cancellationToken: cancellationToken);
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
        
        private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, string[] buttons, string[] lines, long chatId)
        {
            string callbackData = callbackQuery.Data;
            int buttonIndex = Array.FindIndex(buttons, row => row.Any(button => 
                $"button_{button}_data" == callbackData));

            if (buttonIndex != -1)
            {
                string selectedLine = lines[buttonIndex];
                lines[buttonIndex] = $"~~{selectedLine}~~";
                await botClient.SendTextMessageAsync(chatId, string.Join("\n", lines));
            }
        }
    }
}
