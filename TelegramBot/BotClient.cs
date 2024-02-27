using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace TelegramBot;

public static class BotClient
{
    static async Task Main()
    {
        var botToken = "7064515716:AAFTEbS60b5WLHLZtjR6XLUeUiWfVo3oH_A";
        var botClient = new TelegramBotClient(botToken);

        var bot = new TelegramBotProgram();
        botClient.StartReceiving(new DefaultUpdateHandler(bot.MessageUpdateAsync, bot.HandleErrorAsync));
            
        Console.WriteLine("Bot started. Press any key to exit.");
        await Task.Delay(-1);
        Console.ReadKey();
    }
}