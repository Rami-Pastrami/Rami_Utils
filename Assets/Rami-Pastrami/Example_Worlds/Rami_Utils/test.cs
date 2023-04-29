
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Rami;

public class test : UdonSharpBehaviour
{
    void Start()
    {
        float[] a = {1.0f, 2.0f, 3.0f};
        Debug.Log(Rami_Utils.Array2CSV(a));
    }
}
