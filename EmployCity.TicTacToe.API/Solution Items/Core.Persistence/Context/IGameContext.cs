using Core.Models;
using MongoDB.Driver;

namespace Core.Persistence.Context
{
    public interface IGameContext
    {
        IMongoCollection<GameInfo> GameInfos { get; }
    }
}