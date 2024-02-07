using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace TelegramBot;

public class Program
{
    static async Task Main()
    {
        var botToken = "6958296449:AAFdDLvwL2sxEH4GU-Vo0wj-JsQOb6BDVQw";
        var botClient = new TelegramBotClient(botToken);

        var bot = new Bot();
        botClient.StartReceiving(new DefaultUpdateHandler(bot.MessageUpdateAsync, bot.HandleErrorAsync));
            
        Console.WriteLine("Bot started. Press any key to exit.");
        await Task.Delay(-1);
        Console.ReadKey();
    }
}