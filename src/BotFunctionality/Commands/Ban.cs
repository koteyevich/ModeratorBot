using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Ban : CommandBase
    {
        public override string Name => "ban";

        public override string Description => "Bans a user";

        public override string[] Aliases => ["b"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await BanProcessor.ProcessBanAsync(message, bot);
        }
    }
}
