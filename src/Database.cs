using System.Net;
using ModeratorBot.BotFunctionality.Helpers;
using ModeratorBot.Exceptions;
using ModeratorBot.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Telegram.Bot.Types;

namespace ModeratorBot
{
    public static class Database
    {
        //* the brain of the bot

        // Constants
        private static readonly IPAddress? ip = new(new byte[] { 192, 168, 222, 222 });
        private const int port = 27017;
        private const string database_name = "moderatorBot";

        // MongoDB client and collections
        private static readonly MongoClient client;
        private static readonly IMongoDatabase mongo_database;

        private static readonly IMongoCollection<UserModel> user_collection;
        private static readonly IMongoCollection<GroupModel> group_collection;

        static Database()
        {
            int retryCount = 0;

            while (true)
            {
                try
                {
                    string connectionString =
                        $"mongodb://{Secrets.DB_USERNAME}:{Secrets.DB_PASSWORD}@{ip}:{port}";
                    Logger.Debug(
                        "Attempting connection to MongoDB with ip {Host} and port {Port}. User: {User} (Attempt {Attempt})",
                        ip, port, Secrets.DB_USERNAME, retryCount + 1);

                    var settings = MongoClientSettings.FromConnectionString(connectionString);
                    client = new MongoClient(settings);
                    mongo_database = client.GetDatabase(database_name);

                    // Validate connection with a ping
                    mongo_database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();

                    user_collection = mongo_database.GetCollection<UserModel>("Users");
                    group_collection = mongo_database.GetCollection<GroupModel>("Groups");

                    Logger.Info("MongoDB connection successful after {Attempts} attempts.",
                        retryCount + 1);
                    break;
                }
                catch (Exception)
                {
                    retryCount++;
                    Logger.Error(
                        "Failed to connect to MongoDB on attempt {Attempt}.", retryCount);
                }
            }
        }

        /// <summary>
        /// Gets a user by his id and current group id using an original message
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <returns></returns>
        public static async Task<UserModel> GetUser(Message message)
        {
            var user = await user_collection.Find(u => u.UserId == message.From!.Id && u.GroupId == message.Chat.Id)
                .FirstOrDefaultAsync();

            return user ?? await createUser(message);
        }

        /// <summary>
        /// Gets a user by his id and current group id
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="chatId">group id</param>
        /// <returns></returns>
        public static async Task<UserModel> GetUser(long userId, long chatId)
        {
            var user = await user_collection.Find(u => u.UserId == userId && u.GroupId == chatId).FirstOrDefaultAsync();

            return user;
        }

        private static async Task<UserModel> createUser(Message message)
        {
            var user = new UserModel
            {
                UserId = message.From!.Id,
                GroupId = message.Chat.Id,
                Username = message.From.Username
            };

            await user_collection.InsertOneAsync(user);
            return user;
        }

        public static async Task<GroupModel> GetGroup(Message message)
        {
            var group = await group_collection.Find(g => g.GroupId == message.Chat.Id)
                .FirstOrDefaultAsync();

            return group ?? await createGroup(message);
        }

        private static async Task<GroupModel> createGroup(Message message)
        {
            var group = new GroupModel
            {
                GroupId = message.Chat.Id,
            };

            await group_collection.InsertOneAsync(group);
            return group;
        }

        /// <summary>
        /// Adds a punishment of specified type.
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <param name="punishmentType">Punishment type enum</param>
        /// <param name="duration">Optional. Duration of the punishment.</param>
        /// <param name="reason">Optional. Reason for the punishment.</param>
        public static async Task AddPunishment(Message message, PunishmentType punishmentType,
            DateTime? duration = null, string? reason = null)
        {
            var punishment = new PunishmentModel
            {
                ModeratorId = message.From!.Id,
                ModeratorUsername = message.From.Username,
                Type = punishmentType,
                Duration = duration,
                Reason = reason
            };

            if (message.ReplyToMessage != null)
            {
                var user = await GetUser(message.ReplyToMessage);

                await user_collection.UpdateOneAsync(
                    u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.AddToSet(u => u.Punishments, punishment));

                Logger.Debug(
                    "Added punishment type {PunishmentType} to user {UserId} in chat {ChatId}. Until: {Duration}. Reason: {Reason}",
                    punishment.Type, user.UserId, message.Chat.Id, duration?.ToString("G") ?? "FOREVER",
                    reason);
            }
            else
            {
                string?[] args = Parser.ParseArguments(message.Text!);
                if (args.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new MessageException("Provide a valid user ID when not replying to a message.");
                }

                var user = await GetUser(userId, message.Chat.Id);

                await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && user.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.AddToSet(u => u.Punishments, punishment));

                Logger.Debug(
                    "Added punishment type {PunishmentType} to user {UserId} in chat {ChatId}. Until: {Duration}. Reason: {Reason}",
                    punishment.Type, user.UserId, message.Chat.Id, duration?.ToString("G") ?? "FOREVER",
                    reason);
            }
        }

