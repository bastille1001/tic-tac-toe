namespace EmployCity.TicTacToe.API.Wrappers
{
    public class ServiceResponse<T> 
        where T : class
    {
        public bool IsSucceeded { get; set; }

        public T? Data { get; set; }

        public string? Message { get; set; }

        public ServiceResponse(T? data)
        {
            IsSucceeded = data != null;
            Data = data;
        }
    }
}
