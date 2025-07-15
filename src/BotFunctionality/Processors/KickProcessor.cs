using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class KickProcessor
    {
        public static async Task ProcessKickAsync(Message message, TelegramBotClient bot)
        {
            // arguments. split by spaces. skips "/kick".
            string?[]? args = message.Text?.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)
                .ToArray();

            // reason for the kick. parses new line.
            string? reason = message.Text?.Contains('\n') == true
                ? message.Text[(message.Text.IndexOf('\n') + 1)..].Trim()
                : null;
            if (string.IsNullOrWhiteSpace(reason)) reason = null;

            if (message.ReplyToMessage != null)
            {
                // try/catching to catch invalid id errors and send the exceptions as different exceptions that won't
                // make logs extremely trashy.
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await kick(message, replyMember, reason, bot);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("PARTICIPANT_ID_INVALID"))
                    {
                        throw new Exceptions.Message(e.Message);
                    }

                    throw;
                }
            }
            else
            {
                if (args?.Length == 0 || string.IsNullOrEmpty(args?[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new Exceptions.Message("Please provide a valid user id");
                }

                try
                {
                    var member = await bot.GetChatMember(message.Chat.Id, userId);
                    await kick(message, member, reason, bot);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("PARTICIPANT_ID_INVALID"))
                    {
                        throw new Exceptions.Message(e.Message);
                    }
                }
            }
        }

        private static async Task kick(Message message, ChatMember member, string? reason, TelegramBotClient bot)
        {
            if (!member.IsAdmin)
            {
                await bot.BanChatMember(message.Chat.Id, member.User.Id, DateTime.UtcNow.AddSeconds(30));
                await Database.AddPunishment(message, PunishmentType.Kick, reason: reason);

                await bot.SendMessage(message.Chat.Id,
                    $"User <code>{member.User.Id}</code> has been <b>kicked.</b>\n" +
                    $"<b>Reason:</b> <i>{(string.IsNullOrEmpty(reason) ? "No reason provided" : reason)}</i>",
                    ParseMode.Html);
                await bot.UnbanChatMember(message.Chat.Id, member.User.Id);
            }
            else
            {
                throw new Exceptions.Message("Cannot kick admin.");
            }
        }
    }
}
