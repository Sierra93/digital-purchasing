using System.Collections.Generic;

namespace DigitalPurchasing.Analysis
{
    public abstract class Pipeline<T>
    {
        protected readonly List<IFilter<T>> Filters = new List<IFilter<T>>();

        public Pipeline<T> Register(IFilter<T> filter)
        {
            Filters.Add(filter);
            return this;
        }

        public Pipeline<T> Register(params IFilter<T>[] filters)
        {
            Pipeline<T> pipeline = default;
            foreach (var filter in filters)
            {
                pipeline = Register(filter);
            }
            return pipeline;
        }

        public abstract T Process(T input);
    }
}
