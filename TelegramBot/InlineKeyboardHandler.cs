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
            var file = await File.ReadAllTextAsync(filePath); 
            var button = InlineKeyboardDataGetting(callbackQuery); 
            var clearButtonData = Regex.Replace(button, "button_|_data", "");

            var lines = file.Split(Environment.NewLine);
    
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(clearButtonData))
                {
                    // добавляем зачеркивание к строке
                    lines[i] = $"<s>{lines[i]}</s>";
                    // отправляем сообщение
                    string messageToChat = "Нажатие: " + clearButtonData;
                    await botClient.SendTextMessageAsync(chatId, messageToChat, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    break;
                }
            }

            var updatedFileContent = string.Join(Environment.NewLine, lines);
            await File.WriteAllTextAsync(filePath, updatedFileContent);
        }
    }
}