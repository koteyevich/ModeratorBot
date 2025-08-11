using System.Text;
using ModeratorBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class ListFiltersProcessor
    {
        public static async Task ProcessListFiltersAsync(Message message, TelegramBotClient bot)
        {
            var group = await Database.GetGroup(message);
            var sb = new StringBuilder();

            sb.AppendLine("List of all filters:");
            sb.Append("<blockquote expandable>");

            var mediaActions = new Dictionary<string, string>
            {
                { "photo", "🖼️" },
                { "video", "📽️" },
                { "audio", "🔊" },
                { "document", "📄" },
                { "gif", "gif" },
                { "videomessage", "video message" },
                { "voice", "🗣️" },
                { "sticker", "sticker" }
            };

            foreach (var filter in group.Filters)
            {
                string[] splitReply = filter.Reply.Split(':');
                string output;

                if (splitReply[0] == "media")
                {
                    if (!mediaActions.TryGetValue(splitReply[1], out string? mediaType))
                        throw new MessageException($"Unknown media type: {splitReply[1]}");
                    output = $"{filter.Trigger} - {mediaType}";
                }
                else
                {
                    output = $"{filter.Trigger} - {filter.Reply}";
                }

                sb.AppendLine(output);
            }

            sb.AppendLine("</blockquote>");

            await bot.SendMessage(
                message.Chat.Id,
                sb.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyParameters: new ReplyParameters { ChatId = message.Chat.Id, MessageId = message.Id });
        }
    }
}
