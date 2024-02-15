using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class InlineKeyboardHandler
    {
        public static string InlineKeyboardDataGetting(CallbackQuery callbackQuery)
        {
            var buttonCallbackData = callbackQuery.Data;
            Console.WriteLine(buttonCallbackData);
            return buttonCallbackData;
        }

        public async Task InlineKeyboardActionAsync(CallbackQuery callbackQuery)
        {
            var button = InlineKeyboardDataGetting(callbackQuery);
            
        }
    }
}