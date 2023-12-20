using Core.Models;
using MongoDB.Driver;

namespace Core.Persistence.Context
{
    public class GameContext : IGameContext
    {
        public GameContext()
        {

        }

        public IMongoCollection<GameInfo> GameInfos { get; set; }
    }
}
