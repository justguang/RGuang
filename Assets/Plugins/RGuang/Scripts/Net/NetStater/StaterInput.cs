/// <summary>
///********************************************
/// ClassName    ：  StaterInput
/// Description  ：  状态机输入
///********************************************/
/// </summary>
using System;

namespace RGuang.Net.NetState
{
    public abstract class StaterInput { }

    public class StaterInputEnum<T> : StaterInput where T : struct
    {
        private T opEnum;
        public StaterInputEnum(T opEnum)
        {
            this.opEnum = opEnum;
        }

        public override bool Equals(object obj)
        {
            if (obj is StaterInputEnum<T>)
            {
                var ipt = obj as StaterInputEnum<T>;
                return ipt.opEnum.Equals(opEnum);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return opEnum.GetHashCode();
        }
    }

    public class StaterInputData : StaterInput
    {
        public byte[] data;
        public StaterInputData(byte[] data)
        {
            this.data = data;
        }
        public override bool Equals(object obj)
        {
            if (obj is StaterInputData)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class StaterItem<T> where T : struct
    {
        public StaterInput ipt;
        public T dstState;
        public Func<T, StaterInput, T, bool> cb;
        public StaterItem(StaterInput ipt, T dstState, Func<T, StaterInput, T, bool> cb)
        {
            this.ipt = ipt;
            this.dstState = dstState;
            this.cb = cb;
        }
    }
}
