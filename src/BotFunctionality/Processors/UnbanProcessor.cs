using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Processors
{
    public class UnbanProcessor
    {
        public static async Task ProcessUnbanAsync(Message message, TelegramBotClient bot)
        {
            string?[]? args = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

            if (message.ReplyToMessage != null)
            {
                if (args?.Length > 0)
                {
                    throw new Exceptions.Message(
                        "When replying, provide nothing to unban.");
                }

                // try/catching to catch invalid id errors and send the exceptions as different exceptions that won't
                // make logs extremely trashy.
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await unban(message, replyMember, bot);
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
                    await unban(message, member, bot);
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

        private static async Task unban(Message message, ChatMember member, TelegramBotClient bot)
        {
            if (!member.IsAdmin)
            {
                await bot.UnbanChatMember(message.Chat.Id, member.User.Id, true);
                await Database.AddPunishment(message, PunishmentType.Unban);

                await bot.SendMessage(message.Chat.Id, $"User {member.User.Id} has been unbanned.");
            }
            else
            {
                throw new Exceptions.Message("Cannot unrestrict admin");
            }
        }
    }
}
