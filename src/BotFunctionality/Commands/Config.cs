using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Config : CommandBase
    {
        public override string Name => "config";

        public override string Description => "Configure the bot to your liking!";

        public override string[] Aliases => [];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await ConfigProcessor.ProcessConfigAsync(message, bot);
        }
    }
}
