using System;
using UnityEngine;
public static class HypUtil {
    public static float sinh(float x) {
        return 0.5f*(Mathf.Exp(x) - Mathf.Exp(-x));
    }
    public static float cosh(float x) {
        return 0.5f*(Mathf.Exp(x) + Mathf.Exp(-x));
    }
    public static Matrix4x4 BoostX(float t) {
        Matrix4x4 T = Matrix4x4.identity;
        T.SetRow(0,new Vector4(cosh(t),0,sinh(t),0));
        T.SetRow(2,new Vector4(sinh(t),0,cosh(t),0));
        return T;
    }
    public static Matrix4x4 BoostY(float t) {
        Matrix4x4 T = Matrix4x4.identity;
        T.SetRow(1,new Vector4(0,cosh(t),sinh(t),0));
        T.SetRow(2,new Vector4(0,sinh(t),cosh(t),0));
        return T;
    }
}