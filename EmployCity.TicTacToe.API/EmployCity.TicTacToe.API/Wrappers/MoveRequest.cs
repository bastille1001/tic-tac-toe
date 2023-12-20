namespace EmployCity.TicTacToe.API.Wrappers
{
    public class MoveRequest
    {
        public string Token { get; set; } = null!;
        public short Row { get; set; }
        public short Column { get; set; }
    }
}
