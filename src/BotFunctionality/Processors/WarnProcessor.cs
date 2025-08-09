using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class WarnProcessor
    {
        public static async Task ProcessWarnAsync(Message message, TelegramBotClient bot)
        {
            string?[] args = Parser.ParseArguments(message.Text!);
            string? reason = Parser.ParseReason(message.Text!);

            if (message.ReplyToMessage != null)
            {
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await warn(message, replyMember, reason, bot);
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
                    await warn(message, member, reason, bot);
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

        private static async Task warn(Message message, ChatMember member, string? reason, TelegramBotClient bot)
        {
            if (!member.IsAdmin)
            {
                if (member.User.IsBot)
                {
                    throw new MessageException("I can't do it to my kind... (Warning bots is unallowed)");
                }

                var user = await Database.GetUser(member.User.Id, message.Chat.Id);
                var group = await Database.GetGroup(message);

                int warnBanThreshold = (int)group.Config.First(x => x.Name == "WarnBanThreshold").Value;

                await Database.AddWarning(message, reason);

                await bot.SendMessage(message.Chat.Id,
                    $"User <code>{user.UserId}</code> has been <b>warned.</b>\n" +
                    $"<b>Warnings:</b> <i>{user.WarningCount + 1}/{warnBanThreshold}</i>\n" +
                    $"<b>Reason:</b> <i>{(string.IsNullOrEmpty(reason) ? "No reason provided" : reason)}</i>",
                    ParseMode.Html);

                if (user.WarningCount + 1 >= warnBanThreshold)
                {
                    await BanProcessor.ProcessBanAsync(message, bot);
                }
            }
            else
            {
                throw new MessageException("Cannot warn admin.");
            }
        }
    }
}
