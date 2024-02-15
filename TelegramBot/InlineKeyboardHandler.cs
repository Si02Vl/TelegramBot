using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class InlineKeyboardHandler
    {
        public static string InlineKeyboardDataGetting(CallbackQuery callbackQuery)
        {
            var buttonCallbackData = callbackQuery.Data;
            //Console.WriteLine(buttonCallbackData);
            return buttonCallbackData;
        }

        public static void InlineKeyboardActionAsync(CallbackQuery callbackQuery)
        {
            var button = InlineKeyboardDataGetting(callbackQuery);

            switch (button)
            {
                case "button_123_data":
                    Console.WriteLine("Нажато 123");
                    break;
                case "button_456_data":
                    Console.WriteLine("Нажато 456");
                    break;
                case "button_789_data":
                    Console.WriteLine("Нажато 789");
                    break;
                default:
                    Console.WriteLine(button);
                    break;
            }
        }
    }
}