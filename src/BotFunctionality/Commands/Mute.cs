using ModeratorBot.BotFunctionality.Processors;
using ModeratorBot.Interfaces;
using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class Mute : CommandBase
    {
        public override string Name => "mute";

        public override string Description => "mutes a user";

        public override string[] Aliases => ["m"];

        public override bool IsAdminCommand => true;

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await MuteProcessor.ProcessMuteAsync(message, bot);
        }
    }
}
