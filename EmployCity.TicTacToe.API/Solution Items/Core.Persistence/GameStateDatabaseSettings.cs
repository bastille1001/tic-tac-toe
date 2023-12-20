namespace Core.Persistence
{
    public class GameStateDatabaseSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
        public string BooksCollectionName { get; set; } = default!;
    }
}
