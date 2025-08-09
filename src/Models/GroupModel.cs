using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModeratorBot.Models
{
    public class ConfigEntry<T>
    {
        public required string Name { get; set; }
        public required T Value { get; set; }
        public required T DefaultValue { get; set; }
    }

    public class GroupModel
    {
        [BsonId] public ObjectId Id { get; set; }
        public long GroupId { get; init; }

        [BsonElement("Config")]
        public List<ConfigEntry<object>> Config { get; } = new()
        {
            new ConfigEntry<object> { Name = "WarnBanThreshold", Value = 3, DefaultValue = 3 }
        };
    }
}
