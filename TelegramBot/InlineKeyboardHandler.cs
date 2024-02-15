using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class InlineKeyboardHandler
    {
        public static string InlineKeyboardDataGetting(CallbackQuery callbackQuery)
        {
            var buttonCallbackData = callbackQuery.Data;
            return buttonCallbackData;
        }

        public static async Task InlineKeyboardActionAsync(CallbackQuery callbackQuery, ITelegramBotClient botClient, long chatId)
        {
            var button = InlineKeyboardDataGetting(callbackQuery);

            string messageToChat = "Нажатие: " + button;
            await botClient.SendTextMessageAsync(chatId, messageToChat);
        }
    }
}