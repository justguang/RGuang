using System;
using System.Collections.Generic;
/*
* ==============================================================================
* RGuang v1.1.1
* 
* 极简型架构
* - Gitee: https://gitee.com/justguang/RGuang/tree/master/Assets/Plugins/RGuang
* 
* 
* 编写：
*   - JustGuang: https://gitee.com/justguang
*   - qq：2320366520
* 
* 
* 原架构作者[凉鞋]:
* - Github: https://github.com/liangxiegame/QFramework
* - Gitee: https://gitee.com/liangxiegame/QFramework
* 
* 
* 【IModelHub】：架构中心 自定义类继承[ModelHub]并要求实现一个非公有无参的构造函数，
*               作用管理所有Model、System、Utility。
*               
*               通过[RegisterModel、RegisterSytem、RegisterUtility]注册目标对象
*               通过[UnRegisterModel、UnRegisterSytem、UnRegisterUtility]注销目标对象
*               抽象方法 [BuildModelHub] 在架构第一次构建时触发。
*               抽象方法 [OnModelHub2DebugWarn、OnModelHub2DebugError] 在 Editor 中会调用，参数为异常信息。
* 
* 【IView】：表现层，挂载在场景中的物体继承次接口，可使用架构表现层功能
* 
* 【ISystem】：系统层[分摊 View 层的逻辑压力]
* 
* 【IModel】：数据层
* 
* 【IUtility】：工具
* 
* 【ICmd】：命令
* 【IQuery】：查询
* 【Event】: 事件
* 
* 
* ==============================================================================
*/
namespace RGuang
{
    #region Hub Global Config
    public static class ModelHubGlobalConfig
    {
        /// <summary>
        /// 版本
        /// </summary>
        public const string Version = "1.1.1";

        /// <summary>
        /// True懒加载、初始化
        /// </summary>
        public const bool LazyInit = true;

        /// <summary>
        /// 过时api禁用
        /// </summary>
        public const bool ObsoleteAPI2Disable = false;

    }
    #endregion


    #region IModelHub
    /// <summary>
    /// 定义模块中心的接口，负责管理模块、工具、事件、通知、命令和查询。
    /// </summary>
    public partial interface IModelHub
    {
        M GetModel<M>() where M : class, IModel;
        S GetSystem<S>() where S : class, ISystem;
        U GetUtility<U>() where U : class, IUtility;

        IRemover AddEvent<E>(Action evt) where E : struct;
        IRemover AddEvent<E>(Action<E> evt) where E : struct;

        IRemover AddEvent<E>(E type, Action evt) where E : IConvertible;
        IRemover AddEvent<E, T>(E type, Action<T> evt) where E : IConvertible;

        void RmvEvent<E>(Action evt) where E : struct;
        void RmvEvent<E>(Action<E> evt) where E : struct;

        void RmvEvent<E>(E type, Action evt) where E : IConvertible;
        void RmvEvent<E, T>(E type, Action<T> evt) where E : IConvertible;

        void SendEvent<E>(E e) where E : struct;
        void SendEvent<E>() where E : struct;

        void EnumEvent<E>(E type) where E : IConvertible;
        void EnumEvent<E, T>(E type, T info) where E : IConvertible;

        void SendCmd<C>(C cmd) where C : ICmd;
        void SendCmd<C>() where C : struct, ICmd;
        void SendCmd<C, TArgs>(C cmd, TArgs info) where C : ICmd<TArgs>;
        void SendCmd<C, TArgs>(TArgs info) where C : struct, ICmd<TArgs>;

        TResult SendQuery<Q, TResult>() where Q : struct, IQuery<TResult>;
        TResult SendQuery<Q, TArgs, TResult>(TArgs info) where Q : struct, IQuery<TArgs, TResult>;
    }


    #region Abstact ModelHub
    public abstract partial class ModelHub<H> : IModelHub where H : ModelHub<H>
    {
        private static H m_Hub;
        private static readonly string m_Lock = "ModelHubLock";
        public static IModelHub GetIns()
        {
            if (m_Hub == null)
            {
                lock (m_Lock)
                {
                    if (m_Hub == null)
                    {
                        var type = typeof(H);
                        System.Reflection.ConstructorInfo constructor = null;
                        try
                        {
                            var ctorArr = type.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                            constructor = Array.Find(ctorArr, c => c.GetParameters().Length == 0);
                            if (constructor == null)
                            {
#if UNITY_EDITOR
                                throw new Exception($"请实现一个无参非公有的构造函数！");
#endif
                            }

                            m_Hub = constructor?.Invoke(null) as H;
                            m_Hub.BuildModelHub();
                        }
                        catch (Exception e)
                        {
#if UNITY_EDITOR
                            throw e;
#endif
                        }

                    }
                }
            }
            return m_Hub;
        }
        private Dictionary<Type, IUtility> m_Utilities = new Dictionary<Type, IUtility>();
        private Dictionary<Type, ISystem> m_System = new Dictionary<Type, ISystem>();
        private Dictionary<Type, IModel> m_Models = new Dictionary<Type, IModel>();
        private Dictionary<Type, Delegate> m_Events = new Dictionary<Type, Delegate>();
        private Dictionary<Type, Delegate[]> m_EnumEvents = new Dictionary<Type, Delegate[]>();

