namespace ZBase.Foundation.Pooling
{
    public interface IReturnable<in T>
    {
        void Return(T instance);
    }
}
