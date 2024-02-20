using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TelegramBot;

public class Keyboards
{
    public static Task CreateChatKeyboardAsync(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Очистить список"),
                new KeyboardButton("Показать список"),
                new KeyboardButton("Удалить купленное из списка")
            }
        });
        return botClient.SendTextMessageAsync(message.Chat.Id, "Выберите действие на клавиатуре " +
                                                               "или введите покупки отдельными сообщениями.",
            replyMarkup: keyboard, 
            cancellationToken: cancellationToken);
    }
    
    public static InlineKeyboardMarkup CreateInlineKeyboardFromShoppingListFile(string filePath, List<ShoppingList> shoppingList)
    {
        string[] lines = File.ReadAllLines(filePath);

        var buttons = lines.Select(line => new[]
        {
            InlineKeyboardButton.WithCallbackData(line.Replace("<s>", "").Replace("</s>", ""), $"{line}_buttonData")
        }).ToArray();
        return new InlineKeyboardMarkup(buttons);
    }
}