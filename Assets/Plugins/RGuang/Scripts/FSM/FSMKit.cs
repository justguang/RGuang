using System;
using System.Collections.Generic;

namespace RGuang
{
    public interface IState
    {
        bool Condition();
        void Enter();
        void Exit();
        void OnGUI();
        void Update();
        void FixedUpdate();

    }

    /// <summary>
    /// 简易状态机
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSM<T>
    {
        /// <summary>
        /// 状态缓存
        /// key：T = 状态ID
        /// value：IState = 状态
        /// </summary>
        protected Dictionary<T, IState> m_States = new Dictionary<T, IState>();

        /// <summary>
        /// 状态切换
        /// args1 = 切换前的状态ID
        /// args2 = 切换后的状态ID
        /// </summary>
        private Action<T, T> m_OnStateChanged = (fromState, toState) => { };

        /// <summary>
        /// 添加一个状态
        /// </summary>
        /// <param name="id">状态id</param>
        /// <param name="state">状态</param>
        public void AddState(T id, IState state) => m_States.Add(id, state);

        /// <summary>
        /// 添加一个状态
        /// </summary>
        /// <param name="id">状态id</param>
        /// <param name="state">状态</param>
        /// <returns>添加成功返回true，如果id已存在则不添加并返回false</returns>
        public bool TryAddState(T id, IState state) => m_States.TryAdd(id, state);

        /// <summary>
        /// 检测是否存在指定状态
        /// </summary>
        /// <param name="id">指定状态id</param>
        /// <param name="existState">获取到存在的状态</param>
        /// <returns>如果存在返回true，否则返回false</returns>
        public bool HasState(T id, out IState existState)
        {
            if (m_States.TryGetValue(id, out existState))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public IState CurrentState => m_CurrentState;
        private IState m_CurrentState;

        /// <summary>
        /// 当前状态ID
        /// </summary>
        public T CurrentStateId => m_CurrentStateId;
        private T m_CurrentStateId;

        /// <summary>
        /// 上一个状态ID
        /// </summary>
        public T PreviousStateId { get; private set; }

        /// <summary>
        /// 当前状态经过的帧数
        /// </summary>
        public long FrameCountOfCurrentState = 1;
        /// <summary>
        /// 当前状态经过的秒数
        /// </summary>
        public float SecondsOfCurrentState = 0.0f;

        /// <summary>
        /// 启动状态
        /// ps：如果指定的状态ID不存在，则不做任何操作
        /// </summary>
        /// <param name="stateID">状态ID</param>
        public void StartState(T stateID)
        {
            if (m_States.TryGetValue(stateID, out var state))
            {
                PreviousStateId = stateID;
                m_CurrentState = state;
                m_CurrentStateId = stateID;
                FrameCountOfCurrentState = 1;
                SecondsOfCurrentState = 0.0f;
                state.Enter();
            }
        }
        /// <summary>
        /// 状态切换到目标状态
        /// ps：如果当前状态等于目标状态 || 不存在的目标状态ID || 当前状态==null || 目标状态的Condition返回false，则不做任何操作
        /// </summary>
        /// <param name="targetID">从当前状态要切换的目标状态ID</param>
        public void ChangeState(T targetID)
        {
            if (targetID.Equals(CurrentStateId)) return;

            if (m_States.TryGetValue(targetID, out var state))
            {
                if (m_CurrentState != null && state.Condition())
                {
                    m_CurrentState.Exit();
                    PreviousStateId = m_CurrentStateId;
                    m_CurrentState = state;
                    m_CurrentStateId = targetID;
                    m_OnStateChanged?.Invoke(PreviousStateId, m_CurrentStateId);
                    FrameCountOfCurrentState = 1;
                    SecondsOfCurrentState = 0.0f;
                    m_CurrentState.Enter();
                }
            }
        }

        /// <summary>
        /// 设置回调：状态变更时回调
        /// </summary>
        /// <param name="onStateChangedCallback"></param>
        public void SetCallbackOnStateChanged(Action<T, T> onStateChangedCallback) => m_OnStateChanged += onStateChangedCallback;

        /// <summary>
        /// 撤销回调：状态变更时回调
        /// </summary>
        /// <param name="onStateChangedCallback"></param>
        public void UnSetCallbackOnStateChanged(Action<T, T> onStateChangedCallback) => m_OnStateChanged -= onStateChangedCallback;


        public void OnGUI()
        {
            m_CurrentState?.OnGUI();
        }
        public void Update()
        {
            m_CurrentState?.Update();
            FrameCountOfCurrentState++;
            SecondsOfCurrentState += UnityEngine.Time.deltaTime;
        }

        public void FixedUpdate()
        {
            m_CurrentState?.FixedUpdate();
        }

        public void Clear()
        {
            m_CurrentState = null;
            m_CurrentStateId = default;
            m_States.Clear();
            m_OnStateChanged = (fromState, toState) => { };
        }


    }


    /// <summary>
    /// 抽象状态
    /// </summary>
    /// <typeparam name="TStateId"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public abstract class AbstractState<TStateId, TTarget> : IState
    {
        protected FSM<TStateId> m_FSM;
        protected TTarget m_Target;
        public AbstractState(FSM<TStateId> fsm, TTarget target)
        {
            m_FSM = fsm;
            m_Target = target;
        }

        #region Interface IState Implement
        bool IState.Condition() => OnCondition();
        void IState.Enter() => OnEnter();
        void IState.Exit() => OnExit();
        void IState.OnGUI() => OnGUI();
        void IState.Update() => OnUpdate();
        void IState.FixedUpdate() => OnFixedUpdate();
        #endregion

        protected virtual bool OnCondition() => true;
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnGUI() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }

    }


