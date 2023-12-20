namespace EmployCity.TicTacToe.API.Extensions
{
    public static class StringExtension
    {
        public static char ConvertToChar(this string origin)
        {
            if (origin is null)
            {
                return ' ';
            }

            return origin.ToCharArray()[0];
        }
    }
}
