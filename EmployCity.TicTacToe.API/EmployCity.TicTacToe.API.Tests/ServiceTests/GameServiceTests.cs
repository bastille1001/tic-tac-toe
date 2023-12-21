using EmployCity.TicTacToe.API.Context;
using EmployCity.TicTacToe.API.Models;
using EmployCity.TicTacToe.API.Services;
using EmployCity.TicTacToe.API.Wrappers;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

namespace EmployCity.TicTacToe.API.Tests.ServiceTests
{
    public class GameServiceTests
    {
        [Fact]
        public async Task GetByTokenAsync_GameStateFound_ReturnsCorrectResponse()
        {
            // Arrange
            var token = "validToken";
            var expectedGameState = new GameState { Token = token, Board = new List<List<string>> { /* board data here */ } };

            var list = new List<GameState>
            {
                expectedGameState
            };

            var optionsMock = new Mock<IOptions<GameStateDatabaseSettings>>();
            var validatorMock = new Mock<IValidator<StartGameRequest>>();
            var gameStateCollectionMock = new Mock<IMongoCollection<GameState>>();
            var asyncCursorMock = new Mock<IAsyncCursor<GameState>>();

            optionsMock.Setup(x => x.Value)
                .Returns(new GameStateDatabaseSettings { ConnectionString = "mongodb://localhost:27017", DatabaseName = "testDb", GameStateCollectionName = "testGameState" });

            var gameStateCursorMock = new Mock<IAsyncCursor<GameState>>();
            gameStateCursorMock.Setup(x => x.Current).Returns(new List<GameState> { expectedGameState });

            gameStateCollectionMock
                .Setup(x => x.FindSync(Builders<GameState>.Filter.Empty, It.IsAny<FindOptions<GameState>>(), default))
                .Returns(asyncCursorMock.Object);

            asyncCursorMock.Setup(a => a.Current).Returns(list);

            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            mongoDatabaseMock.Setup(x => x.GetCollection<GameState>("testGameState", It.IsAny<MongoCollectionSettings>()))
                .Returns(gameStateCollectionMock.Object);

            var mongoClientMock = new Mock<IMongoClient>();
            mongoClientMock.Setup(x => x.GetDatabase("testDb", It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            var gameService = new GameService(optionsMock.Object, validatorMock.Object);

            // Act
            var result = await gameService.GetByTokenAsync(token);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(token, result?.Data?.Token);
            Assert.Equal(expectedGameState.Board, result?.Data?.Board);
        }

        [Fact]
        public async Task GetByTokenAsync_GameStateNotFound_ReturnsNullDataWithMessage()
        {
            // Arrange
            //var token = "nonExistentToken";

            //var optionsMock = new Mock<IOptions<GameStateDatabaseSettings>>();
            //var validatorMock = new Mock<IValidator<StartGameRequest>>();
            //var gameStateCollectionWrapperMock = new Mock<IMongoCollectionWrapper<GameState>>();

            //optionsMock.Setup(x => x.Value).Returns(new GameStateDatabaseSettings { ConnectionString = "connectionString", DatabaseName = "dbName", GameStateCollectionName = "collectionName" });

            //gameStateCollectionWrapperMock
            //    .Setup(x => x.Find(It.IsAny<FilterDefinition<GameState>>()))
            //    .Returns(Mock.Of<IFindFluent<GameState, GameState>>());

            //var gameService = new GameService(optionsMock.Object, validatorMock.Object, gameStateCollectionWrapperMock.Object);

            //// Act
            //var result = await gameService.GetByTokenAsync(token);

            //// Assert
            //Assert.NotNull(result);
            //Assert.Null(result.Data);
            //Assert.Equal("Game state not found by token provided", result.Message);
        }
    }
}