        /// <summary>
        /// 由子类重写实现所有模块和工具的构建与注册
        /// </summary>
        protected abstract void BuildModelHub();
        /// <summary>
        /// Debug：框架内警告信息
        /// </summary>
        /// <param name="msg"></param>
        protected abstract void OnModelHub2DebugWarn(string msg);
        /// <summary>
        /// Debug：框架内错误信息
        /// </summary>
        /// <param name="msg"></param>
        protected abstract void OnModelHub2DebugError(string msg);

        /// <summary>
        /// 清理所有已初始化模块的状态信息
        /// </summary>
        protected void Dispose()
        {
            var MIterator = m_Models.GetEnumerator();
            while (MIterator.MoveNext())
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"ModelHub-ModelKey[{MIterator.Current.Key}],ModelHub-ModelKey[{MIterator.Current.Value}] Deinit.");
#endif
                (MIterator.Current.Value as ICanInit)?.DeInit();
            }
            var SIterator = m_System.GetEnumerator();
            while (SIterator.MoveNext())
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"ModelHub-SystemKey[{SIterator.Current.Key}],ModelHub-SystemKey[{SIterator.Current.Value}] Deinit.");
#endif
                (SIterator.Current.Value as ICanInit)?.DeInit();
            }

            var UIterator = m_Utilities.GetEnumerator();
            while (UIterator.MoveNext())
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"ModelHub-UtilityKey[{UIterator.Current.Key}],ModelHub-UtilityKey[{UIterator.Current.Value}] Deinit.");
#endif
                (UIterator.Current.Value as ICanInit)?.DeInit();
            }

            m_Models.Clear();
            m_System.Clear();
            m_Utilities.Clear();
            m_Events.Clear();
            m_EnumEvents.Clear();
        }



        #region Register/UnRegister [Model、System、Utility]
        /// <summary>
        /// 注册IModel
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="model"></param>
        protected void RegisterModel<M>(M model) where M : IModel
        {
            var key = typeof(M);
            if (m_Models.TryGetValue(key, out var existM))
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"重复注册Model[{key}]");
#endif
            }
            else
            {
                m_Models.Add(key, model);
                model.SetModelHub(this);
                ICanInit checkInit = (model as ICanInit);
                if (checkInit != null && checkInit.LazyInit == false && checkInit.Initialzed == false)
                {
                    checkInit.Init();
                }
            }

        }
        /// <summary>
        /// 注销 Model
        /// </summary>
        /// <typeparam name="M"></typeparam>
        protected void UnRegisterModel<M>() where M : IModel
        {
            var key = typeof(M);
            if (m_Models.TryGetValue(key, out var existM))
            {
                m_Models.Remove(key);
                (existM as ICanInit)?.DeInit();
            }
            else
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"注销Model失败，未注册Model[{key}]");
#endif
            }
        }

        /// <summary>
        /// 注册ISystem
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="system"></param>
        protected void RegisterSystem<S>(S system) where S : ISystem
        {
            var key = typeof(S);
            if (m_System.TryGetValue(key, out var existS))
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"重复注册System[{key}]");
#endif
            }
            else
            {
                m_System.Add(key, system);
                system.SetModelHub(this);
                ICanInit checkInit = (system as ICanInit);
                if (checkInit != null && checkInit.LazyInit == false && checkInit.Initialzed == false)
                {
                    checkInit.Init();
                }
            }

        }
        /// <summary>
        /// 注销 System
        /// </summary>
        /// <typeparam name="S"></typeparam>
        protected void UnRegisterSystem<S>() where S : ISystem
        {
            var key = typeof(S);
            if (m_System.TryGetValue(key, out var existS))
            {
                m_System.Remove(key);
                (existS as ICanInit)?.DeInit();
            }
            else
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"注销System失败，未注册System[{key}]");
#endif
            }
        }

        /// <summary>
        /// 注册Utility
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="utility"></param>
        protected void RegisterUtility<U>(U utility) where U : IUtility
        {
            var key = typeof(U);
            if (m_Utilities.TryGetValue(key, out var existU))
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"重复注册Utility[{key}]");
#endif
            }
            else
            {
                m_Utilities.Add(key, utility);
                utility.SetModelHub(this);
                ICanInit checkInit = (utility as ICanInit);
                if (checkInit != null && checkInit.LazyInit == false && checkInit.Initialzed == false)
                {
                    checkInit.Init();
                }
            }
        }

        /// <summary>
        /// 注销 Utility
        /// </summary>
        /// <typeparam name="U"></typeparam>
        protected void UnRegisterUtility<U>() where U : IUtility
        {
            var key = typeof(U);
            if (m_Utilities.TryGetValue(key, out var existU))
            {
                m_Utilities.Remove(key);
                (existU as ICanInit)?.DeInit();
            }
            else
            {
#if UNITY_EDITOR
                OnModelHub2DebugWarn($"注销Utility失败，未注册Utility[{key}]");
#endif
            }
        }
        #endregion


        M IModelHub.GetModel<M>()
        {
            if (m_Models.TryGetValue(typeof(M), out var ret))
            {
                ICanInit checkInit = (ret as ICanInit);
                if (checkInit != null && checkInit.LazyInit && checkInit.Initialzed == false)
                {
                    checkInit.Init();
                }
                return ret as M;
            }

#if UNITY_EDITOR
            OnModelHub2DebugError($"{typeof(M)} Model未注册");
            //throw new Exception($"{typeof(M)} Model未注册");
#endif
            return null;
        }
        S IModelHub.GetSystem<S>()
        {
            if (m_System.TryGetValue(typeof(S), out var ret))
            {
                ICanInit checkInit = (ret as ICanInit);
                if (checkInit != null && checkInit.LazyInit && checkInit.Initialzed == false)
                {
                    checkInit.Init();
                }
                return ret as S;
            }

#if UNITY_EDITOR
            OnModelHub2DebugError($"{typeof(S)} System未注册");
            //throw new Exception($"{typeof(S)} System未注册");
#endif
            return null;
        }
        U IModelHub.GetUtility<U>()
        {

            if (m_Utilities.TryGetValue(typeof(U), out var ret))
            {
                ICanInit checkInit = (ret as ICanInit);
                if (checkInit != null && checkInit.LazyInit && checkInit.Initialzed == false)
                {
                    checkInit.Init();
                }
                return ret as U;
            }
#if UNITY_EDITOR
            OnModelHub2DebugError($"{typeof(U)} Utility未注册");
            //throw new Exception($"{typeof(U)} Utility未注册");
#endif
            return null;
        }
        private IRemover Combine<E>(Delegate evt) where E : struct
        {
            if (evt == null)
            {
#if UNITY_EDITOR
                OnModelHub2DebugError($"要添加项 不可为Null");
                //if (evt == null) throw new Exception($"要添加项 不可为Null");
#endif
                return null;
            }

            var key = typeof(E);
            if (m_Events.TryGetValue(key, out var methods))
            {
                if (methods is Action && evt is Action<E>)
                {
                    //if (evt is Action<E>) throw new Exception($"你要添加的是有参数的事件函数， 请使用 AddEvent<E>(Action<E> evt)");
#if UNITY_EDITOR
                    OnModelHub2DebugError($"你要添加的是有参数的事件函数， 请使用 AddEvent<E>(Action<E> evt)");
#endif
                    return null;
                }
                else if (methods is Action<E> && evt is Action)
                {
#if UNITY_EDITOR
                    //if (evt is Action) throw new Exception($"你要添加的是有无数的事件函数， 请使用 AddEvent<E>(Action evt)");
                    OnModelHub2DebugError($"你要添加的是有无数的事件函数， 请使用 AddEvent<E>(Action evt)");
#endif
                    return null;
                }
                m_Events[key] = Delegate.Combine(methods, evt);
            }
            else m_Events.Add(key, evt);
            // 添加一个自定义移除
            return new CustomRemover(() => Separate<E>(evt));
        }
        /// <summary>
        /// 将委托从字典中移除
        /// </summary>
        private void Separate<E>(Delegate evt) where E : struct
        {
            if (evt == null)
            {
#if UNITY_EDITOR
                //if (evt == null) throw new Exception($"要移除项 不可为Null");
                OnModelHub2DebugError($"要移除项 不可为Null");
#endif
                return;
            }

            var key = typeof(E);
            if (m_Events.TryGetValue(key, out var methods))
            {
                if (methods is Action && evt is Action<E>)
                {
#if UNITY_EDITOR
                    //if (evt is Action<E>) throw new Exception($"你要移除的是有参数的事件函数， 请使用 RmvEvent<E>(Action<E> evt)");
                    OnModelHub2DebugError($"你要移除的是有参数的事件函数， 请使用 RmvEvent<E>(Action<E> evt)");
#endif
                    return;
                }
                else if (methods is Action<E> && evt is Action)
                {
#if UNITY_EDITOR
                    //if (evt is Action) throw new Exception($"你要移除的是有无数的事件函数， 请使用 RmvEvent<E>(Action evt)");
                    OnModelHub2DebugError($"你要移除的是有无数的事件函数， 请使用 RmvEvent<E>(Action evt)");
#endif
                    return;
                }
                methods = Delegate.Remove(methods, evt);
                if (methods == null) m_Events.Remove(key);
                else m_Events[key] = methods;
            }
#if UNITY_EDITOR
            else { OnModelHub2DebugWarn($"{key} 事件Key不存在"); }
#endif
        }
        IRemover IModelHub.AddEvent<E>(Action evt) => Combine<E>(evt);
        IRemover IModelHub.AddEvent<E>(Action<E> evt) => Combine<E>(evt);
        void IModelHub.RmvEvent<E>(Action evt) => Separate<E>(evt);
        void IModelHub.RmvEvent<E>(Action<E> evt) => Separate<E>(evt);
        void IModelHub.SendEvent<E>(E e)
        {
            if (m_Events.TryGetValue(typeof(E), out var methods))
            {
                if (methods is Action)
                {
#if UNITY_EDITOR
                    //if (methods is Action) throw new Exception($"{typeof(E)}为无参事件 请使用 SendEvent<E>() 或将注册替换成 AddEvent<E>(Action<E> evt)");
                    OnModelHub2DebugError($"{typeof(E)}为无参事件 请使用 SendEvent<E>() 或将注册替换成 AddEvent<E>(Action<E> evt)");
#endif
                    return;
                }
                (methods as Action<E>).Invoke(e);
            }
#if UNITY_EDITOR
            else { OnModelHub2DebugWarn($"{typeof(E)} 事件未注册"); }
#endif
        }
        void IModelHub.SendEvent<E>()
        {
            if (m_Events.TryGetValue(typeof(E), out var methods))
            {

                if (methods is Action<E>)
                {
#if UNITY_EDITOR
                    //if (methods is Action<E>) throw new Exception($"{typeof(E)}为有参事件 请使用 SendEvent<E>(E e) 或将注册替换成 AddEvent<E>(Action evt)");
                    OnModelHub2DebugError($"{typeof(E)}为有参事件 请使用 SendEvent<E>(E e) 或将注册替换成 AddEvent<E>(Action evt)");
#endif
                    return;
                }
                (methods as Action).Invoke();
            }
#if UNITY_EDITOR
            else { OnModelHub2DebugWarn($"{typeof(E)} 事件未注册"); }
#endif
        }

        // 以下使用枚举转换为索引来驱动事件
        IRemover IModelHub.AddEvent<E>(E type, Action evt)
        {
            if (evt == null)
            {
#if UNITY_EDITOR
                //if (evt == null) throw new Exception($"{evt} 不可为Null");
                OnModelHub2DebugError($"{evt} 不可为Null");
#endif
                return null;
            }
            var key = typeof(E);
            int id = type.ToInt32(null);
            if (m_EnumEvents.TryGetValue(key, out var arr))
            {
                var ms = arr[id];
                if (ms == null)
                {
                    arr[id] = evt;
                }
                else
                {
                    if (!(ms is Action))
                    {
#if UNITY_EDITOR
                        //if (!(ms is Action)) throw new Exception($"{key}为有参事件 请使用 AddEvent<E,T>(E type, Action<T> evt)");
                        OnModelHub2DebugError($"{key}为有参事件 请使用 AddEvent<E,T>(E type, Action<T> evt)");
#endif
                        return null;
                    }
                    arr[id] = Delegate.Combine(ms, evt);
                }
            }
            else
            {
                arr = new Delegate[Enum.GetValues(key).Length];
                arr[id] = evt;
                m_EnumEvents.Add(key, arr);
            }
            // 添加一个自定义移除
            return new CustomRemover(() => RmvEvent(type, evt));
        }
        IRemover IModelHub.AddEvent<E, T>(E type, Action<T> evt)
        {
            if (evt == null)
            {
#if UNITY_EDITOR
                //if (evt == null) throw new Exception($"{evt} 不可为Null");
                OnModelHub2DebugError($"{evt} 不可为Null");
#endif
                return null;
            }
            var key = typeof(E);
            int id = type.ToInt32(null);
            if (m_EnumEvents.TryGetValue(key, out var arr))
            {
                var ms = arr[id];
                if (ms == null)
                {
                    arr[id] = evt;
                }
                else
                {
                    if (ms is Action)
                    {
#if UNITY_EDITOR
                        //if (ms is Action) throw new Exception($"{key}为无参事件 请使用 AddEvent<E>(E type, Action evt)");
                        OnModelHub2DebugError($"{key}为无参事件 请使用 AddEvent<E>(E type, Action evt)");
#endif
                        return null;
                    }
                    arr[id] = Delegate.Combine(ms, evt);
                }
            }
            else
            {
                arr = new Delegate[Enum.GetValues(key).Length];
                arr[id] = evt;
                m_EnumEvents.Add(key, arr);
            }
            // 添加一个自定义移除
            return new CustomRemover(() => RmvEvent(type, evt));
        }
        public void RmvEvent<E>(E type, Action evt) where E : IConvertible
        {
            if (evt == null)
            {
#if UNITY_EDITOR
                //if (evt == null) throw new Exception($"{evt} 不可为Null");
                OnModelHub2DebugError($"{evt} 不可为Null");
#endif
                return;
            }
            var key = typeof(E);
            if (m_EnumEvents.TryGetValue(key, out var arr))
            {
                int id = type.ToInt32(null);
                var ms = arr[id];
                if (ms == null)
                {
#if UNITY_EDITOR
                    OnModelHub2DebugWarn($"事件{id}未被注册 请检查逻辑错误");
#endif
                    return;
                }
                if (!(ms is Action))
                {
#if UNITY_EDITOR
                    //if (!(ms is Action)) throw new Exception($"{key}为有参事件 请使用 RmvEvent<E,T>(E type, Action<T> evt)");
                    OnModelHub2DebugError($"{key}为有参事件 请使用 RmvEvent<E,T>(E type, Action<T> evt)");
#endif
                    return;
                }
                arr[id] = Delegate.Remove(ms, evt);
            }
#if UNITY_EDITOR
            else { OnModelHub2DebugWarn($"{key} 当前枚举事件组不存在"); }
#endif
        }
        public void RmvEvent<E, T>(E type, Action<T> evt) where E : IConvertible
        {
            if (evt == null)
            {
#if UNITY_EDITOR
                //if (evt == null) throw new Exception($"{evt} 不可为Null");
                OnModelHub2DebugError($"{evt} 不可为Null");
#endif
                return;
            }
            var key = typeof(E);
            if (m_EnumEvents.TryGetValue(key, out var arr))
            {
                int id = type.ToInt32(null);
                var ms = arr[id];
                if (ms == null)
                {
#if UNITY_EDITOR
                    OnModelHub2DebugWarn($"事件{id}未被注册 请检查逻辑错误");
#endif
                    return;
                }
                if (ms is Action)
                {
#if UNITY_EDITOR
                    OnModelHub2DebugError($"{key}为无参事件 请使用 RmvEvent<E>(E type, Action evt)");
                    //if (ms is Action) throw new Exception($"{key}为无参事件 请使用 RmvEvent<E>(E type, Action evt)");
#endif
                    return;
                }
                arr[id] = Delegate.Remove(ms, evt);
            }
#if UNITY_EDITOR
            else
            {
                OnModelHub2DebugWarn($"{key} 当前枚举事件组不存在");
            }
#endif
        }

        void IModelHub.EnumEvent<E>(E type)
        {

            var key = typeof(E);
            if (m_EnumEvents.TryGetValue(key, out var arr))
            {
                int id = type.ToInt32(null);
                var ms = arr[id];
                if (ms == null)
                {
#if UNITY_EDITOR
                    OnModelHub2DebugWarn($"事件{id}未被注册 请检查逻辑错误");
#endif
                    return;
                }
                if (!(ms is Action))
                {
#if UNITY_EDITOR
                    //if (!(ms is Action)) throw new Exception($"{key}为有参事件 请使用 EnumEvent<E,T>(E type,T info)");
                    OnModelHub2DebugError($"{key}为有参事件 请使用 EnumEvent<E,T>(E type,T info)");
#endif
                    return;
                }
                (ms as Action).Invoke();
            }
#if UNITY_EDITOR
            else { OnModelHub2DebugWarn($"{key} 当前枚举事件组不存在"); }
#endif
        }
        void IModelHub.EnumEvent<E, T>(E type, T info)
        {
            var key = typeof(E);
            if (m_EnumEvents.TryGetValue(key, out var arr))
            {
                int id = type.ToInt32(null);
                var ms = arr[id];
                if (ms == null)
                {
#if UNITY_EDITOR
                    OnModelHub2DebugWarn($"事件{id}未被注册 请检查逻辑错误");
#endif
                    return;
                }
                if (ms is Action)
                {
#if UNITY_EDITOR
                    //if (ms is Action) throw new Exception($"{key}为无参事件 请使用 EnumEvent<E>(E type)");
                    OnModelHub2DebugError($"{key}为无参事件 请使用 EnumEvent<E>(E type)");
#endif
                    return;
                }
                (ms as Action<T>).Invoke(info);
            }
#if UNITY_EDITOR
            else { OnModelHub2DebugWarn($"{key} 当前枚举事件组不存在"); }
#endif
        }
        void IModelHub.SendCmd<C>() => SendCmd(new C());
        void IModelHub.SendCmd<C, TArgs>(TArgs info) => SendCmd(new C(), info);
        /// <summary>
        /// 可重写的命令 架构子类可对该命令逻辑进行重写 例如在命令前后记录日志
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="cmd"></param>
        public virtual void SendCmd<C>(C cmd) where C : ICmd => cmd.Do();

        /// <summary>
        /// 可重写的命令 架构子类可对该命令逻辑进行重写 例如在命令前后记录日志
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="cmd"></param>
        public virtual void SendCmd<C, TArgs>(C cmd, TArgs info) where C : ICmd<TArgs> => cmd.Do(info);

        TResult IModelHub.SendQuery<Q, TResult>() => new Q().Do();
        TResult IModelHub.SendQuery<Q, TArgs, TResult>(TArgs info) => new Q().Do(info);
    }
    #endregion

    #endregion


    #region IView
    public interface IView : IBelong2ModelHub, ICanGetModel, ICanGetSystem, ICanGetUtility, ICanListenEvent, ICanSendCmd, ICanSendQuery
    {

    }
    #endregion


    #region System
    public interface ISystem : IBelong2ModelHub, ICanSetModelHub, ICanInit, ICanGetModel, ICanGetUtility, ICanSendCmd, ICanSendEvent, ICanSendQuery
    {

    }
    public abstract class AbsSystem : ISystem
    {
        private readonly bool m_lazyInit;
        private bool m_initialzed;
        private IModelHub m_Hub;

        public AbsSystem(bool lazyInit = true)
        {
            m_lazyInit = lazyInit;
        }
        void ICanSetModelHub.SetModelHub(IModelHub modelHub)
        {
            m_Hub = modelHub;
        }
        IModelHub IBelong2ModelHub.GetModelHub() => m_Hub;

        void ICanInit.Init()
        {
            if (Initialzed == false)
            {
                OnInit();
                Initialzed = true;
            }
        }
        void ICanInit.DeInit()
        {
            if (Initialzed)
            {
                OnDeInit();
                Initialzed = false;
            }
        }
        protected abstract void OnInit();
        protected abstract void OnDeInit();

        public bool Initialzed
        {
            get => m_initialzed;
            set => m_initialzed = value;
        }
        public bool LazyInit
        {
            get => m_lazyInit;
        }
    }
    #endregion


    #region Model
    public interface IModel : IBelong2ModelHub, ICanSetModelHub, ICanInit, ICanSendEvent, ICanGetUtility
    {

    }

    public abstract class AbsModel : IModel
    {
        private readonly bool m_lazyInit;
        private bool m_initialzed;
        private IModelHub m_Hub;

        public AbsModel(bool lazyInit = true)
        {
            m_lazyInit = lazyInit;
        }
        void ICanSetModelHub.SetModelHub(IModelHub modelHub)
        {
            m_Hub = modelHub;
        }
        IModelHub IBelong2ModelHub.GetModelHub() => m_Hub;


        void ICanInit.Init()
        {
            if (Initialzed == false)
            {
                OnInit();
                Initialzed = true;
            }
        }
        void ICanInit.DeInit()
        {
            if (Initialzed)
            {
                OnDeInit();
                Initialzed = false;
            }
        }
        protected abstract void OnInit();
        protected abstract void OnDeInit();

        public bool Initialzed
        {
            get => m_initialzed;
            set => m_initialzed = value;
        }
        public bool LazyInit
        {
            get => m_lazyInit;
        }
    }
    #endregion



    #region Utility
    public interface IUtility : IBelong2ModelHub, ICanSetModelHub, ICanInit, ICanSendEvent { }

    public abstract class AbsUtility : ISystem
    {
        private readonly bool m_lazyInit;
        private bool m_initialzed;
        private IModelHub m_Hub;

        public AbsUtility(bool lazyInit = true)
        {
            m_lazyInit = lazyInit;
        }
        void ICanSetModelHub.SetModelHub(IModelHub modelHub)
        {
            m_Hub = modelHub;
        }
        IModelHub IBelong2ModelHub.GetModelHub() => m_Hub;

        void ICanInit.Init()
        {
            if (Initialzed == false)
            {
                OnInit();
                Initialzed = true;
            }
        }
        void ICanInit.DeInit()
        {
            if (Initialzed)
            {
                OnDeInit();
                Initialzed = false;
            }
        }
        protected abstract void OnInit();
        protected abstract void OnDeInit();

        public bool Initialzed
        {
            get => m_initialzed;
            set => m_initialzed = value;
        }
        public bool LazyInit
        {
            get => m_lazyInit;
        }
    }
    #endregion




    #region Command
    public interface ICmd : ICanGetModel, ICanGetSystem, ICanGetUtility, ICanSendCmd, ICanSendEvent, ICanSendQuery
    {
        void Do();
    }

    public interface ICmd<TArgs> : ICanGetModel, ICanGetSystem, ICanGetUtility, ICanSendCmd, ICanSendEvent, ICanSendQuery
    {
        void Do(TArgs info);
    }
    #endregion


    #region Query
    public interface IQuery<TResult> : ICanGetModel, ICanGetSystem, ICanSendQuery
    {
        TResult Do();
    }
    public interface IQuery<TArgs, TResult> : ICanGetModel, ICanGetSystem, ICanSendQuery
    {
        TResult Do(TArgs info);
    }
    #endregion



    #region Rule
    public interface IBelong2ModelHub { IModelHub GetModelHub(); }
    public interface ICanSetModelHub { void SetModelHub(IModelHub modelHub); }
    public interface ICanGetModel : IBelong2ModelHub { }
    public static class CanGetModelHubExtension
    {
        public static M GetModel<M>(this ICanGetModel self) where M : class, IModel => self.GetModelHub().GetModel<M>();
    }
    public interface ICanGetSystem : IBelong2ModelHub { }
    public static class CanGetSystemExtension
    {
        public static S GetSystem<S>(this ICanGetSystem self) where S : class, ISystem => self.GetModelHub().GetSystem<S>();
    }
    public interface ICanGetUtility : IBelong2ModelHub { }
    public static class CanGetUtilityExtension
    {
        public static U GetUtility<U>(this ICanGetUtility self) where U : class, IUtility => self.GetModelHub().GetUtility<U>();
    }
    public interface ICanListenEvent : IBelong2ModelHub { }
    public static class CanListenEventExtension
    {
        //IConvertible
        public static IRemover AddEvent<E>(this ICanListenEvent self, Action onEvent) where E : struct => self.GetModelHub().AddEvent<E>(onEvent);
        public static IRemover AddEvent<E>(this ICanListenEvent self, Action<E> onEvent) where E : struct => self.GetModelHub().AddEvent<E>(onEvent);
        public static IRemover AddEvent<E>(this ICanListenEvent self, E type, Action onEvent) where E : IConvertible => self.GetModelHub().AddEvent<E>(type, onEvent);
        public static IRemover AddEvent<E, TARgs>(this ICanListenEvent self, E type, Action<TARgs> onEvent) where E : IConvertible => self.GetModelHub().AddEvent<E, TARgs>(type, onEvent);
        public static void RmvEvent<E>(this ICanListenEvent self, Action onEvent) where E : struct => self.GetModelHub().RmvEvent<E>(onEvent);
        public static void RmvEvent<E>(this ICanListenEvent self, Action<E> onEvent) where E : struct => self.GetModelHub().RmvEvent<E>(onEvent);
        public static void RmvEvent<E>(this ICanListenEvent self, E type, Action onEvent) where E : IConvertible => self.GetModelHub().RmvEvent<E>(type, onEvent);
        public static void RmvEvent<E, TARgs>(this ICanListenEvent self, E type, Action<TARgs> onEvent) where E : IConvertible => self.GetModelHub().RmvEvent<E, TARgs>(type, onEvent);
    }
    public interface ICanSendEvent : IBelong2ModelHub { }
    public static class CanSendEventExtension
    {
        public static void SendEvent<E>(this ICanSendEvent self) where E : struct => self.GetModelHub().SendEvent<E>();
        public static void SendEvent<E>(this ICanSendEvent self, E args) where E : struct => self.GetModelHub().SendEvent<E>(args);
        public static void EnumEvent<E>(this ICanSendEvent self, E type) where E : IConvertible => self.GetModelHub().EnumEvent<E>(type);
        public static void EnumEvent<E, TArgs>(this ICanSendEvent self, E type, TArgs args) where E : IConvertible => self.GetModelHub().EnumEvent<E, TArgs>(type, args);
    }

    public interface ICanSendCmd : IBelong2ModelHub { }
    public static class CanSendCommandExtensino
    {
        public static void SendCmd<C>(this ICanSendCmd self, C cmd) where C : ICmd => self.GetModelHub().SendCmd<C>(cmd);
        public static void SendCmd<C>(this ICanSendCmd self) where C : struct, ICmd => self.GetModelHub().SendCmd<C>();
        public static void SendCmd<C, TArgs>(this ICanSendCmd self, C cmd, TArgs args) where C : ICmd<TArgs> => self.GetModelHub().SendCmd<C, TArgs>(cmd, args);
    }

    public interface ICanSendQuery : IBelong2ModelHub { }
    public static class CanSendQueryExtension
    {
        public static TResult SendQuery<Q, TResult>(this ICanSendQuery self) where Q : struct, IQuery<TResult> => self.GetModelHub().SendQuery<Q, TResult>();
        public static TResult SendQuery<Q, TArgs, TResult>(this ICanSendQuery self, TArgs args) where Q : struct, IQuery<TArgs, TResult> => self.GetModelHub().SendQuery<Q, TArgs, TResult>(args);
    }

    public interface ICanInit
    {
        /// <summary>
        /// true懒汉初始化
        /// </summary>
        bool LazyInit { get; }
        /// <summary>
        /// true已经初始过
        /// </summary>
        bool Initialzed { get; set; }
        void Init();
        void DeInit();
    }

    #endregion


    #region Remover
    public interface IRemoverLst
    {
        List<IRemover> RemoverLst { get; }
    }

    public static class RemoverLstExtension
    {
        public static void AddToRemoverLst(this IRemover self, IRemoverLst removerLst)
            => removerLst.RemoverLst.Add(self);

        public static void DoRemoverLst(this IRemoverLst self)
        {
            if (self.RemoverLst != null)
            {
                for (int i = 0; i < self.RemoverLst.Count; i++)
                {
                    self.RemoverLst[i]?.Do();
                }
                self.RemoverLst.Clear();
            }
        }
    }



    public interface IRemover { void Do(); }

    // 实现自身移除委托
    public struct CustomRemover : IRemover
    {
        private Action m_removeCallback { get; set; }
        public CustomRemover(Action onRemoveCallback) => this.m_removeCallback = onRemoveCallback;
        public void Do()
        {
            m_removeCallback?.Invoke();
            m_removeCallback = null;
        }
    }


