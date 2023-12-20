namespace EmployCity.TicTacToe.API.Extensions
{
    public static class BoardExtension
    {
        public static List<List<string>> ConvertToReadableBoard(this List<List<string>> initialBoard)
        {
            return initialBoard
                .SelectMany(row => row)
                .Select((value, index) => new { value, index })
                .GroupBy(x => x.index / 3)
                .Select(group => group.Select(x => x.value).ToList())
                .ToList();
        }
    }
}
