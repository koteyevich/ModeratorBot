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

    public class PunishmentsModel
    {
        [BsonId] public ObjectId Id { get; set; }
        public long UserId { get; set; }
        public long GroupId { get; set; }

        public long ModeratorId { get; set; }
        public string? ModeratorUsername { get; set; }

        public PunishmentType Type { get; set; }
    }
}
