using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Info : CommandBase
    {
        public override string Name => "info";

        public override string Description => "Gets info about the user.";

        public override string[] Aliases => ["i"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await InfoProcessor.ProcessInfoAsync(message, bot);
        }
    }
}
