using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Processors
{
    public class KickProcessor
    {
        public static async Task ProcessKickAsync(Message message, TelegramBotClient bot)
        {
            string?[]? args = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

            if (message.ReplyToMessage != null)
            {
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await kick(message, replyMember, bot);
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
                    throw new Exceptions.Message("Please provide a valid user id");
                }

                try
                {
                    var member = await bot.GetChatMember(message.Chat.Id, userId);
                    await kick(message, member, bot);
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

        private static async Task kick(Message message, ChatMember member, TelegramBotClient bot)
        {
            if (!member.IsAdmin)
            {
                await bot.BanChatMember(message.Chat.Id, member.User.Id, DateTime.UtcNow.AddSeconds(30));
                await Database.AddPunishment(message, PunishmentType.Kick);

                await bot.SendMessage(message.Chat.Id, $"User {member.User.Id} has been kicked.");
                await bot.UnbanChatMember(message.Chat.Id, member.User.Id);
            }
        }
    }
}
