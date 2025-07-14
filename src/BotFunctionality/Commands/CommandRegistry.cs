using ModeratorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ModeratorBot.BotFunctionality.Commands
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, ICommand> commands = new();

        public CommandRegistry()
        {
            var commandList = new List<ICommand>
            {
                new Start(),
                new Mute(),
                new Unmute(),
                new Ban(),
                new Unban(),
                new Kick()
            };

            foreach (var command in commandList)
            {
                string commandName = "/" + command.Name.ToLower();
                commands[commandName] = command;

                foreach (string alias in command.Aliases)
                {
                    commands["/" + alias.ToLower()] = command;
                }
            }
        }

        public async Task HandleCommandAsync(Message message, TelegramBotClient bot)
        {
            if (string.IsNullOrEmpty(message.Text))
                return;

            string commandText = message.Text.Split(' ')[0].ToLower();

            string normalizedCommand = commandText.StartsWith('/') ? commandText : "/" + commandText;
            normalizedCommand = normalizedCommand.Split('@')[0];

            var matchingCommand = commands
                .FirstOrDefault(kvp => normalizedCommand.StartsWith(kvp.Key));

            if (matchingCommand.Value != null)
            {
                Logger.Debug("Handling command: {CommandText}", commandText);
                await matchingCommand.Value.ExecuteAsync(message, bot);
            }
            else if (normalizedCommand.StartsWith("/help"))
            {
                await sendHelpMessage(message, bot);
            }
        }

        private async Task sendHelpMessage(Message message, TelegramBotClient bot)
        {
            string helpMessage = "<b>Available commands:</b>\n\n";
            var listed = new HashSet<ICommand>();

            foreach (var command in commands.Values.Distinct())
            {
                if (!listed.Add(command))
                    continue;

                string aliasText = command.Aliases.Length > 0
                    ? $" ({string.Join(", ", command.Aliases.Select(a => $"/{a}"))})"
                    : "";

                helpMessage += $"/{command.Name}{aliasText} - {command.Description}\n";
            }

            helpMessage += "\n";
            helpMessage += "<i>To do admin stuff it is recommended to reply to a message of a rulebreaker.</i>";

            await bot.SendMessage(message.Chat.Id, helpMessage, Telegram.Bot.Types.Enums.ParseMode.Html,
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
        }

        public IReadOnlyDictionary<string, ICommand> Commands => commands.AsReadOnly();
    }
}
