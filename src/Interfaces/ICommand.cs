using ModeratorBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.Interfaces
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string[] Aliases { get; }
        bool IsAdminCommand { get; }
        Task ExecuteAsync(Message message, TelegramBotClient bot);
    }

    public abstract class CommandBase : ICommand
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string[] Aliases { get; }
        public abstract bool IsAdminCommand { get; }

        public async Task ExecuteAsync(Message message, TelegramBotClient bot)
        {
            if (IsAdminCommand)
            {
                var me = await bot.GetChatMember(message.Chat.Id, (await bot.GetMe()).Id);
                var member = await bot.GetChatMember(message.Chat.Id, message.From!.Id);

                if (me.IsAdmin)
                {
                    if (member.IsAdmin)
                        await ExecuteCoreAsync(message, bot);
                    else
                        await bot.DeleteMessage(message.Chat.Id, message.Id);
                }
                else
                {
                    throw new MessageException("I do not have admin permissions!!");
                }
            }
            else
            {
                await ExecuteCoreAsync(message, bot);
            }
        }

        protected abstract Task ExecuteCoreAsync(Message message, TelegramBotClient bot);
    }
}
