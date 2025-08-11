using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class RemoveFilterProcessor
    {
        public static async Task ProcessRemoveFilterAsync(Message message, TelegramBotClient bot)
        {
            string[] args = Parser.ParseArguments(message.Text!);

            if (args.Length > 0)
            {
                await Database.RemoveFilter(message, string.Join(" ", args));
                await bot.SendMessage(message.Chat.Id, $"Removed filter \"{string.Join(" ", args)}\"");
            }
            else
            {
                throw new MessageException("Please provide a filter trigger to remove");
            }
        }
    }
}
