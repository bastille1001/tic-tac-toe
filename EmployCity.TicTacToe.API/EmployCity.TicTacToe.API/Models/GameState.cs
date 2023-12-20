using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmployCity.TicTacToe.API.Models
{
    public class GameState
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Token { get; set; } = null!;
        [BsonElement("currentBoard")]
        public List<List<string>> Board { get; set; } = null!;
        public string? CurrentPlayer { get; set; }
    }
}
