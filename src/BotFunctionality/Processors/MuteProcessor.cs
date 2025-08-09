using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Exceptions;
using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class MuteProcessor
    {
        public static async Task ProcessMuteAsync(Message message, TelegramBotClient bot)
        {
            string?[] args = Parser.ParseArguments(message.Text!);
            string? reason = Parser.ParseReason(message.Text!);

            if (message.ReplyToMessage != null)
            {
                if (args.Length > 0 && long.TryParse(args[0], out _))
                {
                    throw new MessageException(
                        "When replying, provide only a duration (e.g., '1d12h30m') or no arguments for an infinite mute.");
                }

                // try/catching to catch invalid id errors and send the exceptions as different exceptions that won't
                // make logs extremely trashy.
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await mute(message, replyMember, args, 0, reason, bot);
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
            else
            {
                if (args.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new MessageException("Provide a valid user ID when not replying to a message.");
                }

                try
                {
                    var member = await bot.GetChatMember(message.Chat.Id, userId);
                    await mute(message, member, args, 1, reason, bot);
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

        private static async Task mute(Message message, ChatMember member, string?[]? args, short dateIndex,
            string? reason, TelegramBotClient bot)
        {
            if (!member.IsAdmin)
            {
                DateTime? duration = null;
                if (args?.Length > dateIndex && !string.IsNullOrWhiteSpace(args[dateIndex]))
                {
                    long seconds = ConvertToSeconds.Convert(args[dateIndex]);
                    if (seconds == 0)
                    {
                        throw new MessageException("Invalid duration format. Use formats like '1d12h30m'.");
                    }

                    duration = DateTime.UtcNow.AddSeconds(seconds);
                }

                await bot.RestrictChatMember(message.Chat.Id, member.User.Id, new ChatPermissions(),
                    untilDate: duration);
                await Database.AddPunishment(message, PunishmentType.Mute, duration, reason);

                await bot.SendMessage(message.Chat.Id,
                    $"User <code>{member.User.Id}</code> has been <b>muted.</b>\n" +
                    $"<b>Until:</b> <i>{(duration != null ? duration.Value.ToString("G") : "FOREVER")}</i>\n" +
                    $"<b>Reason:</b> <i>{(string.IsNullOrEmpty(reason) ? "No reason provided" : reason)}</i>",
                    ParseMode.Html);
            }
            else
            {
                throw new MessageException("Cannot restrict admin.");
            }
        }
    }
}
