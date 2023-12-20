using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Core.Models
{
    public class GameInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GameToken { get; set; } = default!;

        public List<List<char>> Board { get; set; } = default!;
    }
}
