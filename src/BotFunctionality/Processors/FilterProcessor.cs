using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class FilterProcessor
    {
        public static async Task ProcessFilter(Message message, TelegramBotClient bot)
        {
            var group = await Database.GetGroup(message);

            foreach (var filter in group.Filters)
            {
                bool matches = filter.TriggerCondition switch
                {
                    Filter.TriggerType.Exact => message.Text!.Equals(filter.Trigger,
                        StringComparison.OrdinalIgnoreCase),
                    Filter.TriggerType.Contains => message.Text!.Contains(filter.Trigger,
                        StringComparison.OrdinalIgnoreCase),
                    _ => false
                };

                if (!matches) continue;

                Logger.Debug("Trigger: {filterTrigger}, Reply: {filterReply}", filter.Trigger, filter.Reply);
                await bot.SendMessage(
                    chatId: message.Chat.Id,
                    text: filter.Reply,
                    replyParameters: new ReplyParameters { ChatId = message.Chat.Id, MessageId = message.Id }
                );
                break;
            }
        }
    }
}
