using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Processors
{
    public class BanProcessor
    {
        public static async Task ProcessBanAsync(Message message, TelegramBotClient bot)
        {
            string?[]? args = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

            if (message.ReplyToMessage != null)
            {
                if (args?.Length > 0 && long.TryParse(args[0], out _))
                {
                    throw new Exceptions.Message(
                        "When replying, provide only a duration (e.g., '1d12h30m') or no arguments for an infinite ban.");
                }

                // try/catching to catch invalid id errors and send the exceptions as different exceptions that won't
                // make logs extremely trashy.
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await ban(message, replyMember, args, 0, bot);
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
                if (args?.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new Exceptions.Message("Provide a valid user ID when not replying to a message.");
                }

                try
                {
                    var member = await bot.GetChatMember(message.Chat.Id, userId);
                    await ban(message, member, args, 1, bot);
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

        private static async Task ban(Message message, ChatMember member, string?[]? args, short dateIndex,
            TelegramBotClient bot)
        {
            if (!member.IsAdmin)
            {
                DateTime? duration = null;
                if (args?.Length > dateIndex && !string.IsNullOrEmpty(args[dateIndex]))
                {
                    duration = DateTime.UtcNow.AddSeconds(ConvertToSeconds.Convert(args[dateIndex]));
                }

                await bot.BanChatMember(message.Chat.Id, member.User.Id, untilDate: duration);
                await Database.AddPunishment(message, PunishmentType.Ban, duration);

                await bot.SendMessage(message.Chat.Id, $"User {member.User.Id} has been banned.\n" +
                                                       $"Until: {(duration != null ? duration?.ToString("g") : "FOREVER")}");
            }
            else
            {
                throw new Exceptions.Message("Cannot ban admin.");
            }
        }
    }
}
