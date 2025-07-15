using System.Net;
using ModeratorBot.Models;
using MongoDB.Driver;
using Telegram.Bot.Types;

namespace ModeratorBot
{
    public static class Database
    {
        //* the brain of the bot

        // Constants
        private static readonly IPAddress? ip = new(new byte[] { 192, 168, 222, 222 });
        private const int port = 27017; // Default MongoDB port
        private const string database_name = "moderatorBot";

        // MongoDB client and collections
        private static readonly MongoClient client;
        private static readonly IMongoDatabase mongo_database;

        private static readonly IMongoCollection<UserModel> user_collection;

        public const short MAX_WARNS = 3;

        static Database()
        {
            try
            {
                string connectionString = $"mongodb://{Secrets.DB_USERNAME}:{Secrets.DB_PASSWORD}@{ip}:{port}";
                Logger.Debug("Initializing connecting to MongoDB with ip {Host} and port {Port}. User: {User}", ip,
                    port,
                    Secrets.DB_USERNAME);

                client = new MongoClient(connectionString);
                mongo_database = client.GetDatabase(database_name);
                user_collection = mongo_database.GetCollection<UserModel>("Users");

                Logger.Info("MongoDB connection successful.");
            }
            catch (Exception ex)
            {
                throw new Exception("Could not connect to MongoDB.", ex);
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
                ModeratorId = message.From.Id,
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
                string?[]? args = message.Text?.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)
                    .ToArray();
                if (args?.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new Exceptions.Message("Provide a valid user ID when not replying to a message.");
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
                string?[]? args = message.Text?.Split("\n")[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)
                    .ToArray();
                if (args?.Length == 0 || string.IsNullOrEmpty(args[0]) || !long.TryParse(args[0], out long userId))
                {
                    throw new Exceptions.Message("Provide a valid user ID when not replying to a message.");
                }

                var user = await GetUser(userId, message.Chat.Id);

                await AddPunishment(message, PunishmentType.Warning, reason: reason);
                await user_collection.UpdateOneAsync(
                    u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.Inc(u => u.WarningCount, 1));
            }
        }

        public static async Task ResetWarning(long userId, long chatId)
        {
            var user = await GetUser(userId, chatId);

            await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && u.GroupId == chatId,
                Builders<UserModel>.Update.Set(u => u.WarningCount, 0));
        }

        public static async Task UpdateUserActivity(Message message)
        {
            var user = await GetUser(message);

            await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                Builders<UserModel>.Update.Inc(u => u.MessageCount, 1));
            await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                Builders<UserModel>.Update.Set(u => u.LastSeen, DateTime.UtcNow));
        }
    }
}