#if UNITY_5_6_OR_NEWER
    public abstract class Remover : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IRemover> m_RemoveSet = new HashSet<IRemover>();
        public IRemover AddRemover(IRemover rmv)
        {
            m_RemoveSet.Add(rmv);
            return rmv;
        }

        public void RmvRemover(IRemover rmv) => m_RemoveSet.Remove(rmv);

        public void DoRemover()
        {
            foreach (var item in m_RemoveSet)
            {
                item?.Do();
            }
            m_RemoveSet.Clear();
        }
    }

    public class RemoverTriggerWhenDestroy : Remover
    {
        private void OnDestroy()
        {
            DoRemover();
        }
    }
    public class RemoverTriggerWhenDisable : Remover
    {
        private void OnDisable()
        {
            DoRemover();
        }
    }
#endif

    public static class RemoverExtension
    {
#if UNITY_5_6_OR_NEWER
        static T GetOrAddComponent<T>(UnityEngine.GameObject gameObject) where T : UnityEngine.Component
        {
            var trigger = gameObject.GetComponent<T>();
            if (!trigger)
            {
                trigger = gameObject.AddComponent<T>();
            }
            return trigger;
        }


        public static IRemover RemoveWhenGameObjectDestroyed(this IRemover self, UnityEngine.GameObject gameObject)
            => GetOrAddComponent<RemoverTriggerWhenDestroy>(gameObject).AddRemover(self);

        public static IRemover RemoveWhenGameObjectDestroyed<T>(this IRemover self, T component)
            where T : UnityEngine.Component
            => self.RemoveWhenGameObjectDestroyed(component.gameObject);

        public static IRemover RemoveWhenDisabled(this IRemover self, UnityEngine.GameObject gameObject)
            => GetOrAddComponent<RemoverTriggerWhenDisable>(gameObject).AddRemover(self);

        public static IRemover RemoveWhenDisabled<T>(this IRemover self, T component)
            where T : UnityEngine.Component
            => self.RemoveWhenDisabled(component.gameObject);

#endif
    }


    #endregion



}






