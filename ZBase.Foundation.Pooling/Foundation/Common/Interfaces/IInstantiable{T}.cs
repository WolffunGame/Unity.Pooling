namespace ZBase.Foundation.Pooling
{
    public interface IInstantiable<out T>
    {
        T InstantiateAsync();
    }
}
