using System;
using UnityEngine;


namespace RGuang
{
    public static class RMath
    {




        #region 四舍五入
        /*
         

        在C#中，Math.Round()函数默认使用的是银行家舍入法（Banker's rounding）
        按照【银行家舍入法（Banker's rounding）】规则如下：

            1、当舍去位的数值小于5时。直接舍去。

            2、当舍去位的数值大于6时，进位加1。

            3、当舍去位的数值等于5时，分两种情况：

                （1）若5后面有其他非0数字(即5不是最后一位)时,进位加1

　               （2）若5后面只有0(即5是最后一位)时,则根据5的前一位的奇偶来判断，前一位为奇数则进位加1，为偶数则舍去。

                    遇到5需要舍去的情况只有一种，即5是最后一位有效数且前一位数是偶数。



        解决：
        使用Marh.Round 中第三个参数【MidpointRounding.AwayFromZero】枚举值

        官方链接：【https://learn.microsoft.com/zh-cn/dotnet/api/system.midpointrounding?view=net-7.0】            

        */

        /// <summary>
        /// 四舍五入
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits">保留几位小数【默认2】</param>
        /// <returns></returns>
        public static double Round(double value, int digits = 2)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        ///  四舍五入
        ///  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static decimal Round(decimal value, int digits = 2)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }
        #endregion


        #region RInt

        /// <summary>
        /// 如果值小于(大于) 最小(最大)限定值 就返回最小(最大)限定值，否直返回值本身
        /// 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">限定最小值</param>
        /// <param name="max">限定最大值</param>
        /// <returns>如果值小于(大于) 最小(最大)限定值 就返回最小(最大)限定值，否直返回值本身</returns>
        public static RInt Clamp(RInt value, RInt min, RInt max) => RInt.Clamp(value, min, max);
        /// <summary>
        /// 如果值小于限定最小值返回限定最小值，否则返回值本身
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">限定最小值</param>
        /// <returns>如果值小于限定最小值返回限定最小值，否则返回值本身</returns>
        public static RInt Min(RInt value, RInt min) => RInt.Min(value, min);
        /// <summary>
        /// 如果值大于限定最大值返回限定最大值，否则返回值本身
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">限定最大值</param>
        /// <returns>如果值大于限定最大值返回限定最小值，否则返回值本身</returns>
        public static RInt Max(RInt value, RInt max) => RInt.Max(value, max);
        #endregion

        /// <summary>
        /// 随机 1 or -1
        /// </summary>
        public static int GetOneOrMinuseOne => UnityEngine.Random.Range(0, 2) * 2 - 1;

        /// <summary>
        /// 随机 true or false
        /// </summary>
        public static bool GetTrueOrFalse => UnityEngine.Random.Range(0, 2) == 1;

    }

}
