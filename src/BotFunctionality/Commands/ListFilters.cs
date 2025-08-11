using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class ListFilters : CommandBase
    {
        public override string Name => "listfilters";

        public override string Description => "Lists all filters active";

        public override string[] Aliases => ["lf"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await ListFiltersProcessor.ProcessListFiltersAsync(message, bot);
        }
    }
}
