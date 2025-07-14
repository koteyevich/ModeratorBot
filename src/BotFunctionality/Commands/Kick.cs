using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Kick : CommandBase
    {
        public override string Name => "kick";

        public override string Description => "Kicks a user";

        public override string[] Aliases => ["k"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await KickProcessor.ProcessKickAsync(message, bot);
        }
    }
}
