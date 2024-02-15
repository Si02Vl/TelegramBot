using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class InlineKeyboardHandler
    {
        public static void InlineKeyboardDataGetting(CallbackQuery callbackQuery)
        {
            var buttonCallbackData = callbackQuery.Data;
            Console.WriteLine(buttonCallbackData);
        }

        public async Task InlineKeyboardActionAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
            CancellationToken cancellationToken)
        {
            
        }
    }
}