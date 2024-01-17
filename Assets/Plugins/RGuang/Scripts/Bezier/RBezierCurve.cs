using System.Collections.Generic;
using UnityEngine;

namespace RGuang
{
    /// <summary>
    /// 贝塞尔曲线
    /// 
    /// </summary>
    public static class RBezierCurve
    {
        /// <summary>
        /// bezier
        /// </summary>
        /// <param name="t"></param>
        /// <param name="controlP">0：起始点 1：控制点1 2：控制点2 3：重点</param>
        /// <returns></returns>
        private static Vector2 ThirdOrderBezierCurve(float t, Vector2[] controlP)
        {
            Vector2 res = new Vector2();
            if (controlP.Length != 4)
            {
                Debug.LogWarning("三阶贝塞尔坐标需要4个点。");
                return res;
            }

            float u = 1 - t;

            float partx0 = controlP[0].x * u * u * u;
            float partx1 = 3 * t * u * u * controlP[1].x;
            float partx2 = 3 * t * t * u * controlP[2].x;
            float partx3 = t * t * t * controlP[3].x;
            float x = partx0 + partx1 + partx2 + partx3;
            res.x = x;

            float party0 = controlP[0].y * u * u * u;
            float party1 = 3 * t * u * u * controlP[1].y;
            float party2 = 3 * t * t * u * controlP[2].y;
            float party3 = t * t * t * controlP[3].y;
            float y = party0 + party1 + party2 + party3;
            res.y = y;

            return res;
        }

        /// <summary>
        /// [三阶] Bezier
        /// </summary>
        /// <param name="originPoint">0：起始点 1：控制点 2：终点</param>
        /// <param name="stepNum">需要获取Vector的数量</param>
        public static void GetBezierCurveForThirdOrder(Vector2[] originPoint, int stepNum, ref List<Vector2> result)
        {
            if (originPoint.Length != 4)
            {
                Debug.LogWarning("三阶贝塞尔坐标需要4个点。");
                return;
            }

            float u = 1;
            float CurveStep = 1 / (float)stepNum;
            result.Clear();
            while (u > 0)
            {
                Vector2 Point = ThirdOrderBezierCurve(u, originPoint);
                u = u - CurveStep;
                result.Add(Point);
            }
            result.Add(originPoint[0]);
        }


    }
}
