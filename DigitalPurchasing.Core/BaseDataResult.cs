using System.Collections.Generic;

namespace DigitalPurchasing.Core
{
    public abstract class BaseDataResponse<TData> where TData: class 
    {
        public int Total { get; set; }
        public List<TData> Data { get; set; } = new List<TData>();
    }
}
