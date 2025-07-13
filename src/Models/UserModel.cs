using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModeratorBot.Models
{
    public class UserModel
    {
        [BsonId] public ObjectId Id { get; set; }
        public long UserId { get; set; }
        public long GroupId { get; set; }
        public string? Username { get; set; }

        public DateTime FirstSeen { get; set; } = DateTime.UtcNow;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        public long MessageCount { get; set; } = 0;
        public int WarningCount { get; set; } = 0;

        public PunishmentModel[] Punishments { get; set; } = [];
    }
}
