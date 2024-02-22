
namespace ZBase.Foundation.Pooling
{
    public interface ISyncInstantiable<out T>
    {
        T Instantiate();
    }
}
