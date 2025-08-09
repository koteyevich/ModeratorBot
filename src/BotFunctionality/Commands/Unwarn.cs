using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Unwarn : CommandBase
    {
        public override string Name => "unwarn";
        public override string Description => "Removes one warning from the user.";
        public override string[] Aliases { get; } = ["uw"];
        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await UnwarnProcessor.ProcessUnwarnAsync(message, bot);
        }
    }
}
