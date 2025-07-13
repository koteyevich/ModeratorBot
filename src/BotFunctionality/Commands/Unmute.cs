using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Unmute : CommandBase
    {
        public override string Name => "unmute";

        public override string Description => "Unmutes a user";

        public override string[] Aliases => ["um"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await UnmuteProcessor.ProcessUnmuteAsync(message, bot);
        }
    }
}
