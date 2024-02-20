using System.Text;
using System;
using System.IO;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TelegramBot
{
    public class TelegramBotProgram
    {
        public string _filePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "shoppingListData.txt");
        private readonly List<ShoppingList> _shoppingList = new ();
        
        public async Task MessageUpdateAsync(ITelegramBotClient botClient, Update update, //убрать лишний if null
            CancellationToken cancellationToken)
        {
            if (update.CallbackQuery != null)
            {
                InlineKeyboardHandler.InlineKeyboardDataGetting(update.CallbackQuery);
                await InlineKeyboardHandler.InlineKeyboardActionAsync(update.CallbackQuery, botClient, chatId: update.CallbackQuery.From.Id);
            }
            //выводим список по нажатию inline кнопки             
            if (update.Message != null)
            {
                var message = update.Message.Text;
                if (update.CallbackQuery != null)
                {
                    var callbackData = update.CallbackQuery.Data;
                
                    if (int.TryParse(callbackData, out int index))
                    {
                        if (index >= 0 && index < _shoppingList.Count)
                        {
                            _shoppingList[index].IsBought = true;
                            await ShowShoppingListAsync(botClient, update.Message, cancellationToken);
                        }
                    }
                }

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

        private async Task WritingToFile(Update update,ITelegramBotClient botClient, CancellationToken cancellationToken) 
        {
            if (update.Message != null)
                if (update.Message.Text != null)
                    _shoppingList.Add(new ShoppingList
                    {
                        Product = update.Message.Text,
                        IsBought = false
                    });

            string fileContent = await File.ReadAllTextAsync(_filePath, cancellationToken);

            foreach (var item in _shoppingList)
            {
                string newItem = $"{item.Product}";
                if (!fileContent.Contains(newItem))
                {
                    fileContent += newItem + "\n";
                }
            }
            try
            {
                await File.WriteAllTextAsync(_filePath, fileContent, cancellationToken);

                await UserMessageDelete(botClient, update.Message, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при записи в файл: " + ex.Message);
            }
        }
        
        public async Task ShowShoppingListAsync(ITelegramBotClient botClient, Message updateMessage,
            CancellationToken cancellationToken)
        {
            if (File.ReadAllText(_filePath) != "")
            {
                await botClient.SendTextMessageAsync(updateMessage.Chat.Id,
                    $"<u><b>Список покупок:\n\r</b></u>" + File.ReadAllText(_filePath),
                    cancellationToken: cancellationToken, 
                    replyMarkup: Keyboards.CreateInlineKeyboardFromShoppingListFile(_filePath, _shoppingList), 
                    parseMode: ParseMode.Html);
                
                Console.WriteLine("Вызван метод показа списка покупок.");
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
