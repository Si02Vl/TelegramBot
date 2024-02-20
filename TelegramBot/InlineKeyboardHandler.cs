using System.Net.Mime;
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

        //нужно сравнить button_***_data из callbackquery (оставив ***) с текстом в файле
        public static async Task InlineKeyboardActionAsync(CallbackQuery callbackQuery, ITelegramBotClient botClient, long chatId)
        {
            var filePath = "C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingListData.txt";
            var items = await File.ReadAllLinesAsync(filePath); 
            
            var button = InlineKeyboardDataGetting(callbackQuery); 
            var clearButtonData = Regex.Replace(button, "button_|_data", "");

            bool found = false;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == clearButtonData)
                {
                    // добавляем зачеркивание к совпавшей строке
                    items[i] = $"<s>{items[i]}</s>"; 
                    var updatedFileContent = string.Join(Environment.NewLine, items);
                    await File.WriteAllTextAsync(filePath, updatedFileContent);
                    break;
                }

            }
        }
    }
}