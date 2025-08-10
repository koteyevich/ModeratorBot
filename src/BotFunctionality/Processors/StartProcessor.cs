using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class StartProcessor
    {
        public static async Task ProcessStartAsync(Message message, TelegramBotClient bot)
        {
            // string[]? args = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // example usage of the args variable
            // if (args is { Length: > 1 })
            // {
            //     if (args[1].StartsWith("koteyevich"))
            //     {
            //         Logger.Log("meow!");
            //     }
            //
            //     return;
            // }

            await bot.SendMessage(
                chatId: message.Chat.Id,
                text: "👋 Hello! This is a bot for moderation!\n",
                parseMode: ParseMode.Html,
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true }
            );
        }
    }
}
