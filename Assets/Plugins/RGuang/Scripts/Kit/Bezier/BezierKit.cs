using System;
using System.Collections.Generic;
using UnityEngine;

namespace RGuang.Kit
{
    /// <summary>
    /// 贝塞er
    /// 
    /// </summary>
    public static class BezierKit
    {
        /// <summary>
        /// 线性公式
        /// B(t) = P0 + (P1-P0) * t = (1-t) * P0 + t * P1
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 Bezier(Vector3 p0, Vector3 p1, float t)
        {
            return (1 - t) * p0 + t * p1;
        }
        /// <summary>
        /// 二次方公式
        /// P0P1 = (1-t) * P0 + t * P1;
        /// P1P2 = (1-t) * P1 + t * P2;
        /// 
        /// B(t) = (1-t) * P0P1 + t * P1P2
        /// 
        /// 简化：B(t) = (1-t)^2 * P0 + 2 * t * (1-t) * P1 + t^2 * P2
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            Vector3 p0p1 = (1 - t) * p0 + t * p1;
            Vector3 p1p2 = (1 - t) * p1 + t * p2;
            Vector3 result = (1 - t) * p0p1 + t * p1p2;
            return result;
        }
        /// <summary>
        /// 三次方公式
        /// 
        /// P0P1 = (1-t) * P0 + t * P1;
        /// P1P2 = (1-t) * P1 + t * P2;
        /// P2P3 = (1-t) * P2 + t * P3;
        /// 
        /// P0P1P2 = (1-t) * P0P1 + t * P1P2
        /// P1P2P3 = (1-t) * P1P2 + t * P2P3
        /// 
        /// B(t) = (1-t) * P0P1P2 + t * P1P2P3
        /// 
        /// 简化：B(t) = P0 * (1-t)^3 + 3 * P1 * t * (1-t)^2 + 3 * P2 * t^2 * (1-t) + P3 * t^3
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 p0p1 = (1 - t) * p0 + t * p1;
            Vector3 p1p2 = (1 - t) * p1 + t * p2;
            Vector3 p2p3 = (1 - t) * p2 + t * p3;

            Vector3 p0p1p2 = (1 - t) * p0p1 + t * p1p2;
            Vector3 p1p2p3 = (1 - t) * p1p2 + t * p2p3;

            Vector3 result = (1 - t) * p0p1p2 + t * p1p2p3;
            return result;
        }


        /// <summary>
        /// Vector2 三阶
        /// 
        /// B(t) = P0 * (1-t)^3 + 3 * P1 * t * (1-t)^2 + 3 * P2 * t^2 * (1-t) + P3 * t^3
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static Vector2 Bezier2D(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 res = new Vector2();

            float u = 1 - t;

            float x0 = p0.x * u * u * u;
            float x1 = p1.x * u * u * t * 3;
            float x2 = p2.x * u * t * t * 3;
            float x3 = p3.x * t * t * t;
            res.x = x0 + x1 + x2 + x3;


            float y0 = p0.y * u * u * u;
            float y1 = p1.y * u * u * t * 3;
            float y2 = p2.y * u * t * t * 3;
            float y3 = p3.y * t * t * t;
            res.y = y0 + y1 + y2 + y3;

            return res;
        }


        /// <summary>
        /// [三阶] Bezier
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="stepNum">需要获取Vector的数量</param>
        public static void GetBezierForThirdOrder(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int stepNum, ref List<Vector2> result)
        {
            float u = 1;
            float CurveStep = 1 / (float)stepNum;
            result.Clear();
            while (u > 0)
            {
                Vector2 Point = Bezier2D(u, p0, p1, p2, p3);
                u = u - CurveStep;
                result.Add(Point);
            }
        }

    }
}
