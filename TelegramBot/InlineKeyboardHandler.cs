using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class InlineKeyboardHandler
    {
        private ITelegramBotClient _botClient;
        private string buttonCallbackData;

        public static void InlineKeyboardDataGetting(CallbackQuery callbackQuery)
        {
            var buttonCallbackData = callbackQuery.Data;
            Console.WriteLine(buttonCallbackData);
        }
    }
}