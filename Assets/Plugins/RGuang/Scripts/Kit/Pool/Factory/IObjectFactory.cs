
namespace RGuang.Kit
{
    /// <summary>
    /// 对象工厂接
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectFactory<T>
    {
        T Create();
    }

}
