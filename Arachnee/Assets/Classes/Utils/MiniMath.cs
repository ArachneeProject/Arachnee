using UnityEngine;

namespace Assets.Classes.Utils
{
    public static class MiniMath
    {
        /// <summary>
        /// Returns the number of bit set to 1 in the binary representation of the given int.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public static int HammingWeight(int value)
        {
            value = value - ((value >> 1) & 0x55555555);
            value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
            return (((value + (value >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public static float GetSquaredDistance(Vector3 positionA, Vector3 positionB)
        {
            float X = positionA.x - positionB.x;
            float Y = positionA.y - positionB.y;
            float Z = positionA.z - positionB.z;
            return X * X + Y * Y + Z * Z;
        }
    }
}