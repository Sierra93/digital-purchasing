using System.Collections.Generic;

namespace DigitalPurchasing.Services
{
    public abstract class BaseDataResult<TData> where TData: class 
    {
        public int Total { get; set; }
        public List<TData> Data { get; set; } = new List<TData>();
    }
}
