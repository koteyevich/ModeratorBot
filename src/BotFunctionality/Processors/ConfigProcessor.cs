using System.Text;
using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class ConfigProcessor
    {
        public static async Task ProcessConfigAsync(Message message, TelegramBotClient bot)
        {
            string?[] args = Parser.ParseArguments(message.Text!);
            var group = await Database.GetGroup(message);


            if (args.Length > 1)
            {
                await Database.UpdateGroupConfigSetting(message, args[0]!, args[1]!);
                await bot.SendMessage(message.Chat.Id,
                    $"<b>Got it!</b> The value of <code>{args[0]}</code> setting is now <b>{args[1]}</b>.",
                    ParseMode.Html);
            }
            else
            {
                var sb = new StringBuilder();

                sb.AppendLine($"<blockquote expandable>");
                foreach (ConfigEntry<object> setting in group.Config)
                {
                    sb.AppendLine(
                        $"<code>{setting.Name}</code> - <b>{setting.Value}</b> <i>(Default: <b>{setting.DefaultValue}</b>)</i>"
                    );
                }

                sb.AppendLine($"</blockquote>");

                sb.AppendLine("<b>To set a value to a setting, type /config [NAME] [VALUE]</b>");
                sb.AppendLine("<i>Example: /config WarnBanThreshold 5</i>");

                await bot.SendMessage(message.Chat.Id, sb.ToString(), ParseMode.Html);
            }
        }
    }
}
