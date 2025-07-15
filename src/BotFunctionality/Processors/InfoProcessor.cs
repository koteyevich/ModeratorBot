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
            string?[]? args = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            if (message.ReplyToMessage != null)
            {
                try
                {
                    var replyMember = await bot.GetChatMember(message.Chat.Id, message.ReplyToMessage.From!.Id);
                    await info(message, replyMember, bot);
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
                    await info(message, member, bot);
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

        private static async Task info(Message message, ChatMember member, TelegramBotClient bot)
        {
            if (message.ReplyToMessage != null)
            {
                var user = await Database.GetUser(message);

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
                var user = await Database.GetUser(member.User.Id, message.Chat.Id);

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
