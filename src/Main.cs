using ModeratorBot.BotFunctionality.Callbacks;
using ModeratorBot.BotFunctionality.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot
{
    public static class Program
    {
        private static TelegramBotClient? bot;
        private static CancellationTokenSource? cts;

        private static CommandRegistry? commandRegistry;
        private static CallbackRegistry? callbackRegistry;

        /// <summary>
        /// Bot initialization.
        /// </summary>
        public static async Task Main()
        {
            Logger.Debug("Bot starting");

            cts = new CancellationTokenSource();

            bot = Secrets.SERVER switch
            {
                Server.Test =>
                    new TelegramBotClient(new TelegramBotClientOptions(Secrets.TEST_TOKEN,
                        useTestEnvironment: true)),
                Server.Production => new TelegramBotClient(Secrets.PRODUCTION_TOKEN),
                _ => throw new ArgumentOutOfRangeException()
            };


            var me = await bot.GetMe();

            Logger.Info("Bot connected as {me.Username} in {server} server.", $"@{me.Username}", Secrets.SERVER);

            commandRegistry = new CommandRegistry();
            callbackRegistry = new CallbackRegistry();

            bot.OnMessage += async (message, _) => { await OnMessage(message); };
            bot.OnUpdate += OnUpdate;

            AppDomain.CurrentDomain.ProcessExit += (_, _) => cts?.Cancel();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts?.Cancel();
            };

            try
            {
                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (TaskCanceledException)
            {
                Logger.Debug("Shutting down...");
            }
        }

        /// <summary>
        /// Telegram.Bot function. Listens to messages.
        /// </summary>
        /// <param name="message">Message.</param>
        private static async Task OnMessage(Message message)
        {
            try
            {
                await Database.UpdateUserActivity(message);
                if (message.Text == null)
                {
                    return;
                }

                if (message.Text!.StartsWith('/'))
                {
                    await commandRegistry?.HandleCommandAsync(message, bot!)!;
                }
            }
            catch (Exception? ex)
            {
                await OnError(ex, message.Chat.Id);
            }
        }

        /// <summary>
        /// Telegram.Bot function that gets bot updates, such as Callbacks, Telegram payments like Stars, etc.
        /// </summary>
        /// <param name="update">Update type</param>
        private static async Task OnUpdate(Update update)
        {
            switch (update)
            {
                case { CallbackQuery: not null }:
                    try
                    {
                        await callbackRegistry?.HandleCallbackAsync(update.CallbackQuery!, bot!)!;
                    }
                    catch (Exception? ex)
                    {
                        await OnError(ex, update.CallbackQuery!.Message!.Chat.Id);
                    }

                    break;
            }
        }

        /// <summary>
        /// Telegram.Bot function. If something goes wrong, it will be caught by this method.
        /// </summary>
        /// <param name="exception">takes an exception to get details from</param>
        /// <param name="chatId">used to send the error messages in chat where the error happened</param>
        private static async Task OnError(Exception? exception, long chatId)
        {
            if (exception is Exceptions.Message)
            {
                await bot!.SendMessage(chatId,
                    $"<b>Ah!</b> <i>Something bad happened...</i>\n" +
                    $"<blockquote expandable><i>{exception.Message}</i></blockquote>",
                    ParseMode.Html);
                return;
            }

            Logger.Fatal($"Something bad happened", exception);
            await bot!.SendMessage(chatId,
                $"<b>Ah!</b> <i>Something bad happened...</i> The problem has been reported to the devs automatically.\n" +
                $"<blockquote expandable><i>{exception?.Message}</i></blockquote>",
                ParseMode.Html);
        }
    }
}
