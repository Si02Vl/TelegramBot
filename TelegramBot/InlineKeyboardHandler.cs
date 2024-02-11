using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot;

public class InlineKeyboardHandler
{
    private readonly ITelegramBotClient _botClient;

    public InlineKeyboardHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleInlineKeyboardAction(CallbackQuery callbackQuery)
    {
        // Получаем данные обратного вызова
        string callbackData = callbackQuery.Data;

        // Обработка действий в зависимости от данных обратного вызова
        if (callbackData.StartsWith("button_"))
        {
            // Извлечь текст из данных обратного вызова
            string buttonText = callbackData.Split("_data")[0].Replace("button_", "").Replace("_", " ");
            
            // Обрабатываем действие
            await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Выбрано: {buttonText}", cancellationToken: default);
        }
        // Добавьте обработку других действий по мере необходимости
    }
}