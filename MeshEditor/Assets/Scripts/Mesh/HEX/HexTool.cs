using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HEX 
{
    public static class HexTool
    {
        public static HexData HexTemplate => _hexTemplate;
        private static HexData _hexTemplate = new HexData(Vector3.zero, new HexEvenQ(0, 0));
        public static Vector3 EvenQ2WorldSpace(HexEvenQ evenQ)
        {
            return HexTemplate.EvenQ2WorldSpace(evenQ, 0.2f);
        }
        public static HexAxial WorldPos2Axial(Vector3 pos)
        {
            return HexTemplate.WorldPos2Axial(pos,0.2f);
        }
    }
}

