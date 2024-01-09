﻿using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace Bot
{
    class Bot
    {
        static ITelegramBotClient bot = new TelegramBotClient("5396048539:AAEugLYh52N30Khic5HilGXyQvFDXXvyn94");
        
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введена команда START");
                    return;
                }
                else if (message.Text.ToLower() == "/end")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введена команда END");
                    return;
                }
            }
            
            // Получаем список всех пользователей бота
            var botUpdates = await botClient.GetUpdatesAsync();
            var users = botUpdates.Select(u => u.Message.Chat).Distinct(); // покопаться тут!!!!

            // Отправляем сообщение каждому пользователю
            foreach (var user in users)
            {
                await botClient.SendTextMessageAsync(user.Id, $"Дата сообщения: {message.Date}, " +
                                                                   $"Пользователь: {message.Chat.FirstName}, " +
                                                                   $"Смолвил: {message.Text}");
                await botClient.SendTextMessageAsync(message.Chat, "Бот говорит Всем - Leerooooooy Jenkins!!!");
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
