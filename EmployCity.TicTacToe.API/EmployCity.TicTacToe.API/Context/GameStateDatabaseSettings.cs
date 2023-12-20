namespace EmployCity.TicTacToe.API.Context
{
    public class GameStateDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string GameStateCollectionName { get; set; } = null!;
    }
}
