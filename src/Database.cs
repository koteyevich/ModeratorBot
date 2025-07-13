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
        private static readonly IPAddress ip = new(new byte[] { 192, 168, 222, 222 });
        private const int port = 27017; // Default MongoDB port
        private const string database_name = "moderatorBot";

        // MongoDB client and collections
        private static readonly MongoClient client;
        private static readonly IMongoDatabase mongo_database;

        private static readonly IMongoCollection<UserModel> user_collection;

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
        /// Gets a user by his id and current group
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <returns></returns>
        public static async Task<UserModel> GetUser(Message message)
        {
            var user = await user_collection.Find(u => u.UserId == message.From.Id && u.GroupId == message.Chat.Id)
                .FirstOrDefaultAsync();

            return user ?? await createUser(message);
        }

        public static async Task<UserModel> GetUser(long userId, long chatId)
        {
            var user = await user_collection.Find(u => u.UserId == userId && u.GroupId == chatId).FirstOrDefaultAsync();

            return user;
        }

        private static async Task<UserModel> createUser(Message message)
        {
            var user = new UserModel
            {
                UserId = message.From.Id,
                GroupId = message.Chat.Id,
                Username = message.From.Username
            };

            await user_collection.InsertOneAsync(user);
            return user;
        }

        public static async Task AddPunishment(Message message, PunishmentType punishmentType,
            DateTime? duration = null)
        {
            var punishment = new PunishmentModel()
            {
                ModeratorId = message.From.Id,
                ModeratorUsername = message.From.Username,
                Type = punishmentType,
                Duration = duration,
            };
            if (message.ReplyToMessage != null)
            {
                var user = await GetUser(message.ReplyToMessage);

                await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && u.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.AddToSet(u => u.Punishments, punishment));

                Logger.Debug(
                    "Added punishment type of {PunishmentType} to user {user} in {chat} chat. Until: {duration}",
                    punishment.Type, user.UserId, message.Chat.Id, duration);
            }
            else
            {
                string?[]? args = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

                long userId = long.Parse(args?[0]);
                var user = await GetUser(userId, message.Chat.Id);

                await user_collection.UpdateOneAsync(u => u.UserId == user.UserId && user.GroupId == message.Chat.Id,
                    Builders<UserModel>.Update.AddToSet(u => u.Punishments, punishment));

                Logger.Debug(
                    "Added punishment type of {PunishmentType} to user {user} in {chat} chat. Until: {duration}",
                    punishment.Type, user.UserId, message.Chat.Id, duration);
            }
        }
    }
}
