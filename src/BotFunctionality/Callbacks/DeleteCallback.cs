using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Callbacks
{
    public class DeleteCallback : CallbackBase
    {
        public override string Name => "delete";

        public override string[] Aliases => [];

        protected override async Task ExecuteCoreAsync(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            await bot.DeleteMessage(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId);
        }
    }
}
