using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// &	与	两个位都为1时，结果才为1
/// |	或 两个位都为0时，结果才为0
/// ^	异或 两个位相同为0，相异为1
/// ~   取反  0变1，1变0
/// <<	左移 各二进位全部左移若干位，高位丢弃，低位补0
/// >>	右移 各二进位全部右移若干位，对无符号数，高位补0，有符号数，各编译器处理方法不一样，有的补符号位（算术右移），有的补0（逻辑右移） 

namespace HEX
{
    public struct HexAxial
    {
        public int q;
        public int r;
        public int s;

        public HexAxial(int q, int r)
        {
            this.q = q;
            this.r = r;
            this.s = -(q+r);
        }
        public HexEvenQ Axial2EvenQ()
        {
            int col = q;
            int row = r + (q + (q & 1)) / 2;

            return new HexEvenQ(col, row);
        }

        public HexAxial getNeighborOffsets(int dir)
        {
            return neighborOffsets_[dir];
        }
        static HexAxial[] neighborOffsets_ = {
        new HexAxial(-1,1),
        new HexAxial(0,1),
        new HexAxial(1,0),
        new HexAxial(1,-1),
        new HexAxial(0,-1),
        new HexAxial(-1,0)
        };
        public HexAxial GetNeighbor(int dir)
        {
            dir = HexDir.round(dir);
            return this + neighborOffsets_[dir];
        }



        public static int AxialDistance(HexAxial a, HexAxial b)
        {
            var vec = a - b;
            return (Mathf.Abs(vec.q) + Mathf.Abs(vec.r) + Mathf.Abs(vec.s)) / 2;
        }
        #region 运算符重写
        public static bool operator ==(HexAxial a, HexAxial b)
        {
            return a.q == b.q && a.r == b.r;
        }
        public static bool operator !=(HexAxial a, HexAxial b)
        {
            return a.q != b.q || a.r != b.r;
        }
        public static HexAxial operator +(HexAxial a, HexAxial b)
        {
            return new HexAxial(a.q + b.q, a.r + b.r);
        }
        public static HexAxial operator -(HexAxial a, HexAxial b)
        {
            return new HexAxial(a.q - b.q, a.r - b.r);
        }
        public static HexAxial operator -(HexAxial a)
        {
            return new HexAxial(-a.q, -a.r);
        }
        #endregion
    }
    public static class MethondAxial
    {
        /// <summary>
        /// 用于坐标转换Axial2EvenQ
        /// </summary>
        /// <param name="Axial"></param>
        /// <returns></returns>
        public static HexEvenQ Axial2EvenQ(this HexAxial Axial)
        {
            int col = Axial.q;
            int row = Axial.r + (Axial.q + (Axial.q & 1)) / 2;

            return new HexEvenQ(col, row);
        }
    }
}
