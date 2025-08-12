using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

                if (message.ReplyToMessage != null)
                {
                    var replyMsg = message.ReplyToMessage;
                    var mediaTypes =
                        new Dictionary<MessageType, (string Type, long? MaxSize, Func<string, string> GetReply)>
                        {
                            {
                                MessageType.Text,
                                ("text", null, (_) => replyMsg.Text ?? throw new MessageException("Empty text message"))
                            },
                            {
                                MessageType.Animation,
                                ("gif", 17825792, (caption) => $"media:gif:{caption}:{replyMsg.Animation?.FileId}")
                            },
                            {
                                MessageType.Audio,
                                ("audio", 52428800, (caption) => $"media:audio:{caption}:{replyMsg.Audio?.FileId}")
                            },
                            {
                                MessageType.VideoNote,
                                ("videomessage", null,
                                    (caption) => $"media:videomessage:{caption}:{replyMsg.VideoNote?.FileId}")
                            },
                            {
                                MessageType.Voice,
                                ("voice", null, (caption) => $"media:voice:{caption}:{replyMsg.Voice?.FileId}")
                            },
                            {
                                MessageType.Sticker,
                                ("sticker", null, (caption) => $"media:sticker:{caption}:{replyMsg.Sticker?.FileId}")
                            },
                            {
                                MessageType.Document,
                                ("document", 52428800,
                                    (caption) => $"media:document:{caption}:{replyMsg.Document?.FileId}")
                            },
                            {
                                MessageType.Video,
                                ("video", 52428800, (caption) => $"media:video:{caption}:{replyMsg.Video?.FileId}")
                            },
                            {
                                MessageType.Photo,
                                ("photo", 10485760, (caption) => $"media:photo:{caption}:{replyMsg.Photo?[^1].FileId}")
                            }
                        };

                    if (mediaTypes.TryGetValue(replyMsg.Type, out var media))
                    {
                        if (media.MaxSize.HasValue)
                        {
                            long? fileSize = replyMsg.Type switch
                            {
                                MessageType.Animation => replyMsg.Animation?.FileSize,
                                MessageType.Audio => replyMsg.Audio?.FileSize,
                                MessageType.Document => replyMsg.Document?.FileSize,
                                MessageType.Video => replyMsg.Video?.FileSize,
                                MessageType.Photo => replyMsg.Photo?[^1].FileSize,
                                _ => 0
                            };
                            if (fileSize >= media.MaxSize.Value)
                                throw new MessageException(
                                    $"{media.Type} too big! (>= {media.MaxSize.Value / 1024 / 1024}MB)");
                        }

                        reply = media.GetReply(replyMsg.Caption!);
                    }
                    else
                    {
                        throw new MessageException("Filters do not support this type of media. Try photo or a gif!");
                    }
                }

                if (reply != null)
                {
                    await Database.AddFilter(message, parsedType, trigger, reply);
                    await bot.SendMessage(message.Chat.Id,
                        $"Done! Filter with the trigger '{trigger}' has been added.");
                }
                else
                {
                    throw new MessageException("Please provide a reply to the trigger.");
                }
            }
        }
    }
}
