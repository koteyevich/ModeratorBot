using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ModeratorBot.BotFunctionality.Callbacks
{
    public class HideCallback : CallbackBase
    {
        public override string Name => "hide";

        public override string[] Aliases => [];

        protected override async Task ExecuteCoreAsync(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            string?[]? args = callbackQuery.Data?.Split('_', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

            var user = await Database.GetUser(long.Parse(args?[0]!), long.Parse(args?[1]!));

            var buttons = new List<InlineKeyboardButton>
            {
                new("🔒", $"reveal_{user.UserId}_{user.GroupId}"),
                new("❌", "delete")
            };

            var replyMarkup = buttons.Count != 0 ? new InlineKeyboardMarkup(buttons) : null;

            if (callbackQuery.Message != null)
            {
                await bot.EditMessageText(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    text: "👀 <i>Information hidden from curious eyes...</i>",
                    replyMarkup: replyMarkup,
                    parseMode: ParseMode.Html);
            }
        }
    }
}
