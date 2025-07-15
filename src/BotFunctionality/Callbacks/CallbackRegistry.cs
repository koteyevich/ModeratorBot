using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Callbacks
{
    public class CallbackRegistry
    {
        private readonly Dictionary<string, ICallback> callbacks = new();

        public CallbackRegistry()
        {
            var callbackList = new List<ICallback>
            {
                new DeleteCallback(),
                new RevealCallback(),
                new HideCallback(),
            };

            foreach (var cb in callbackList)
            {
                callbacks[cb.Name.ToLower()] = cb;
                foreach (string alias in cb.Aliases)
                {
                    callbacks[alias.ToLower()] = cb;
                }
            }
        }

        /// <summary>
        /// Tries to match with callbacks from callbackList.
        /// <c>null</c> callback is used for buttons that display text, but have no function.
        /// </summary>
        /// <param name="query">Used to get the query data</param>
        /// <param name="bot">Used to answer the queries</param>
        public async Task HandleCallbackAsync(CallbackQuery query, TelegramBotClient bot)
        {
            if (string.IsNullOrWhiteSpace(query.Data)) return;

            if (query.Data == "null")
            {
                await bot.AnswerCallbackQuery(query.Id, "...");
                return;
            }

            string key = query.Data.Split('_')[0].ToLower();

            if (callbacks.TryGetValue(key, out var handler))
            {
                Logger.Debug("Handling callback: {query.Data}", query.Data);
                await handler.ExecuteAsync(query, bot);
                await bot.AnswerCallbackQuery(query.Id);
            }
            else
            {
                await bot.AnswerCallbackQuery(query.Id, "Unknown button...");
            }
        }
    }
}
