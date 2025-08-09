namespace ModeratorBot.Exceptions
{
    /// <summary>
    /// Error that shows to user, but is never reported to the developer.
    /// </summary>
    /// <param name="message">Error message</param>
    public class MessageException(string message) : Exception(message);
}
