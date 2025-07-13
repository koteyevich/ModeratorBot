using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Start : CommandBase
    {
        public override string Name => "start";
        public override string Description => "Starts the bot.";

        public override string[] Aliases => [];

        public override bool IsAdminCommand => false;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await StartProcessor.ProcessStartAsync(message, bot);
        }
    }
}
