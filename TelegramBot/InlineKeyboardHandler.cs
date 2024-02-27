using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;
using System.Text.RegularExpressions;
    
namespace TelegramBot
{
    
    public class InlineKeyboardHandler
    {
        //получаем button_***_data
        public static string InlineKeyboardDataGetting(CallbackQuery callbackQuery)
        {
            var buttonCallbackData = callbackQuery.Data;
            return buttonCallbackData;
        }

        public static async Task InlineKeyboardActionAsync(CallbackQuery callbackQuery, ITelegramBotClient botClient, 
            long chatId)
        {
            var filePath = $"C:/Users/Si02/RiderProjects/TelegramBot_Si02/TelegramBot/Data/{chatId}_DataFile.txt";
            //var filePath = $"C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/Data/{chatId}_DataFile.txt";
            var items = await File.ReadAllLinesAsync(filePath); 
            
            var button = InlineKeyboardDataGetting(callbackQuery); 
            var clearButtonData = Regex.Replace(button, "_buttonData", "");

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == clearButtonData)
                {
                    items[i] = $"<s>{items[i]}</s>"; 
                    var updatedFileContent = string.Join(Environment.NewLine, items);
                    await File.WriteAllTextAsync(filePath, updatedFileContent);
                    await botClient.DeleteMessageAsync(chatId, callbackQuery.Message.MessageId);
                    
                    var bot = new TelegramBotProgram();
                    await bot.ShowShoppingListAsync(botClient, callbackQuery.Message, CancellationToken.None);
                    
                    break;
                }
            }
        }
    }
}