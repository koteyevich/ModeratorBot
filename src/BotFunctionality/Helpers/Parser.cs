using ModeratorBot.Models;

namespace ModeratorBot.BotFunctionality.Helpers
{
    public static class Parser
    {
        /// <summary>
        /// Parses the first line of the string, splits using spaces and skips the first word (because that should be the /[command])
        /// </summary>
        /// <param name="str">Original string</param>
        /// <returns>Arguments in an array</returns>
        public static string[] ParseArguments(string str)
        {
            return str.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)
                .ToArray();
        }

        /// <summary>
        /// Parses the second line (if it exists) of the original string
        /// </summary>
        /// <param name="str">Original string</param>
        /// <returns>If the second line exists - contents of that line. Else - null</returns>
        public static string? ParseReason(string str)
        {
            return str.Contains('\n')
                ? str[(str.IndexOf('\n') + 1)..].Trim()
                : null;
        }

        public static Filter.TriggerType ParseTriggerType(string str)
        {
            Enum.TryParse(str.ToLower(), true, out Filter.TriggerType triggerType);

            return triggerType;
        }
    }
}
