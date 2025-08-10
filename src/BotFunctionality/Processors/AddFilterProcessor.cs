using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class AddFilterProcessor
    {
        public static async Task ProcessFilterAsync(Message message, TelegramBotClient bot)
        {
            string[] args = Parser.ParseArguments(message.Text!);
            string? reply = Parser.ParseReason(message.Text!);

            if (args.Length > 1)
            {
                // this solution here covers one word triggers and multiple-word triggers
                string trigger = string.Join(" ", args.Skip(1));

                var parsedType = Parser.ParseTriggerType(args[0]);

                if (reply != null)
                {
                    await Database.AddFilter(message, parsedType, trigger, reply);
                }
                else
                {
                    throw new MessageException("Please provide a reply to the trigger.");
                }
            }
        }
    }
}
