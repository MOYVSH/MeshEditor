using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HEX
{
    public struct HexEvenQ
    {
        public int col;
        public int row;

        public HexEvenQ(int inCol, int inRow)
        {
            this.col = inCol;
            this.row = inRow;
        }
        public HexAxial EvenQ2Axial()
        {
            int q = col;
            int r = row - (col + (col & 1)) / 2;

            return new HexAxial(q, r);
        }
 
        public HexEvenQ GetNeighbor(int dir)
        {
            return EvenQ2Axial().GetNeighbor(dir).Axial2EvenQ();
        }

        #region 运算符重写
        public static string ToString(HexEvenQ a)
        {
            return "(" + a.col + "," + a.row + "+)";
        }
        public static bool operator ==(HexEvenQ a, HexEvenQ b)
        {
            return a.row == b.row && a.col == b.col;
        }
        public static bool operator !=(HexEvenQ a, HexEvenQ b)
        {
            return a.row != b.row || a.col != b.col;
        }
        public static HexEvenQ operator +(HexEvenQ a, HexEvenQ b)
        {
            return new HexEvenQ(a.row + b.row, a.col + b.col);
        }
        public static HexEvenQ operator -(HexEvenQ a, HexEvenQ b)
        {
            return new HexEvenQ(a.row - b.row, a.col - b.col);
        }
        public static HexEvenQ operator -(HexEvenQ a)
        {
            return new HexEvenQ(-a.row, -a.row);
        }
        #endregion
    }
    public static class MethondEvenQ
    {
        /// <summary>
        /// 用于坐标转换EvenQ2Axial
        /// </summary>
        /// <param name="EvenQ"></param>
        /// <returns></returns>
        public static HexAxial EvenQ2Axial(this HexEvenQ EvenQ)
        {
            int q = EvenQ.col;
            int r = EvenQ.row - (EvenQ.col + (EvenQ.col & 1)) / 2;

            return new HexAxial(q, r);
        }
    }
}

