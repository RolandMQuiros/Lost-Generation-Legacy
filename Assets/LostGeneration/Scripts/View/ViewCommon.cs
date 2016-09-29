using UnityEngine;
using System.Collections;
using LostGen;

public static class ViewCommon {
    public static Vector3 PointToVector3(Point point) {
        return new Vector3(-point.X, 0f, point.Y);
    }
}