        /// <summary>
        /// Adds a warning to a user.
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <param name="reason">Optional. Reason for the warning</param>
        public static async Task AddWarning(Message message, string? reason)
        {
            if (message.ReplyToMessage != null)
            {
                var user = await GetUser(message.ReplyToMessage);

                await AddPunishment(message, PunishmentType.Warning, reason: reason);
                await user_collection.UpdateOneAsync(
                    u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.Inc(u => u.WarningCount, 1));
            }
            else
            {
                string?[] args = Parser.ParseArguments(message.Text!);
                if (args.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new MessageException("Provide a valid user ID when not replying to a message.");
                }

                var user = await GetUser(userId, message.Chat.Id);

                await AddPunishment(message, PunishmentType.Warning, reason: reason);
                await user_collection.UpdateOneAsync(
                    u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.Inc(u => u.WarningCount, 1));
            }
        }

        /// <summary>
        /// Removes a warning from the user.
        /// </summary>
        /// <param name="message">Original message</param>
        /// <param name="reason">Optional. Reason for the warning removal</param>
        public static async Task AddUnwarning(Message message, string? reason)
        {
            if (message.ReplyToMessage != null)
            {
                var user = await GetUser(message.ReplyToMessage);

                await AddPunishment(message, PunishmentType.Unwarning, reason: reason);
                await user_collection.UpdateOneAsync(
                    u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.Inc(u => u.WarningCount, -1));
            }
            else
            {
                string?[] args = Parser.ParseArguments(message.Text!);
                if (args.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new MessageException("Provide a valid user ID when not replying to a message.");
                }

                var user = await GetUser(userId, message.Chat.Id);

                await AddPunishment(message, PunishmentType.Unwarning, reason: reason);
                await user_collection.UpdateOneAsync(
                    u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.Inc(u => u.WarningCount, -1));
            }
        }

        /// <summary>
        /// Resets warnings of a user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="chatId">Chat id</param>
        public static async Task ResetWarnings(long userId, long chatId)
        {
            var user = await GetUser(userId, chatId);

            await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && u.GroupId == chatId,
                Builders<UserModel>.Update.Set(u => u.WarningCount, 0));
        }

        /// <summary>
        /// Increments message count and sets last activity time to the current time in UTC timezone
        /// </summary>
        /// <param name="message">Original message</param>
        public static async Task UpdateUserActivity(Message message)
        {
            var user = await GetUser(message);

            await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                Builders<UserModel>.Update.Inc(u => u.MessageCount, 1));
            await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                Builders<UserModel>.Update.Set(u => u.LastSeen, DateTime.UtcNow));
        }

        /// <summary>
        /// Updates setting value in configuration.
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="newValue">New value for the setting</param>
        public static async Task UpdateGroupConfigSetting(Message message, string name, string newValue)
        {
            object validatedValue = ConfigValidator.ValidateAndConvert(name, newValue);

            var filter = Builders<GroupModel>.Filter.And(
                Builders<GroupModel>.Filter.Eq(g => g.GroupId, message.Chat.Id),
                Builders<GroupModel>.Filter.ElemMatch(g => g.Config, c => c.Name == name)
            );

            var update = Builders<GroupModel>.Update.Set(g => g.Config.FirstMatchingElement().Value, validatedValue);

            await group_collection.UpdateOneAsync(filter, update);
        }

        public static async Task AddFilter(Message message, Filter.TriggerType triggerType, string trigger,
            string reply
        )
        {
            var group = await GetGroup(message);
            var filter = new Filter
            {
                Trigger = trigger,
                TriggerCondition = triggerType,
                Reply = reply
            };

            if (group.Filters.Find(f => f.Trigger == trigger) != null)
            {
                throw new MessageException($"Trigger '{trigger}' already exists.");
            }

            var dbFilter = Builders<GroupModel>.Filter.Eq(g => g.GroupId, message.Chat.Id);

            var update = Builders<GroupModel>.Update.AddToSet(g => g.Filters, filter);

            await group_collection.UpdateOneAsync(dbFilter, update);
        }

        public static async Task RemoveFilter(Message message, string trigger)
        {
            var group = await GetGroup(message);

            if (group.Filters.Find(f => f.Trigger == trigger) != null)
            {
                var dbFilter = Builders<GroupModel>.Filter.Eq(g => g.GroupId, message.Chat.Id);
                var update = Builders<GroupModel>.Update.PullFilter(g => g.Filters, f => f.Trigger == trigger);

                await group_collection.UpdateOneAsync(dbFilter, update);
            }
            else
            {
                throw new MessageException($"Trigger '{trigger}' does not exist.");
            }
        }
    }
}
