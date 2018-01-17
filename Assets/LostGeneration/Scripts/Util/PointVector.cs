using UnityEngine;
using LostGen.Model;

namespace LostGen.Util {
    public static class PointVector {
        public static Vector3 Round(Vector3 v) {
            return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        }

        public static Point ToPoint(Vector3 v) {
            return new Point(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector3 ToVector(Point p) {
            return new Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }
    }
}