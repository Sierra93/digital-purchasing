namespace DigitalPurchasing.Core
{
    public class BaseResult<T>
    {
        public T Data { get; }

        public bool IsSuccess { get; set; }

        public BaseResult(bool isSuccess, T data)
        {
            Data = data;
            IsSuccess = isSuccess;
        }

        public BaseResult(T data): this(data != null, data)
        {
        }
    }
}
