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
            //var filePath = $"C:/Users/Si02/RiderProjects/TelegramBot_Si02/TelegramBot/Data/{chatId}_DataFile.txt";
            var filePath = $"C:/Users/user/RiderProjects/TelegramBot_Si02/TelegramBot/Data/{chatId}_DataFile.txt";
            var product = await File.ReadAllLinesAsync(filePath); 
            
            var onlyProductsName = product.Select(line => 
            {
                string firstWord = line.Split(',').First();
                return firstWord;
                
            }).ToArray();
    
            var button = InlineKeyboardDataGetting(callbackQuery); 
            var clearButtonData = button.Split("_").First(); //до этого момента работает
            
            for (int i = 0; i < onlyProductsName.Length; i++)
            {
                if (onlyProductsName[i] == clearButtonData)
                {
                    // зачеркиваем при совпадении
                    onlyProductsName[i] = $"<s>{onlyProductsName[i]}</s>, ChatID = {chatId} , Bought = Yes!\n"; 
                    
                    var updatedFileContent = string.Join(Environment.NewLine, product); // пишет не совсем то, что нужно 
                   
                    await File.WriteAllTextAsync(filePath, product);
                    //удаляем сообщение и обновляем список в чате
                    await botClient.DeleteMessageAsync(chatId, callbackQuery.Message.MessageId);
            
                    var bot = new TelegramBotProgram();
                    await bot.ShowShoppingListAsync(botClient, callbackQuery.Message, CancellationToken.None);
            
                    break;
                }
            }
        }
    }
}