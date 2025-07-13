using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Unban : CommandBase
    {
        public override string Name => "unban";

        public override string Description => "Unbans a user";

        public override string[] Aliases => ["ub"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await UnbanProcessor.ProcessUnbanAsync(message, bot);
        }
    }
}
