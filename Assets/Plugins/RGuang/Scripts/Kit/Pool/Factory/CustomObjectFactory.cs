using System;

namespace RGuang.Kit
{

    /// <summary>
    /// 自定义对象工厂：相关对象是 自己定义 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomObjectFactory<T>:IObjectFactory<T>
    {
        protected Func<T> _factoryMethod;
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public T Create()
        {
            return _factoryMethod();
        }
    }
}
