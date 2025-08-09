using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Exceptions;
using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class UnwarnProcessor
    {
        public static async Task ProcessUnwarnAsync(Message message, TelegramBotClient bot)
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
                    await unwarn(message, replyMember, reason, bot);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("PARTICIPANT_ID_INVALID"))
                    {
                        throw new MessageException(e.Message);
                    }
                }
            }
            else
            {
                if (args.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new MessageException("Provide a valid user ID when not replying to a message.");
                }

                try
                {
                    var member = await bot.GetChatMember(message.Chat.Id, userId);
                    await unwarn(message, member, reason, bot);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("PARTICIPANT_ID_INVALID"))
                    {
                        throw new MessageException(e.Message);
                    }

                    throw;
                }
            }
        }

        private static async Task unwarn(Message message, ChatMember member, string? reason, TelegramBotClient bot)
        {
            if (!member.IsAdmin)
            {
                var user = await Database.GetUser(member.User.Id, message.Chat.Id);
                if (user.WarningCount <= 0)
                {
                    await bot.SendMessage(message.Chat.Id, "This user has no warnings.");
                    return;
                }

                await Database.AddUnwarning(message, reason: reason);

                await bot.SendMessage(message.Chat.Id,
                    $"User <code>{member.User.Id}</code> has been <b>unwarned.</b>\n" +
                    $"<b>Reason:</b> <i>{(string.IsNullOrEmpty(reason) ? "No reason provided" : reason)}</i>",
                    ParseMode.Html);
            }
            else
            {
                throw new MessageException("Cannot unrestrict admin.");
            }
        }
    }
}