    /// <summary>
    /// 自定义状态
    /// </summary>
    public class CustomState : IState
    {
        private Func<bool> m_onCondition;
        private Action m_OnEnter;
        private Action m_OnExit;
        private Action m_OnGUI;
        private Action m_OnUpdate;
        private Action m_OnFixedUpdate;

        public CustomState() { }
        public CustomState(Func<bool> onCondition,
                            Action onEnter,
                            Action onExit,
                            Action onGUI,
                            Action onUpdate,
                            Action onFixedUpdate)
        {
            m_onCondition += onCondition;
            m_OnEnter += onEnter;
            m_OnExit += onExit;
            m_OnGUI += onGUI;
            m_OnUpdate += onUpdate;
            m_OnFixedUpdate += onFixedUpdate;
        }


        #region 设置回调
        public CustomState SetCallbackOnCondition(Func<bool> onConditionrCallback)
        {
            m_onCondition += onConditionrCallback;
            return this;
        }
        public CustomState SetCallbackOnEnter(Action onEnterCallback)
        {
            m_OnEnter += onEnterCallback;
            return this;
        }
        public CustomState SetCallbackOnExit(Action onExitCallback)
        {
            m_OnExit += onExitCallback;
            return this;
        }
        public CustomState SetCallbackOnGUI(Action onGUICallback)
        {
            m_OnGUI += onGUICallback;
            return this;
        }
        public CustomState SetCallbackOnUpdate(Action onUpdateCallback)
        {
            m_OnUpdate += onUpdateCallback;
            return this;
        }
        public CustomState SetCallbackOnFixedUpdate(Action onFixedUpdateCallback)
        {
            m_OnFixedUpdate += onFixedUpdateCallback;
            return this;
        }
        public CustomState UnSetCallbackOnCondition(Func<bool> onConditionrCallback)
        {
            m_onCondition -= onConditionrCallback;
            return this;
        }
        public CustomState UnSetCallbackOnEnter(Action onEnterCallback)
        {
            m_OnEnter -= onEnterCallback;
            return this;
        }
        public CustomState UnSetCallbackOnExit(Action onExitCallback)
        {
            m_OnExit -= onExitCallback;
            return this;
        }
        public CustomState UnSetCallbackOnGUI(Action onGUICallback)
        {
            m_OnGUI -= onGUICallback;
            return this;
        }
        public CustomState UnSetCallbackOnUpdate(Action onUpdateCallback)
        {
            m_OnUpdate -= onUpdateCallback;
            return this;
        }
        public CustomState UnSetCallbackOnFixedUpdate(Action onFixedUpdateCallback)
        {
            m_OnFixedUpdate -= onFixedUpdateCallback;
            return this;
        }
        #endregion


        public bool Condition()
        {
            var result = m_onCondition?.Invoke();
            return result == null || result.Value;
        }

        public void Enter()
        {
            m_OnEnter?.Invoke();
        }

        public void Exit()
        {
            m_OnExit?.Invoke();
        }

        public void Update()
        {
            m_OnUpdate?.Invoke();
        }

        public void FixedUpdate()
        {
            m_OnFixedUpdate?.Invoke();
        }

        public void OnGUI()
        {
            m_OnGUI?.Invoke();
        }

    }


}
