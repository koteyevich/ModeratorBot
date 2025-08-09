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

    public class GroupModel
    {
        [BsonId] public ObjectId Id { get; set; }
        public long GroupId { get; init; }

        [BsonElement("Config")]
        public List<ConfigEntry<object>> Config { get; private set; } = new()
        {
            new ConfigEntry<object> { Name = "WarnBanThreshold", Value = 3, DefaultValue = 3 }
        };
    }
}
