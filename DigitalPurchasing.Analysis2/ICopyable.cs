namespace DigitalPurchasing.Analysis2
{
    public interface ICopyable<out T> where T : ICopyable<T>
    {
        T Copy();
    }
}