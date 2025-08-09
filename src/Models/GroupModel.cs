using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModeratorBot.Models
{
    public class ConfigEntry<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
        public T DefaultValue { get; set; }
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
