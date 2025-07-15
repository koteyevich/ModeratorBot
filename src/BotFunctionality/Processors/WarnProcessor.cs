using Telegram.Bot;
using Telegram.Bot.Types;

using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class WarnProcessor
    {
        public static async Task ProcessWarnAsync(Message message, TelegramBotClient bot)
        {
            string?[]? args = message.Text?.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)
                .ToArray();
            string? reason = message.Text?.Contains('\n') == true
                ? message.Text[(message.Text.IndexOf('\n') + 1)..].Trim()
                : null;
            if (string.IsNullOrWhiteSpace(reason)) reason = null;

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
                        throw new Exceptions.Message(e.Message);
                    }

                    throw;
                }
            }
            else
            {
                if (args?.Length == 0 || string.IsNullOrEmpty(args?[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new Exceptions.Message("Provide a valid user ID when not replying to a message.");
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
                        throw new Exceptions.Message(e.Message);
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
                    throw new Exceptions.Message("I can't do it to my kind... (Warning bots is unallowed)");
                }

                var user = await Database.GetUser(member.User.Id, message.Chat.Id);
                await Database.AddWarning(message, reason);

                await bot.SendMessage(message.Chat.Id,
                    $"User <code>{user.UserId}</code> has been <b>warned.</b>\n" +
                    $"<b>Warnings:</b> <i>{user.WarningCount + 1}/{Database.MAX_WARNS}</i>\n" +
                    $"<b>Reason:</b> <i>{(string.IsNullOrEmpty(reason) ? "No reason provided" : reason)}</i>",
                    ParseMode.Html);

                if (user.WarningCount + 1 >= Database.MAX_WARNS)
                {
                    await BanProcessor.ProcessBanAsync(message, bot);
                }
            }
            else
            {
                throw new Exceptions.Message("Cannot warn admin.");
            }
        }
    }
}
