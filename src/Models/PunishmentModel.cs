using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModeratorBot.Models
{
    public enum PunishmentType
    {
        Mute,
        Unmute,
        Ban,
        Unban,
        Kick,
        Warning
    }

    public class PunishmentModel
    {
        [BsonId] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? Duration { get; set; }
        public PunishmentType Type { get; set; }

        public long ModeratorId { get; set; }
        public string? ModeratorUsername { get; set; }
    }
}
