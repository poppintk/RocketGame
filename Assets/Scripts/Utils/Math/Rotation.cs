using System;

namespace TX
{
    /// <summary>
    /// Rotation of multiple of 90 degrees.
    /// </summary>
    public enum Rot4 { None, CCW90, Rev, CW90 }

    /// <summary>
    /// Rotation extension.
    /// </summary>
    public static class RotationExt
    {
        #region Rot4

        /// <summary>
        /// Adds the specified rotation.
        /// </summary>
        /// <param name="a"> This rotation. </param>
        /// <param name="b"> The additional rotation. </param>
        /// <returns> The combined rotation. </returns>
        public static Rot4 Add(this Rot4 a, Rot4 b)
        {
            return (Rot4)(((int)a + (int)b) % 4);
        }

        /// <summary>
        /// Gets the reversed/negative rotation.
        /// NOTE: This is different from just adding <see cref="Rot4.Rev"/>!!
        /// </summary>
        /// <param name="r"> This rotation. </param>
        /// <returns> The reversed rotation. </returns>
        public static Rot4 Reverse(this Rot4 r)
        {
            return (Rot4)((4 - (int)r) % 4);
        }

        #endregion
    }
}
