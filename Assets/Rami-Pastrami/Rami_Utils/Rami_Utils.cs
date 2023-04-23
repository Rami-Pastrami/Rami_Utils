// Created by Rami-Pastrami
// Just some common functions used in my various projects


using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public static class Rami_Utils
{

    //////////////////////////////////////////////////////////
    //////////////// Positions and Rotations /////////////////
    //////////////////////////////////////////////////////////
    #region Positions and Rotations
    /// <summary>
    /// Applies a positional offset to a known position/rotation to get the new position, as if it were a child
    /// </summary>
    /// <param name="preOffsetPos"></param>
    /// <param name="offset"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    static Vector3 GetPosWithOffset(Vector3 preOffsetPos, Vector3 offset, Quaternion rotation)
    {
        return ((rotation * offset) + preOffsetPos);
    }

    /// <summary>
    /// returns a position (child) relative to a parents position and rotation, as if we were using their local axis
    /// </summary>
    /// <param name="ChildPos"></param>
    /// <param name="ParentPos"></param>
    /// <param name="ParentRotation"></param>
    /// <returns>Relative V3 Position</returns>
    static Vector3 GetLocalCoordinates(Vector3 childPos_Global, Vector3 parentPos_Global, Quaternion parentRotation)
    {
        return Matrix4x4.TRS(parentPos_Global, parentRotation, new Vector3(1f, 1f, 1f)).inverse.MultiplyPoint3x4(childPos_Global); //The solution for this came to me in a dream
    }

    /// <summary>
    /// Find rotation of A in respect to B, normalized for neural networks.
    /// Ensures first value is always positive to avoid artifacts in training
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    static Quaternion QuatOfARespectToBNormalized(Quaternion A, Quaternion B)
    {
        Quaternion C = (Quaternion.Inverse(B) * A);
        if (C.w < 0f) //avoid confusion in neural network
        {
            C = new Quaternion(-C.x, -C.y, -C.z, -C.w);
        }
        return C;
    }

    /// <summary>
    /// sets position with a V3 Offset (local space, not world) through the power of magic and/or quaternions,
    /// </summary>
    /// <param name="positionNeedingOffset"></param>
    /// <param name="newQuaternion"></param>
    /// <param name="YOffset"></param>
    /// <param name="transformTarget"><</param>
    /// <param name="rotationalOffset"><</param>
    static void SetRelativePosition(Vector3 positionNeedingOffset, Quaternion newQuaternion, Vector3 offset, Transform transformTarget, Quaternion rotationalOffset)
    {
        offset = newQuaternion * offset; //"Marty, you're not thinking fourth dimensionally!"
        offset += positionNeedingOffset;
        transformTarget.transform.SetPositionAndRotation(offset, newQuaternion * rotationalOffset); //apply additional rotational offset so plane looks correct
    }
    #endregion



    //////////////////////////////////////////////////////////
    ////////////////////// Other Stuff ///////////////////////
    //////////////////////////////////////////////////////////
    #region Other Stuff

    /// <summary>
    /// A modulus function that handles negatives in a sane way IMO
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static int NegatableMod(int a, int b) { return ((a % b) + b) % b; }

    #endregion
}
