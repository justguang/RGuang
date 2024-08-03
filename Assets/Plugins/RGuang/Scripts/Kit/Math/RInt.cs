using System;


namespace RGuang.Kit
{
    /// <summary>
    /// 定点数运算
    /// 解决浮点类非精度数带来的误差
    /// </summary>
    public struct RInt : IEquatable<RInt>, IComparable<RInt>
    {
        private long scaledValue;
        /// <summary>
        /// 存储的值，非实际值，用于逻辑运算
        /// </summary>
        public long ScaledValue
        {
            get { return scaledValue; }
            set { scaledValue = value; }
        }

        /*
         
        win32:
        0000 0000 0000 0000  0000 0000 0000 0001 => 1

        1 << 10

        0000 0000 0000 0000  0000 0100 0000 0000 => 1024

          
         */

        /// <summary>
        /// 位移计数
        /// </summary>
        public const int BIT_MOVE_COUNT = 10;
        /// <summary>
        /// RInt 存储值的倍数
        /// 如：
        ///     0.1f用RInt储存就是 0.1f*倍数(MULTIPLIER_FACTOR) = RInt实际存储的值，
        ///     用 RawInt可获取，用RawFloat获取原本的值
        /// </summary>
        public const long MULTIPLIER_FACTOR = 1 << BIT_MOVE_COUNT;

        public static readonly RInt zero = new RInt(0);
        public static readonly RInt one = new RInt(1);

        #region 构造函数
        //供给内部使用，scaledValue已经缩放后的数据
        private RInt(long scaledValue)
        {
            this.scaledValue = scaledValue;
        }
        /// <summary>
        /// 存 double 
        /// </summary>
        /// <param name="val"></param>
        public RInt(double val)
        {
            scaledValue = (long)Math.Round(val * MULTIPLIER_FACTOR, mode: MidpointRounding.AwayFromZero);
        }
        /// <summary>
        /// 存 float
        /// </summary>
        /// <param name="val"></param>
        public RInt(float val)
        {
            scaledValue = (long)Math.Round(val * MULTIPLIER_FACTOR, mode: MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 存 int
        /// </summary>
        /// <param name="val"></param>
        public RInt(int val)
        {
            scaledValue = val * MULTIPLIER_FACTOR;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public RInt(RInt val)
        {
            scaledValue = val.scaledValue;
        }
        #endregion

        #region 类型转换
        //float损失精度，必须显示转换
        public static explicit operator RInt(float f)
        {
            return new RInt(f);
        }
        //double损失精度，必须显示转换
        public static explicit operator RInt(double d)
        {
            return new RInt(d);
        }

        //int不损失精度，可以隐式转换
        public static implicit operator RInt(int i)
        {
            return new RInt(i);
        }
        #endregion

        #region 运算符
        public static RInt operator +(RInt a, RInt b)
        {
            return new RInt(a.scaledValue + b.scaledValue);
        }

        public static RInt operator -(RInt a, RInt b)
        {
            return new RInt(a.scaledValue - b.scaledValue);
        }

        public static RInt operator *(RInt a, RInt b)
        {
            long value = a.scaledValue * b.scaledValue;
            if (value >= 0)
            {
                value >>= BIT_MOVE_COUNT;
            }
            else
            {
                value = -(-value >> BIT_MOVE_COUNT);
            }
            return new RInt(value);
        }

        public static RInt operator /(RInt a, RInt b)
        {
            if (b.scaledValue == 0)
            {
                throw new Exception("除数不能为零.");
            }

            return new RInt((a.scaledValue << BIT_MOVE_COUNT) / b.scaledValue);
        }

        public static RInt operator -(RInt value)
        {
            return new RInt(-value.scaledValue);
        }


        public static bool operator ==(RInt a, RInt b)
        {
            return a.scaledValue == b.scaledValue;
        }

        public static bool operator !=(RInt a, RInt b)
        {
            return a.scaledValue != b.scaledValue;
        }

        public static bool operator >(RInt a, RInt b)
        {
            return a.scaledValue > b.scaledValue;
        }

        public static bool operator <(RInt a, RInt b)
        {
            return a.scaledValue < b.scaledValue;
        }

        public static bool operator >=(RInt a, RInt b)
        {
            return a.scaledValue >= b.scaledValue;
        }

        public static bool operator <=(RInt a, RInt b)
        {
            return a.scaledValue <= b.scaledValue;
        }

        public static RInt operator >>(RInt value, int moveCount)
        {
            if (value.scaledValue >= 0)
            {
                return new RInt(value.scaledValue >> moveCount);
            }
            else
            {
                return new RInt(-(-value.scaledValue >> moveCount));
            }
        }

        public static RInt operator <<(RInt value, int moveCount)
        {
            return new RInt(value.scaledValue << moveCount);
        }
        #endregion

        #region 值获取
        /// <summary>
        /// 获取实际的Float值
        /// 注意：转换完成后的值，不可再参与逻辑运算
        /// </summary>
        public float RawFloat
        {
            get { return scaledValue * 1.0f / MULTIPLIER_FACTOR; }
        }

        /// <summary>
        /// 获取实际的Double值
        /// 注意：转换完成后的值，不可再参与逻辑运算
        /// </summary>
        public double RawDouble
        {
            get { return scaledValue * 1.0d / MULTIPLIER_FACTOR; }
        }

        /// <summary>
        /// 获取实际的Int值，不四舍五入舍去小数部分
        /// 注意：转换完成后的值，不可再参与逻辑运算
        /// </summary>
        public int RawInt
        {
            get
            {
                if (scaledValue >= 0)
                {
                    return (int)(scaledValue >> BIT_MOVE_COUNT);

                }
                else
                {
                    return -(int)(-scaledValue >> BIT_MOVE_COUNT);
                }
            }
        }
        #endregion

        #region 其他操作
        public bool Equals(RInt other)
        {
            return scaledValue == other.scaledValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            RInt vInt = (RInt)obj;
            return scaledValue == vInt.scaledValue;
        }

        public int CompareTo(RInt other)
        {
            return scaledValue.CompareTo(other.scaledValue);
        }

        public override int GetHashCode()
        {
            return scaledValue.GetHashCode();
        }

        public override string ToString()
        {
            return RawFloat.ToString();
        }

        public string ToString(string format)
        {
            return RawFloat.ToString(format);
        }
        #endregion


        #region static func 
        /// <summary>
        /// 如果值小于(大于) 最小(最大)限定值 就返回最小(最大)限定值，否直返回值本身
        /// 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">限定最小值</param>
        /// <param name="max">限定最大值</param>
        /// <returns>如果值小于(大于) 最小(最大)限定值 就返回最小(最大)限定值，否直返回值本身</returns>
        public static RInt Clamp(RInt value, RInt min, RInt max)
        {
            if (value < min) return min;

            if (value > max) return max;

            return value;
        }
        /// <summary>
        /// 如果值小于限定最小值返回限定最小值，否则返回值本身
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">限定最小值</param>
        /// <returns>如果值小于限定最小值返回限定最小值，否则返回值本身</returns>
        public static RInt Min(RInt value, RInt min)
        {
            if (value < min) return min;
            return value;
        }
        /// <summary>
        /// 如果值大于限定最大值返回限定最大值，否则返回值本身
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">限定最大值</param>
        /// <returns>如果值大于限定最大值返回限定最小值，否则返回值本身</returns>
        public static RInt Max(RInt value, RInt max)
        {
            if (value > max) return max;
            return value;
        }

        #endregion

    }
}
