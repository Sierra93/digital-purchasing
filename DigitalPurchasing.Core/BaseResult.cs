using System.Collections.Generic;

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

    public class DeleteResultVm : BaseResult<List<string>>
    {
        public DeleteResultVm(bool isSuccess, List<string> data) : base(isSuccess, data)
        {
        }

        public DeleteResultVm(List<string> data) : base(data)
        {
        }

        public static DeleteResultVm Success() => new DeleteResultVm(true, null);

        public static DeleteResultVm Failure(string reason) => new DeleteResultVm(false, new List<string> { reason });
    }
}
