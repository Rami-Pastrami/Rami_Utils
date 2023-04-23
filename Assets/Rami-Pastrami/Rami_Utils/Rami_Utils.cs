// Created by Rami-Pastrami
// Just some common functions used in my various projects


using System;
using UnityEngine;

namespace Rami.Utils
{
    public static class Rami_Utils
    {

        //////////////////////////////////////////////////////////
        //////////////// Positions and Rotations /////////////////
        //////////////////////////////////////////////////////////
        #region Positions and Rotations
        /// <summary>
        /// Finds the position of a point that has been offset from a parent position in accordance to that parent's rotation space
        /// </summary>
        /// <param name="preOffsetPos">The position of the 'parent'</param>
        /// <param name="offset">The offset to be applied in the parents rotation space</param>
        /// <param name="rotation">The parent's rotation</param>
        /// <returns>Offset world position of the offset</returns>
        static public Vector3 GetPosWithOffset(Vector3 preOffsetPos, Vector3 offset, Quaternion rotation)
        {
            return ((rotation * offset) + preOffsetPos);
        }

        /// <summary>
        /// returns a position (child) relative to a parents position and rotation, as if we were using their local axis
        /// </summary>
        /// <param name="childPos_Global"></param>
        /// <param name="parentPos_Global"></param>
        /// <param name="parentRotation"></param>
        /// <returns>Relative V3 Position</returns>
        static public Vector3 GetLocalCoordinates(Vector3 childPos_Global, Vector3 parentPos_Global, Quaternion parentRotation)
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
        static public Quaternion QuatOfARespectToBNormalized(Quaternion A, Quaternion B)
        {
            Quaternion C = (Quaternion.Inverse(B) * A);
            if (C.w < 0f) //avoid confusion in neural networks
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
        static public void SetRelativePosition(Vector3 positionNeedingOffset, Quaternion newQuaternion, Vector3 offset, Transform transformTarget, Quaternion rotationalOffset)
        {
            offset = GetPosWithOffset(positionNeedingOffset, offset, newQuaternion);
            transformTarget.transform.SetPositionAndRotation(offset, newQuaternion * rotationalOffset); //apply additional rotational offset so plane looks correct
        }
        #endregion

        //////////////////////////////////////////////////////////
        /////////////////// Stupid Conversions ///////////////////
        //////////////////////////////////////////////////////////
        #region Stupid Conversions
        // Can't wait for VRC to add a lot of array stuff natively


        /// <summary>
        /// Converts an array to a Comma Seperated Values string
        /// </summary>
        /// <typeparam name="TYPE">The element type that the array contains</typeparam>
        /// <param name="array"></param>
        /// <returns>CSV string</returns>
        static public string Array2CSV<TYPE>(TYPE[] array)
        {
            string[] strArr = new string[array.Length];
            for(int i = 0; i < array.Length; ++i)
            {
                strArr[i] = array[i].ToString();
            }
            return string.Join(", ", strArr);
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
        static public int NegatableMod(int a, int b) { return ((a % b) + b) % b; }

        #endregion
    }
}