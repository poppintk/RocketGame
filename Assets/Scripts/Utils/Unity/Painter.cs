using System;
using UnityEngine;

namespace TX
{
    public abstract class Painter
    {
        public virtual Color Brush { get; set; }

        public abstract void DrawLine(Vector3 from, Vector3 to);

        /// <summary>
        /// Draws the point at given position.
        /// </summary>
        /// <param name="pt">The position.</param>
        /// <param name="scale">The scale (doesn't influence position).</param>
        public virtual void DrawPoint(Vector3 pt, float scale)
        {
            scale /= 2;
            Vector3 right = Vector3.right * scale;
            Vector3 up = Vector3.up * scale;
            Vector3 forward = Vector3.forward * scale;
            DrawLine(pt - right, pt + right);
            DrawLine(pt - up, pt + up);
            DrawLine(pt - forward, pt + forward);
        }

        /// <summary>
        /// Draws a rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="shrink">The shrink value.</param>
        /// <param name="xzPlane">if set to <c> true </c> draw on xz plane, otherwise draw on xy plane.</param>
        public virtual void DrawRect(Rect rect, float scale = 1f, float shrink = 0f, bool xzPlane = true, float defaultAxis = 0f)
        {
            Func<Vector2, Vector3> toVec3 = xzPlane ?
                (Func<Vector2, Vector3>)
                (v2 => v2.ToXZ(defaultAxis)) :
                (v2 => new Vector3(v2.x, v2.y, defaultAxis));
            if (shrink != 0f)
            {
                rect = rect.Shrink(shrink);
            }
            DrawLine(
                toVec3(rect.BottomLeft() * scale),
                toVec3(rect.BottomRight() * scale));
            DrawLine(
                toVec3(rect.BottomRight() * scale),
                toVec3(rect.TopRight() * scale));
            DrawLine(
                toVec3(rect.TopRight() * scale),
                toVec3(rect.TopLeft() * scale));
            DrawLine(
                toVec3(rect.TopLeft() * scale),
                toVec3(rect.BottomLeft() * scale));
        }

        public virtual void DrawCube(Vector3 center, Vector3 size)
        {
            throw new NotImplementedException();
        }
    }

    public class DebugPainter : Painter
    {
        public static Painter Default = new DebugPainter();

        public override void DrawLine(Vector3 from, Vector3 to)
        {
            Debug.DrawLine(from, to, Brush);
        }
    }

    public class GizmosPainter : Painter
    {
        public static Painter Default = new GizmosPainter();

        public override Color Brush
        {
            get { return Gizmos.color; }
            set { Gizmos.color = value; }
        }

        public override void DrawLine(Vector3 from, Vector3 to)
        {
            Gizmos.DrawLine(from, to);
        }

        public override void DrawCube(Vector3 center, Vector3 size)
        {
            Gizmos.DrawCube(center, size);
        }
    }
}
