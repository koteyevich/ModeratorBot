using ModeratorBot.Exceptions;

namespace ModeratorBot.BotFunctionality.Helpers
{
    public static class ConfigValidator
    {
        private static readonly Dictionary<string, Type> config_types = new()
        {
            { "WarnBanThreshold", typeof(int) },
        };

        public static object ValidateAndConvert(string name, string inputValue)
        {
            if (!config_types.TryGetValue(name, out var expectedType))
            {
                throw new Message($"Unknown config key: {name}");
            }

            if (expectedType == typeof(int))
            {
                if (int.TryParse(inputValue, out int intValue))
                    return intValue;
                throw new Message($"Value for {name} must be a valid integer.");
            }

            if (expectedType == typeof(string))
            {
                return inputValue; // Strings are already correct
            }

            if (expectedType == typeof(bool))
            {
                if (bool.TryParse(inputValue, out bool boolValue))
                    return boolValue;
                throw new Message($"Value for {name} must be a valid boolean (true/false).");
            }

            throw new Message($"Unsupported type for {name}: {expectedType}");
        }
    }
}
