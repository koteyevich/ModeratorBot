using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class UnbanProcessor
    {
        public static async Task ProcessUnbanAsync(Message message, TelegramBotClient bot)
        {
            string?[] args = Parser.ParseArguments(message.Text!);
            string? reason = Parser.ParseReason(message.Text!);

            if (message.ReplyToMessage != null)
            {
                // try/catching to catch invalid id errors and send the exceptions as different exceptions that won't
                // make logs extremely trashy.
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await unban(message, replyMember, reason, bot);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("PARTICIPANT_ID_INVALID"))
                    {
                        throw new Exceptions.Message(e.Message);
                    }
                }
            }
            else
            {
                if (args.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new Exceptions.Message("Provide a valid user ID when not replying to a message.");
                }

                try
                {
                    var member = await bot.GetChatMember(message.Chat.Id, userId);
                    await unban(message, member, reason, bot);
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
        }

        private static async Task unban(Message message, ChatMember member, string? reason, TelegramBotClient bot)
        {
            if (!member.IsAdmin)
            {
                await bot.UnbanChatMember(message.Chat.Id, member.User.Id, true);
                await Database.AddPunishment(message, PunishmentType.Unban, reason: reason);

                await bot.SendMessage(message.Chat.Id,
                    $"User <code>{member.User.Id}</code> has been <b>unbanned.</b>\n" +
                    $"<b>Reason:</b> {(string.IsNullOrEmpty(reason) ? "No reason provided" : reason)}", ParseMode.Html);
            }
            else
            {
                throw new Exceptions.Message("Cannot unrestrict admin");
            }
        }
    }
}
