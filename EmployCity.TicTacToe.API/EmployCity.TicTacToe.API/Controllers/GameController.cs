using EmployCity.TicTacToe.API.Services;
using EmployCity.TicTacToe.API.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace EmployCity.TicTacToe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GameStateResponse>>> StartGame([FromBody] StartGameRequest startGameRequest)
        {
            var result = await _gameService.CreateAsync(startGameRequest);

            return Ok(result);
        }

        [HttpPost("makeMove")]
        public async Task<ActionResult<ServiceResponse<string>>> MakeMove([FromBody] MoveRequest moveRequest)
        {
            var result = await _gameService.MakeMoveAsync(moveRequest);

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<GameStateResponse>>> GetGame(string token)
        {
            var result = await _gameService.GetByTokenAsync(token);

            return Ok(result);
        }
    }
}
