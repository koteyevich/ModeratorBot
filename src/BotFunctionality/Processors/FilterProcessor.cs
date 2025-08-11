using ModeratorBot.Exceptions;
using ModeratorBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ModeratorBot.BotFunctionality.Processors
{
    public static class FilterProcessor
    {
        public static async Task ProcessFilter(Message message, TelegramBotClient bot)
        {
            var group = await Database.GetGroup(message);

            foreach (var filter in group.Filters)
            {
                bool matches = filter.TriggerCondition switch
                {
                    Filter.TriggerType.Exact => message.Text!.Equals(filter.Trigger,
                        StringComparison.OrdinalIgnoreCase),
                    Filter.TriggerType.Contains => message.Text!.Contains(filter.Trigger,
                        StringComparison.OrdinalIgnoreCase),
                    _ => false
                };

                if (!matches) continue;

                string[] splitReply = filter.Reply.Split(':');
                //media : document : {caption} : {replyMsg.Document.FileId}

                // STINKY DOO-DOO CODE INCOMING
                if (splitReply[0] == "media")
                {
                    await bot.SendChatAction(message.Chat.Id, ChatAction.UploadDocument);

                    var mediaActions = new Dictionary<string, Func<Task>>
                    {
                        {
                            "photo",
                            async () => await bot.SendPhoto(message.Chat.Id, splitReply[3], splitReply[2],
                                replyParameters: new ReplyParameters
                                    { ChatId = message.Chat.Id, MessageId = message.Id })
                        },
                        {
                            "video",
                            async () => await bot.SendVideo(message.Chat.Id, splitReply[3], splitReply[2],
                                replyParameters: new ReplyParameters
                                    { ChatId = message.Chat.Id, MessageId = message.Id })
                        },
                        {
                            "audio",
                            async () => await bot.SendAudio(message.Chat.Id, splitReply[3], splitReply[2],
                                replyParameters: new ReplyParameters
                                    { ChatId = message.Chat.Id, MessageId = message.Id })
                        },
                        {
                            "document",
                            async () => await bot.SendDocument(message.Chat.Id, splitReply[3], splitReply[2],
                                replyParameters: new ReplyParameters
                                    { ChatId = message.Chat.Id, MessageId = message.Id })
                        },
                        {
                            "gif",
                            async () => await bot.SendAnimation(message.Chat.Id, splitReply[3], splitReply[2],
                                replyParameters: new ReplyParameters
                                    { ChatId = message.Chat.Id, MessageId = message.Id })
                        },
                        {
                            "videomessage",
                            async () => await bot.SendVideoNote(message.Chat.Id, splitReply[3],
                                replyParameters: new ReplyParameters
                                    { ChatId = message.Chat.Id, MessageId = message.Id })
                        },
                        {
                            "voice",
                            async () => await bot.SendVoice(message.Chat.Id, splitReply[3], splitReply[2],
                                replyParameters: new ReplyParameters
                                    { ChatId = message.Chat.Id, MessageId = message.Id })
                        },
                        {
                            "sticker",
                            async () => await bot.SendSticker(message.Chat.Id, splitReply[3],
                                replyParameters: new ReplyParameters
                                    { ChatId = message.Chat.Id, MessageId = message.Id })
                        }
                    };

                    if (!mediaActions.TryGetValue(splitReply[1], out var sendAction))
                        throw new MessageException($"Unsupported media type: {splitReply[1]}");

                    await sendAction();
                    break;
                }

                Logger.Debug("Trigger: {filterTrigger}, Reply: {filterReply}", filter.Trigger, filter.Reply);
                await bot.SendMessage(
                    chatId: message.Chat.Id,
                    text: filter.Reply,
                    replyParameters: new ReplyParameters { ChatId = message.Chat.Id, MessageId = message.Id }
                );
                break;
            }
        }
    }
}
