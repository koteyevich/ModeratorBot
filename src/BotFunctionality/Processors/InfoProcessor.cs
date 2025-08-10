using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class InfoProcessor
    {
        public static async Task ProcessInfoAsync(Message message, TelegramBotClient bot)
        {
            string?[] args = Parser.ParseArguments(message.Text!);

            if (message.ReplyToMessage != null)
            {
                // try/catching to catch invalid id errors and send the exceptions as different exceptions that won't
                // make logs extremely trashy.
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await info(message, replyMember, bot);
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
                    await info(message, member, bot);
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

        private static async Task info(Message message, ChatMember member, TelegramBotClient bot)
        {
            var user = await Database.GetUser(member.User.Id, message.Chat.Id);

            if (message.ReplyToMessage != null)
            {
                var keyboard = new InlineKeyboardMarkup();

                var revealButton = new InlineKeyboardButton("🔒", $"reveal_{user.UserId}_{message.Chat.Id}");
                var deleteButton = new InlineKeyboardButton("❌", "delete");

                keyboard.AddButtons(revealButton, deleteButton);

                await bot.SendMessage(message.Chat.Id, "👀 <i>Information hidden from curious eyes...</i>",
                    ParseMode.Html,
                    replyMarkup: keyboard);
            }
            else
            {
                var keyboard = new InlineKeyboardMarkup();

                var revealButton = new InlineKeyboardButton("🔒", $"reveal_{user.UserId}_{user.GroupId}");
                var deleteButton = new InlineKeyboardButton("❌", "delete");

                keyboard.AddButtons(revealButton, deleteButton);

                await bot.SendMessage(message.Chat.Id, "👀 <i>Information hidden from curious eyes...</i>",
                    ParseMode.Html,
                    replyMarkup: keyboard);
            }
        }
    }
}
