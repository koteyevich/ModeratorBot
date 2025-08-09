namespace ModeratorBot.BotFunctionality.Helpers
{
    public static class ConvertToSeconds
    {
        public static long Convert(string? timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
                return 0;

            timeString = timeString.ToLower();
            long totalSeconds = 0;
            string number = "";

            foreach (char c in timeString)
            {
                if (char.IsDigit(c))
                {
                    number += c;
                }
                else if (c is 'd' or 'h' or 'm' or 's')
                {
                    if (long.TryParse(number, out long value))
                    {
                        totalSeconds += c switch
                        {
                            'd' => value * 24 * 60 * 60, // days
                            'h' => value * 60 * 60, // hours
                            'm' => value * 60, // minutes
                            's' => value, // seconds
                            _ => 0
                        };
                    }

                    number = "";
                }
            }

            return totalSeconds;
        }
    }
}
