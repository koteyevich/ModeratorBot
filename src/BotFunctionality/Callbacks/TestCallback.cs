using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Callbacks
{
    public class TestCallback : CallbackBase
    {
        public override string Name => "TestCallback";

        public override string[] Aliases => ["tc"];

        protected override async Task ExecuteCoreAsync(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            await bot.AnswerCallbackQuery(callbackQuery.Id, "Test Callback!", showAlert: true);
        }
    }
}
