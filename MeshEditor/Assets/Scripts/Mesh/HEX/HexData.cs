using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HEX
{
    
    public class HexData
    {
        float sqrt3 = 1.7320508f;
        public float w => sqrt3 * r / 2;//六边形中心点到边的距离
        public float r = 1;//边长
        public float height = 0;
        public bool canWalk;
        public HexEvenQ EvenQ;
        public HexAxial Axial;

        public Vector3 CenterPoint;
        public List<Vector3> Points = new List<Vector3>();

        private Vector3 StartPoint;
        #region 生成六边形数据
        public HexData(Vector3 StartPoint)
        {
            this.StartPoint = StartPoint;
        }
        /// <summary>
        /// StartPoint应该手动限定为左边的点，之后逆时针旋转
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="pos"></param>
        public HexData(Vector3 StartPoint, HexEvenQ pos = default) : this(StartPoint)
        {
            this.EvenQ = pos;
            this.Axial = pos.EvenQ2Axial();
            AddPoints(StartPoint);
        }
        public HexData(Vector3 StartPoint, HexAxial pos = default, float radius = 1) : this(StartPoint)
        {
            this.Axial = pos;
            this.EvenQ = pos.Axial2EvenQ();
            AddPoints(StartPoint);
        }
        public void AddPoints(Vector3 StartPoint)
        {
            this.CenterPoint = StartPoint + new Vector3(r, height, 0);
            this.Points.Add(StartPoint + new Vector3(0, height, 0));
            this.Points.Add(StartPoint + new Vector3(.5f * r, height, -w));
            this.Points.Add(StartPoint + new Vector3(1.5f * r, height, -w));
            this.Points.Add(StartPoint + new Vector3(2.0f * r, height, 0));
            this.Points.Add(StartPoint + new Vector3(1.5f * r, height, w));
            this.Points.Add(StartPoint + new Vector3(.5f * r, height, w));
        }
        #endregion

        public void RefreshPoints()
        {
            Points.Clear();
            AddPoints(StartPoint);
        }

        /// <summary>
        /// 求evenQ对应的中心点
        /// </summary>
        /// <param name="evenQ"></param>
        /// <returns></returns>
        public Vector3 EvenQ2WorldSpace(HexEvenQ evenQ)
        {
            float x = r * 1.5f * evenQ.col;
            //由于默认设定为EvenQ是从左下角为0点，向上 row-1 所以需要整体添加负号来计算
            float z = -r * sqrt3 * (evenQ.row - 0.5f * (evenQ.col & 1));//先向上走再向下走
            return new Vector3(x,height,z);
        }
        /// <summary>
        /// 求Axial对应的中心点
        /// </summary>
        /// <param name="hexAxial"></param>
        /// <returns></returns>
        public Vector3 Axial2WorldSpace(HexAxial hexAxial)
        {
            return EvenQ2WorldSpace(hexAxial.Axial2EvenQ());
        }
        /// <summary>
        /// 求evenQ对应的中心点 带偏移
        /// </summary>
        /// <param name="evenQ"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Vector3 EvenQ2WorldSpace(HexEvenQ evenQ,float offset)
        {
            float x = (r + offset) * 1.5f * evenQ.col;
            //由于默认设定为EvenQ是从左下角为0点，向上 row-1 所以需要整体添加负号来计算
            float z = -(r + offset) * sqrt3 * (evenQ.row - 0.5f * (evenQ.col & 1));//先向上走再向下走
            return new Vector3(x, height, z);
        }
        /// <summary>
        /// 求Axial对应的中心点 带偏移
        /// </summary>
        /// <param name="hexAxial"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Vector3 Axial2WorldSpace(HexAxial hexAxial, float offset)
        {
            return EvenQ2WorldSpace(hexAxial.Axial2EvenQ(),offset);
        }

        public HexAxial WorldPos2Axial(Vector3 pos, float offset = 0)
        {
            float q = 2.0f / 3 * pos.x / (this.r + offset);
            float r = (-1.0f / 3 * pos.x + sqrt3 / 3 * -pos.z) / (this.r + offset);
            float s = -(q + r);
            return cube_round(q, r, s);
        }

        //float3形式的AxiAxial轴坐标 四舍五入为最近的Axial坐标 Rounding To Nearest Hex
        public HexAxial cube_round(float q , float r, float s)
        {
            var q_Round = Mathf.Round(q);
            var r_Round = Mathf.Round(r);
            var s_Round = Mathf.Round(s);

            var q_diff = Mathf.Abs(q_Round - q);
            var r_diff = Mathf.Abs(r_Round - r);
            var s_diff = Mathf.Abs(s_Round - s);
            if (q_diff > r_diff && q_diff > s_diff)
                q_Round = -(r_Round + s_Round);
            else if (r_diff > s_diff)
                r_Round = -(q_Round + s_Round);
            else
                s_Round = -(q_Round + r_Round);
            return new HexAxial((int)q_Round, (int)r_Round);
        }


        #region 两点间连线
        public float lerp(float Origin, float Target, float t)
        {
            return Origin + (Target - Origin) * t;
        }
        public Vector3 hexAxialLerp(HexAxial Origin, HexAxial Target, float t)
        {
            return new Vector3(
                lerp(Origin.q, Target.q, t),
                lerp(Origin.r, Target.r, t),
                lerp(Origin.s, Target.s, t)
                );
        }
        public List<HexAxial> HexLine(HexAxial Origin, HexAxial Target)
        {
            int Distance = HexAxial.AxialDistance(Origin, Target);
            List<HexAxial> List = new List<HexAxial>();
            for (int i = 0; i <= Distance; i++)
            {
                Vector3 temp = hexAxialLerp(Origin, Target, 1.0f / Distance * i);
                List.Add(cube_round(temp.x, temp.y, temp.z));
            }
            return List;
        }
        #endregion

        #region HexRings
        public HexAxial HexRingScale(HexAxial hexAxial,int radius)
        {
            return new HexAxial(hexAxial.q * radius, hexAxial.r * radius);
        }

        List<HexAxial> ringResult = new List<HexAxial>();
        List<HexAxial> rangeResult = new List<HexAxial>();
        //single
        public List<HexAxial> HexAxialRing(HexAxial center,int radius)
        {
            ringResult.Clear();
            if (radius ==0)
            {
                ringResult.Add(center);
                return ringResult;
            }
            else
            {
                var hex = center + HexRingScale(center.getNeighborOffsets(0), radius);
                for (int i = 2; i < 8; i++)
                {
                    for (int j = 0; j < radius; j++)
                    {
                        ringResult.Add(hex);
                        hex = hex.GetNeighbor(i);
                    }
                }
            }
            return ringResult;
        }
        //Spiral
        public List<HexAxial> HexAxialRingRange(HexAxial center, int RingCount)
        {
            rangeResult.Clear();
            for (int i = 0; i < RingCount; i++)
            {
                rangeResult.AddRange(HexAxialRing(center, i));
            }
            return rangeResult;
        }
        public List<HexAxial> HexAxialRingRange(int RingCount, HexAxial center)
        {
            for (int q = -RingCount; q <= RingCount; q++)
            {
                for (int r = Mathf.Max(-RingCount, -q - RingCount); r <= Mathf.Min(RingCount, -q + RingCount); r++)
                {
                    rangeResult.Add(new HexAxial(q, r) + center);
                }
            }
            return rangeResult;
        }
        #endregion
    }
}
