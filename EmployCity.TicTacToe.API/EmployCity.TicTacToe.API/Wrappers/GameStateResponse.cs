namespace EmployCity.TicTacToe.API.Wrappers
{
    public class GameStateResponse
    {
        public string Token { get; set; }

        public List<List<string>> Board { get; set; }
    }
}
