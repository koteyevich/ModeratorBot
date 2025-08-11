using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Warn : CommandBase
    {
        public override string Name => "warn";

        public override string Description => "Warns a user.";

        public override string[] Aliases => ["w"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await WarnProcessor.ProcessWarnAsync(message, bot);
        }
    }
}
