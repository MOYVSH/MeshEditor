using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HEX
{
    [System.Serializable]
    public class HexMap
    {
        [System.Serializable]
        public class Region
        {
            public string name;
            public List<HexEvenQ> coords = new List<HexEvenQ>();
        }

        [System.Serializable]
        public struct Vector
        {
            public Vector(float _x, float _y, float _z) { x = _x; y = _y; z = _z; }
            public float x;
            public float y;
            public float z;
        }
        /// <summary>
        /// Number cells of map on X
        /// </summary>
        public int width;

        /// <summary>
        /// Number cells of map on Y
        /// </summary>
        public int height;

        /// <summary>
        /// Cells.
        /// </summary>
        public List<HexData> cells = new List<HexData>();

        /// <summary>
        /// Regions.
        /// </summary>
        public List<Region> regions = new List<Region>();

        public HexMap() { }
        public HexMap(int w, int h)
        {
            width = w;
            height = h;
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    var evenQ = new HexEvenQ(col, -row);
                    Vector3 startPos = HexTool.EvenQ2WorldSpace(evenQ) + Vector3.left;
                    cells.Add(new HexData(startPos, evenQ));
                }
            }
        }


        /// <summary>
        /// 标判断偏移坐是否合法
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool isValidCell(HexEvenQ c) { return (c.col >= 0 && c.col < width && c.row <= 0 && -c.row < height); }


        /// <summary>
        /// 将EvenQ规则的偏移坐标转化成数组中的下标
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int getCellArrayId(HexEvenQ c) { return c.col * width - c.row; }

        public HexData getCell(int arrayId)
        {
            if (arrayId < 0 || arrayId >= cells.Count)
                return null;
            return cells[arrayId];
        }

        public HexData getCell(HexEvenQ c)
        {
            if (!isValidCell(c))
                return null;
            return cells[getCellArrayId(c)];
        }
    }
}

