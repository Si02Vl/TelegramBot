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
            var file = await File.ReadAllTextAsync($"C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/shoppingListData.txt"); //читаем файл
            var button = InlineKeyboardDataGetting(callbackQuery); //передаем button_***_data в этот метод
            var clearButtonData = Regex.Replace(button, "button_|_data", "");//создаем новую переменную, удаляя из button_***_data лишнее

            var lines = file.Split(Environment.NewLine);
            foreach (var i in lines)//проверяем совпадает ли button_***_data с текстом в файле
            {
                if (lines.Contains(clearButtonData))
                {

                }
            }
            //string messageToChat = "Нажатие: " + clearButtonData;
            //await botClient.SendTextMessageAsync(chatId, messageToChat);
        }
    }
}