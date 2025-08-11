using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class RemoveFilter : CommandBase
    {
        public override string Name => "removefilter";

        public override string Description => "Removes a filter";

        public override string[] Aliases => ["rf"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await RemoveFilterProcessor.ProcessRemoveFilterAsync(message, bot);
        }
    }
}
