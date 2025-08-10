using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Filter : CommandBase
    {
        public override string Name => "filter";

        public override string Description => "Add a filter.";

        public override string[] Aliases => ["af"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await AddFilterProcessor.ProcessFilterAsync(message, bot);
        }
    }
}
