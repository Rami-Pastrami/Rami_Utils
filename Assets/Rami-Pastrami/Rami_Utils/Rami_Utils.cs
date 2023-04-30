// Created by Rami-Pastrami
// Just some common functions used in my various projects
// Up to date public repo can be found at https://github.com/Rami-Pastrami/Rami_Utils

// Uncomment to enable safety warnings, useful for debugging. This should be commented out before release for performance reasons
//#define DEBUG

using System;
using UnityEngine;

namespace Rami
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
        /////////////////////// Array Jank ///////////////////////
        //////////////////////////////////////////////////////////
        #region Array Jank
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
            for (int i = 0; i < array.Length; ++i)
            {
                strArr[i] = array[i].ToString();
            }
            return string.Join(", ", strArr);
        }

        /// <summary>
        /// Write a subarray onto a parent array, overwriting previous values, starting from a specific (parent) index
        /// </summary>
        /// <typeparam name="TYPE">The element type that the array contains</typeparam>
        /// <param name="subArray"></param>
        /// <param name="parentArray"></param>
        /// <param name="index"></param>
        /// <returns>Altered parent array</returns>
        static public TYPE[] WriteSubArrayOverArray<TYPE>(TYPE[] subArray, TYPE[] parentArray, int index)
        {
            #if DEBUG
            if (subArray.Length + index > parentArray.Length)
            {
                //Only log a warning instead of an error so we cause a more useful error instead of stopping beforehand
                Debug.LogWarning("Rami_Utils DEBUG - WriteSubArrayOverArray - index and subArray lengths are greater than parentArray length, this WILL cause an error");
            }
            #endif

            for (int i = 0; i < subArray.Length; ++i)
            {
                parentArray[i + index] = subArray[i];
            }
            return parentArray;
        }

        /// <summary>
        /// returns SubArray from parent array, given start and end indexes
        /// </summary>
        /// <typeparam name="TYPE">The element type that the array contains</typeparam>
        /// <param name="parentArray"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns>Subarray of TYPE</returns>
        static public TYPE[] GetSubArrayFromArray_StartEnd<TYPE>(TYPE[] parentArray, int startIndex, int endIndex)
        {
            #if DEBUG
            if ((startIndex < 0) | (endIndex > parentArray.Length))
            {
                Debug.LogWarning("Rami_Utils DEBUG - GetSubArrayFromArray_StartEnd - start/end values are invalid. this WILL cause an error");
            }
            #endif

            TYPE[] output = new TYPE[endIndex - startIndex];
            int i = 0;
            for (; startIndex < endIndex; ++startIndex)
            {
                output[i] = parentArray[startIndex];
                i++;
            }

            return output;
        }

        /// <summary>
        /// returns SubArray from parent array, given start and subarray size
        /// </summary>
        /// <typeparam name="TYPE">The element type that the array contains</typeparam>
        /// <param name="parentArray"></param>
        /// <param name="startIndex"></param>
        /// <param name="size"></param>
        /// <returns>Subarray of TYPE</returns>
        static public TYPE[] GetSubArrayFromArray_StartSize<TYPE>(TYPE[] parentArray, int startIndex, int size)
        {
            return GetSubArrayFromArray_StartEnd(parentArray, startIndex, startIndex + size);
        }

        /// <summary>
        /// Expands array from the start, with the first part being filled with a defined value and the rest kept as is
        /// </summary>
        /// <typeparam name="TYPE">The element type that the array contains</typeparam>
        /// <param name="initialArray">The array you intend to expand</param>
        /// <param name="numElementsToAdd">number of elements you intend to expand by</param>
        /// <param name="fillingElement">The element th blank expanded spots will be filled with</param>
        /// <returns>The expanded array</returns>
        static public TYPE[] ExpandArrayFromStart<TYPE>(TYPE[] initialArray, int numElementsToAdd, TYPE fillingElement)
        {
            TYPE[] output = new TYPE[numElementsToAdd + initialArray.Length];

            for (int i = 0; i < numElementsToAdd; ++i)
            {
                output[i] = fillingElement;
            }

            for (int i = 0; i < initialArray.Length; ++i)
            {
                output[i + numElementsToAdd] = initialArray[i];
            }

            return output;
        }

        /// <summary>
        /// Expands array from the start, with the last part being filled with a defined value and the rest kept as is
        /// </summary>
        /// <typeparam name="TYPE">The element type that the array contains</typeparam>
        /// <param name="initialArray">The array you intend to expand</param>
        /// <param name="numElementsToAdd">number of elements you intend to expand by</param>
        /// <param name="fillingElement">The element th blank expanded spots will be filled with</param>
        /// <returns>The expanded array</returns>
        static public TYPE[] ExpandArrayFromEnd<TYPE>(TYPE[] initialArray, int numElementsToAdd, TYPE fillingElement)
        {
            TYPE[] output = new TYPE[numElementsToAdd + initialArray.Length];

            for(int i = 0; i < initialArray.Length; ++i)
            {
                output[i] = initialArray[i];
            }

            for (int i = 0; i < numElementsToAdd; ++i)
            {
                output[i + initialArray.Length] = fillingElement;
            }

            return output;
        }

        /// <summary>
        /// Converts a Vector2 to a float array
        /// </summary>
        /// <param name="input"></param>
        /// <returns>2 long float array</returns>
        static public float[] Vector22FloatArr(Vector2 input) { return new float[] { input.x, input.y }; }

        /// <summary>
        /// Converts a Vector3 to a float array
        /// </summary>
        /// <param name="input"></param>
        /// <returns>3 long float array</returns>
        static public float[] Vector32FloatArr(Vector3 input) { return new float[] { input.x, input.y, input.z }; }

        /// <summary>
        /// Converts a Vector4 to a float array
        /// </summary>
        /// <param name="input"></param>
        /// <returns>4 long float array</returns>
        static public float[] Vector42FloatArr(Vector4 input) { return new float[] { input.x, input.y, input.z, input.w }; }

        /// <summary>
        /// Converts an array of Vector2s into a 1D float array
        /// </summary>
        /// <param name="input"></param>
        /// <returns>1D float array</returns>
        static public float[] Vector2Arr2FloatArr(Vector2[] input)
        {
            float[] output = new float[input.Length * 2];
            for (int i = 0; i < input.Length; ++i)
            {
                output[i] = input[i].x;
                output[i + 1] = input[i].y;
            }
            return output;
        }

        /// <summary>
        /// returns midpoint
        /// </summary>
        /// <param name="Vs"></param>
        /// <returns></returns>
        static public Vector2 CenterOfVector2s(Vector2[] Vs)
        {
            Vector2 output = Vector2.zero;
            for (int i = 0; i < Vs.Length; ++i)
            {
                output += Vs[i];
            }

            return output / (float)Vs.Length;
        }

        /// <summary>
        /// Converts an array of Vector3s into a 1D float array
        /// </summary>
        /// <param name="input"></param>
        /// <returns>1D float array</returns>
        static public float[] Vector3Arr2FloatArr(Vector3[] input)
        {
            float[] output = new float[input.Length * 3];
            for (int i = 0; i < input.Length; ++i)
            {
                output[i] = input[i].x;
                output[i + 1] = input[i].y;
                output[i + 2] = input[i].z;
            }
            return output;
        }

        /// <summary>
        /// returns midpoint
        /// </summary>
        /// <param name="Vs"></param>
        /// <returns></returns>
        static public Vector3 CenterOfVector3s(Vector3[] Vs)
        {
            Vector3 output = Vector3.zero;
            for (int i = 0; i < Vs.Length; ++i)
            {
                output += Vs[i];
            }

            return output / (float)Vs.Length;
        }

        /// <summary>
        /// Converts an array of Vector4s into a 1D float array
        /// </summary>
        /// <param name="input"></param>
        /// <returns>1D float array</returns>
        static public float[] Vector4Arr2FloatArr(Vector4[] input)
        {
            float[] output = new float[input.Length * 4];
            for (int i = 0; i < input.Length; ++i)
            {
                output[i] = input[i].x;
                output[i + 1] = input[i].y;
                output[i + 2] = input[i].z;
                output[i + 3] = input[i].w;
            }
            return output;
        }

        /// <summary>
        /// returns midpoint
        /// </summary>
        /// <param name="Vs"></param>
        /// <returns></returns>
        static public Vector4 CenterOfVector4s(Vector4[] Vs)
        {
            Vector4 output = Vector4.zero;
            for (int i = 0; i < Vs.Length; ++i)
            {
                output += Vs[i];
            }

            return output / (float)Vs.Length;
        }

        /// <summary>
        /// returns midpoint position vector from an array of transforms
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        static public Vector3 GetCenterOfTransforms(Transform[] trans)
        {
            Vector3 output = Vector3.zero;
            for (int i = 0; i < trans.Length; i++)
            {
                output += trans[i].position;
            }

            return output / (float)trans.Length;
        }

        /// <summary>
        /// Converts Transform Array into Position Vector3 Array
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        static public Vector3[] TransformArr2PosArr(Transform[] trans)
        {
            Vector3[] output = new Vector3[trans.Length];
            for (int i = 0; i < trans.Length; ++i)
            {
                output[i] = trans[i].position;
            }
            return output;
        }

        /// <summary>
        /// Converts Transform Array into Rotation Quaternion Array
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        static public Quaternion[] TransformArr2RotArr(Transform[] trans)
        {
            Quaternion[] output = new Quaternion[trans.Length];
            for (int i = 0; i < trans.Length; ++i)
            {
                output[i] = trans[i].rotation;
            }
            return output;
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