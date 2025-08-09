using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class ConfigProcessor
    {
        public static async Task ProcessConfigAsync(Message message, TelegramBotClient bot)
        {
            string?[] args = Parser.ParseArguments(message.Text!);
            var group = await Database.GetGroup(message);


            if (args.Length > 0)
            {
                foreach (ConfigEntry<object> setting in group.Config)
                {
                    Logger.Debug($"{setting.Name} - {setting.Value}");
                }
            }
        }
    }
}
