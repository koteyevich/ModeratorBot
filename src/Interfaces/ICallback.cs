using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.Interfaces
{
    public interface ICallback
    {
        string Name { get; }
        string[] Aliases { get; }

        Task ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient bot);
    }

    public abstract class CallbackBase : ICallback
    {
        public abstract string Name { get; }
        public abstract string[] Aliases { get; }

        public async Task ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            await ExecuteCoreAsync(callbackQuery, bot);
        }

        protected abstract Task ExecuteCoreAsync(CallbackQuery callbackQuery, TelegramBotClient bot);
    }
}
