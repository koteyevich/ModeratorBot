using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModeratorBot.Models
{
    public class ConfigEntry<T>
    {
        public required string Name { get; init; }
        public required T Value { get; init; }
        public required T DefaultValue { get; init; }
    }

    public class Filter
    {
        public enum TriggerType
        {
            Exact,
            Contains,
        }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required string Trigger { get; set; }
        public required string Reply { get; set; }
        public required TriggerType TriggerCondition { get; set; }
    }

    public class GroupModel
    {
        [BsonId] public ObjectId Id { get; set; }
        public long GroupId { get; init; }

        [BsonElement("Config")]
        public List<ConfigEntry<object>> Config { get; private set; } =
        [
            new() { Name = "WarnBanThreshold", Value = 3, DefaultValue = 3 }
        ];

        [BsonElement("Filters")] public List<Filter> Filters { get; private set; } = [];
    }
}
