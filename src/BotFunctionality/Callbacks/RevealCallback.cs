using System.Text;
using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ModeratorBot.BotFunctionality.Callbacks
{
    public class RevealCallback : CallbackBase
    {
        private const int punishments_per_page = 5;

        public override string Name => "reveal";
        public override string[] Aliases => [];

        public override bool IsAdminCallback => true;

        protected override async Task ExecuteCoreAsync(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            string?[]? args = callbackQuery.Data?.Split('_', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

            int page = 1;

            // if page is specified, set the page to it
            if (args?.Length >= 3 && int.TryParse(args[2], out int parsedPage))
            {
                page = parsedPage;
            }

            var user = await Database.GetUser(long.Parse(args?[0]!), long.Parse(args?[1]!));
            var group = await Database.GetGroup(callbackQuery.Message!);

            int warnBanThreshold = (int)group.Config.First(x => x.Name == "WarnBanThreshold").Value;

            var punishments = user.Punishments.ToList();
            int totalPages = (int)Math.Ceiling((double)punishments.Count / punishments_per_page);
            page = Math.Max(1, Math.Min(page, totalPages));

            var sb = new StringBuilder();

            sb.AppendLine($"<b>This is user <code>{user.UserId}</code>.</b>");
            sb.AppendLine($"<b>First seen:</b> {user.FirstSeen.ToString("G")}");
            sb.AppendLine($"<b>Last seen:</b> {user.LastSeen.ToString("G")}\n");

            sb.AppendLine($"<b>Message count:</b> {user.MessageCount}");
            sb.AppendLine($"<b>Warning count:</b> {user.WarningCount}/{warnBanThreshold}\n");

            sb.AppendLine($"<b>Punishments (Page {page}/{totalPages}): </b>");

            var punishmentsToShow = punishments
                .Skip((page - 1) * punishments_per_page)
                .Take(punishments_per_page)
                .ToList();

            if (punishmentsToShow.Count == 0)
            {
                sb.AppendLine("<i>No punishments found.</i>");
            }
            else
            {
                foreach (var punishment in punishmentsToShow)
                {
                    sb.AppendLine($"<blockquote>Type: {punishment.Type}");
                    sb.AppendLine($"Created at: {punishment.CreatedAt.ToString("G")}");
                    sb.AppendLine($"Moderator: {punishment.ModeratorId}");
                    if (punishment.Duration != null)
                    {
                        sb.AppendLine($"Duration: {punishment.Duration.Value.ToString("G")} UTC");
                    }

                    if (!string.IsNullOrEmpty(punishment.Reason))
                    {
                        sb.AppendLine($"Reason: {punishment.Reason}");
                    }

                    sb.AppendLine("</blockquote>");
                }
            }

            var buttons = new List<InlineKeyboardButton>();
            if (page > 1)
            {
                buttons.Add(
                    InlineKeyboardButton.WithCallbackData("◀️", $"reveal_{args?[0]!}_{args?[1]!}_{page - 1}"));
            }

            buttons.Add(InlineKeyboardButton.WithCallbackData($"📜 {page}/{totalPages}", "null"));
            if (page < totalPages)
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData("▶️", $"reveal_{args?[0]!}_{args?[1]!}_{page + 1}"));
            }

            var deleteButton = new InlineKeyboardButton("❌", "delete");
            var hideButton = new InlineKeyboardButton("🔓", $"hide_{user.UserId}_{user.GroupId}");

            var replyMarkup = buttons.Count != 0
                ? new InlineKeyboardMarkup(buttons).AddNewRow(hideButton, deleteButton)
                : null;

            Logger.Debug("chat id: {chatid}, message id: {msgid}, page: {page}",
                callbackQuery.Message?.Chat.Id, callbackQuery.Message?.MessageId, page);

            if (callbackQuery.Message != null)
            {
                await bot.EditMessageText(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    text: sb.ToString(),
                    replyMarkup: replyMarkup,
                    parseMode: ParseMode.Html);
            }
        }
    }
}
