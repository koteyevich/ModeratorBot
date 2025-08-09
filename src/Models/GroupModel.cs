using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModeratorBot.Models
{
    public class GroupModel
    {
        [BsonId] public ObjectId Id { get; set; }
        public long GroupId { get; init; }

        [BsonElement("Config")]
        public List<object[]> Config { get; } =
        [
            new object[] { "WarnBanThreshold", 3 },
        ];

        public T? GetConfigValue<T>(string key, T? defaultValue = default)
        {
            object[]? item = Config.FirstOrDefault(kv => kv.Length > 0 && kv[0].ToString() == key);
            if (item == null || item.Length < 2) return defaultValue;
            return item[1] is T value ? value : defaultValue;
        }
    }
}
