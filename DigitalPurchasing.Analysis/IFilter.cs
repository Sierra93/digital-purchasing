namespace DigitalPurchasing.Analysis
{
    public interface IFilter<T>
    {
        T Execute(T input);
    }
}
