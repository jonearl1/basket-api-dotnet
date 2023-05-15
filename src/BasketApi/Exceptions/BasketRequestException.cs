namespace BasketApi.Exceptions
{
    public class BasketRequestException: Exception
    {
        public BasketRequestException(string message)
            :base(message)
        {
        }
    }
}
