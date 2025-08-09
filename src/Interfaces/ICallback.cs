using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.Interfaces
{
    public interface ICallback
    {
        string Name { get; }
        string[] Aliases { get; }
        bool IsAdminCallback { get; }

        Task ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient bot);
    }

    public abstract class CallbackBase : ICallback
    {
        public abstract string Name { get; }
        public abstract string[] Aliases { get; }
        public abstract bool IsAdminCallback { get; }

        public async Task ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            var me = await bot.GetChatMember(callbackQuery.Message!.Chat.Id, (await bot.GetMe()).Id);
            var member = await bot.GetChatMember(callbackQuery.Message.Chat.Id, callbackQuery.From.Id);

            if (me.IsAdmin)
            {
                if (member.IsAdmin)
                    await ExecuteCoreAsync(callbackQuery, bot);
                else
                {
                    await bot.AnswerCallbackQuery(callbackQuery.Id, "Hey! That's for admins to click!");
                }
            }
            else
            {
                throw new Exceptions.Message("I do not have admin permissions!!");
            }
        }

        protected abstract Task ExecuteCoreAsync(CallbackQuery callbackQuery, TelegramBotClient bot);
    }
}
