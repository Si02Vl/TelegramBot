using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TelegramBot;

public class Keyboard
{
    public static Task ChatKeyboardAsync(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Очистить список"),
                new KeyboardButton("Показать список")
            }
        });
        return botClient.SendTextMessageAsync(message.Chat.Id, "Выберите действие на клавиатуре " +
                                                               "или введите покупки отдельными сообщениями.",
            replyMarkup: keyboard, cancellationToken: cancellationToken);
    }
    
    public static InlineKeyboardMarkup InlineKeyboardFromTextFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        var buttons = lines.Select(line => new[]
        {
            InlineKeyboardButton.WithCallbackData(line, $"button_{line.Replace(" ", "_")}_data")
        }).ToArray();
        return new InlineKeyboardMarkup(buttons);
    }
}