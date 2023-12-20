using EmployCity.TicTacToe.API.Context;
using EmployCity.TicTacToe.API.Extensions;
using EmployCity.TicTacToe.API.Models;
using EmployCity.TicTacToe.API.Wrappers;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmployCity.TicTacToe.API.Services
{
    public class GameService
    {
        private readonly IMongoCollection<GameState> _gameStateCollection;
        private readonly IValidator<StartGameRequest> _validator;

        public GameService(IOptions<GameStateDatabaseSettings> options,
            IValidator<StartGameRequest> validator)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
            _gameStateCollection = mongoDatabase.GetCollection<GameState>(options.Value.GameStateCollectionName);
            _validator = validator;
        }

        public async Task<ServiceResponse<GameStateResponse>> GetByTokenAsync(string token)
        {
            var gameState = await _gameStateCollection.Find(x => x.Token.Equals(token)).FirstOrDefaultAsync();

            if (gameState is null)
            {
                return new ServiceResponse<GameStateResponse>(null)
                {
                    Message = "Game state not found by token provided"
                };
            }

            var gameStateResponse = new GameStateResponse
            {
                Token = gameState.Token,
                Board = gameState.Board.ConvertToReadableBoard()
            };

            return new ServiceResponse<GameStateResponse>(gameStateResponse);
        }

        public async Task<ServiceResponse<GameStateResponse>> CreateAsync(StartGameRequest startGameRequest)
        {
            var validationResult = _validator.Validate(startGameRequest);

            if (!validationResult.IsValid)
            {
                var response = new ServiceResponse<GameStateResponse>(null);

                foreach (var error in validationResult.Errors)
                {
                    response.Message = error.ErrorMessage;
                }

                return response;
            }

            string gameToken = Guid.NewGuid().ToString();

            var initialBoard = new List<List<string>>
            {
                new List<string> { " ", " ", " " },
                new List<string> { " ", " ", " " },
                new List<string> { " ", " ", " " }
            };

            string playerSymbol = startGameRequest.StartingPlayer;
            string botSymbol = (playerSymbol.Equals("X", StringComparison.InvariantCultureIgnoreCase)) ? "O" : "X";

            if (playerSymbol.Equals("O", StringComparison.InvariantCultureIgnoreCase))
            {
                MakeBotMove(initialBoard, botSymbol);
            }

            var gameState = new GameState
            {
                Token = gameToken,
                Board = initialBoard,
                CurrentPlayer = playerSymbol
            };

            try
            {
                await _gameStateCollection.InsertOneAsync(gameState);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GameStateResponse>(null)
                {
                    Message = ex.ToString()
                };
            }

            var gameStateResponse = new GameStateResponse
            {
                Board = gameState.Board.ConvertToReadableBoard(),
                Token = gameToken
            };

            return new ServiceResponse<GameStateResponse>(gameStateResponse)
            {
                Message = "Game started successfully"
            };
        }

        public async Task<ServiceResponse<string>> MakeMoveAsync(MoveRequest moveRequest)
        {
            var (success, message) = ValidateBoard(moveRequest, out GameState gameState);

            if (!success)
            {
                return new ServiceResponse<string>(message);
            }

            MakePlayerMove(moveRequest, gameState);

            await UpdateGameStateAsync(moveRequest.Token, gameState);

            if (CheckGameResult(gameState, out string? resultMessage))
            {
                return new ServiceResponse<string>(resultMessage);
            }

            ChangePlayer(gameState);

            MakeBotMove(gameState);

            var successResult = CheckGameResult(gameState, out resultMessage);

            ChangePlayer(gameState);

            await UpdateGameStateAsync(moveRequest.Token, gameState);

            if (successResult)
            {
                return new ServiceResponse<string>(resultMessage);
            }

            return new ServiceResponse<string>("Successful move");
        }

        private static void MakePlayerMove(MoveRequest moveRequest, GameState gameState)
        {
            gameState.Board[moveRequest.Row][moveRequest.Column] = gameState.CurrentPlayer;
        }

        private async Task UpdateGameStateAsync(string token, GameState gameState)
        {
            await _gameStateCollection.ReplaceOneAsync(g => g.Token == token, gameState);
        }

        private static bool CheckGameResult(GameState gameState, out string? resultMessage)
        {
            if (CheckForWin(gameState))
            {
                resultMessage = $"Player {gameState.CurrentPlayer} wins!";
                return true;
            }

            if (CheckForDraw(gameState))
            {
                resultMessage = "It's a draw!";
                return true;
            }

            resultMessage = null;
            return false;
        }

        private static void ChangePlayer(GameState gameState)
        {
            gameState.CurrentPlayer = (gameState.CurrentPlayer == "X") ? "O" : "X";
        }

        private (bool success, string message) ValidateBoard(MoveRequest moveRequest, out GameState gameState)
        {
            gameState = _gameStateCollection.Find(g => g.Token.Equals(moveRequest.Token)).FirstOrDefault();

            if (gameState is null)
            {
                return (false, "Board not found");
            }

            if (moveRequest.Row < 0 || moveRequest.Row >= 3 || moveRequest.Column < 0 || moveRequest.Column >= 3)
            {
                return (false, "Invalid coordinates");
            }

            if (gameState.Board[moveRequest.Row][moveRequest.Column] != " ")
            {
                return (false, "Cell is occupied");
            }

            return (true, string.Empty);
        }

        private static bool CheckForWin(GameState gameState)
        {
            return IsWinner(gameState.Board, gameState.CurrentPlayer);
        }

        private static bool CheckForDraw(GameState gameState)
        {
            return IsBoardFull(gameState.Board);
        }

        private static bool IsWinner(List<List<string>> board, string player)
        {
            for (int i = 0; i < 3; i++)
            {
                if ((board[i][0] == player && board[i][1] == player && board[i][2] == player) ||
                    (board[0][i] == player && board[1][i] == player && board[2][i] == player))
                {
                    return true;
                }
            }

            if ((board[0][0] == player && board[1][1] == player && board[2][2] == player) ||
                (board[0][2] == player && board[1][1] == player && board[2][0] == player))
            {
                return true;
            }

            return false;
        }

        private static bool IsBoardFull(List<List<string>> board)
        {
            foreach (var row in board)
            {
                foreach (var cell in row)
                {
                    if (cell == " ")
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void MakeBotMove(List<List<string>> board, string botPlayer)
        {
            var random = new Random();
            int row, col;

            do
            {
                row = random.Next(0, 3);
                col = random.Next(0, 3);
            } while (board[row][col] != " ");

            board[row][col] = botPlayer;
        }

        private static void MakeBotMove(GameState gameState)
        {
            MakeBotMove(gameState.Board, gameState.CurrentPlayer);
        }
    }
}
