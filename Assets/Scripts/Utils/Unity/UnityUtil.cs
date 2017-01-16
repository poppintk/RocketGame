using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TX
{
    public static class UnityUtil
    {
        /// <summary>
        /// Coalesces the specified alternative. Use this method when <see cref="values"/> can be
        /// either <see cref="object"/> or <see cref="UnityEngine.Object"/>.
        /// </summary>
        /// <param name="values"> The values to be null-coalesced. </param>
        /// <returns> The first non-null object. </returns>
        public static T Coalesce<T>(params T[] values) where T : UnityEngine.Object
        {
            return values.FirstOrDefault(v => v != null);
        }

        private static readonly Dictionary<string, Color> builtInColorMap = new[]
        {
            "black",
            "white",
            "red",
            "yellow",
            "magenta",
            "blue",
            "clear",
            "grey",
            "gray",
            "green",
            "cyan",
        }.ToDictionary(name => name, name => (Color)typeof(Color).GetProperty(name).GetValue(null, null));

        public static Color StrToColor(string code)
        {
            code = code
                .Replace("0x", "")
                .Replace("#", "")
                .ToLower();

            Color builtIn;
            if (builtInColorMap.TryGetValue(code, out builtIn))
                return builtIn;

            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(code.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(code.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(code.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            //Only use alpha if the string has enough characters
            if (code.Length == 8)
            {
                a = byte.Parse(code.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }
    }

    /// <summary>
    /// Unity extension methods.
    /// </summary>
    public static class UnityExtensions
    {
        #region Rect

        public static float Area(this Rect r) { return r.width * r.height; }

        public static Vector2 BottomLeft(this Rect r) { return new Vector2(r.xMin, r.yMin); }

        public static Vector2 TopLeft(this Rect r) { return new Vector2(r.xMin, r.yMax); }

        public static Vector2 TopRight(this Rect r) { return new Vector2(r.xMax, r.yMax); }

        public static Vector2 BottomRight(this Rect r) { return new Vector2(r.xMax, r.yMin); }

        public static Rect Mult(this Rect r, float s)
        {
            return new Rect(r.position * s, r.size * s);
        }

        public static Rect Shrink(this Rect r, float x, float y)
        {
            return new Rect()
            {
                xMin = r.xMin + x,
                yMin = r.yMin + y,
                xMax = r.xMax - x,
                yMax = r.yMax - y,
            };
        }

        public static Rect Shrink(this Rect r, float xy)
        {
            return r.Shrink(xy, xy);
        }

        public static Rect Expand(this Rect r, float x, float y)
        {
            return r.Shrink(-x, -y);
        }

        public static Rect Expand(this Rect r, float xy)
        {
            return r.Shrink(-xy);
        }

        public static IEnumerable<Vector2> GetCorners(this Rect r)
        {
            yield return r.BottomLeft();
            yield return r.BottomRight();
            yield return r.TopRight();
            yield return r.TopLeft();
        }

        public static float AspectRatio(this Rect r) { return r.width / r.height; }

        public static Rect Move(this Rect rect, Vector2 offset)
        {
            return new Rect(rect.position + offset, rect.size);
        }

        #endregion

        #region Ray

        /// <summary>
        /// Checks if ray intersects with the specified plane.
        /// </summary>
        /// <param name="r"> The ray. </param>
        /// <param name="plane"> The plane. </param>
        /// <param name="intersection"> The intersection point. </param>
        /// <returns> True if this ray intersects with the plane. </returns>
        public static bool Intersect(this Ray r, Plane plane, out Vector3 intersection)
        {
            float dist;
            bool intersected = plane.Raycast(r, out dist);
            intersection = intersected ? r.GetPoint(dist) : new Vector3(-1, -2, -3);
            return intersected;
        }

        #endregion

        #region Transform

        public static Transform AddEmptyChild(this Transform transform, string name)
        {
            Transform child = new GameObject(name).transform;
            child.SetParent(transform);
            return child;
        }

        public static Transform InstantiateAsChild(this Transform transform, GameObject go, string name = null)
        {
            Transform child = GameObject.Instantiate(go).transform;
            child.SetParent(transform);
            if (name != null)
                child.name = name;
            return child;
        }

        public static T InstantiateComponentAsChild<T>(this Transform transform, T comp) where T : Component
        {
            T child = UnityEngine.Object.Instantiate(comp);
            child.transform.SetParent(transform);
            return child;
        }

        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        #endregion

        #region Vector

        /// <summary>
        /// Map the vector to the xz component of a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vec"> The vector. </param>
        /// <param name="y"> The new y value. </param>
        /// <returns> The new vector. </returns>
        public static Vector3 ToXZ(this Vector2 vec, float y = 0f)
        {
            return new Vector3(vec.x, y, vec.y);
        }

        /// <summary>
        /// Gets the xz components as <see cref="Vector2"/>.
        /// </summary>
        /// <param name="vec"> The vector. </param>
        /// <returns> Vector with xz components. </returns>
        public static Vector2 GetXZ(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        #endregion

        #region Camera

        /// <summary>
        /// Shoot ray from a viewport point forward and backward return the first intersection to a plane.
        /// </summary>
        /// <param name="cam"> This camera. </param>
        /// <param name="viewport"> The viewport point. </param>
        /// <param name="plane"> The plane. </param>
        /// <returns> The intersection point. </returns>
        public static Vector3 ViewportPointToPlane(this Camera cam, Vector2 viewport, Plane plane)
        {
            // shoot a ray forward, then if not hit, shoot another ray backward
            Ray r = cam.ViewportPointToRay(new Vector3(viewport.x, viewport.y, 0));
            Vector3 intersection;
            if (!r.Intersect(plane, out intersection))
            {
                new Ray(r.origin, -r.direction).Intersect(plane, out intersection);
            }
            return intersection;
        }

        /// <summary>
        /// Determine if a world position is within view frustum of this camera.
        /// </summary>
        /// <param name="cam"> This camera. </param>
        /// <param name="worldPoint"> The world point. </param>
        /// <returns> True if camera's view frustum covers given point. </returns>
        public static bool Covers(this Camera cam, Vector2 worldPoint)
        {
            Vector3 vp = cam.WorldToViewportPoint(worldPoint);
            return MathUtil.InRange(vp.x, 0, 1)
                && MathUtil.InRange(vp.y, 0, 1);
        }

        /// <summary>
        /// Determine if a world position is within view frustum of this camera.
        /// </summary>
        /// <param name="cam"> This camera. </param>
        /// <param name="worldPoint"> The world point. </param>
        /// <returns> True if camera's view frustum covers given point. </returns>
        public static bool Covers(this Camera cam, Vector3 worldPoint)
        {
            Vector3 vp = cam.WorldToViewportPoint(worldPoint);
            return MathUtil.InRange(vp.x, 0, 1)
                && MathUtil.InRange(vp.y, 0, 1)
                && MathUtil.InRange(vp.z, 0, 1);
        }

        #endregion

        #region Color

        public static string ToHex(this Color32 color)
        {
            return color.r.ToString("X2") +
                color.g.ToString("X2") +
                color.b.ToString("X2") +
                color.a.ToString("X2");
        }

        #endregion

        #region Texture

        public static void Overlay(this Texture2D background, Texture2D overlay)
        {
            if (background.width != overlay.width || background.height != overlay.height)
            {
                throw new ArgumentException("Unmatched size");
            }
            Color32[] bgPix = background.GetPixels32();
            Color32[] fgPix = overlay.GetPixels32();
            for (int i = 0; i < bgPix.Length; i++)
            {
                if (fgPix[i].a > 0)
                {
                    bgPix[i] = fgPix[i];
                }
            }
            background.SetPixels32(bgPix);
            background.Apply();
        }

        #endregion

        #region Material

        public static void SetAlpha(this Material mat, float alpha)
        {
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;
        }

        #endregion

        #region UI

        public static void Hide(this CanvasGroup cg)
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        public static void Show(this CanvasGroup cg)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        #endregion

        /// <summary>
        /// Determines whether this instance is null. Use this method when <see cref="obj"/> can be
        /// either <see cref="object"/> or <see cref="UnityEngine.Object"/>.
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <returns> Whether the object is null. </returns>
        public static bool IsNull(this object obj)
        {
            return obj == null || (obj is UnityEngine.Object && (obj as UnityEngine.Object) == null);
        }
    }

    /// <summary>
    /// RNG extension methods.
    /// </summary>
    public static class RNGExtensions
    {
        public static Vector2 NextVector2(this RNG rng)
        {
            return new Vector2(rng.NextFloat1(), rng.NextFloat1());
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Gizmo drawing utility.
    /// </summary>
    public static class GizmoUtil
    {
        public static void DrawText(string text, Vector3 worldPos, Color? colour = null)
        {
            UnityEditor.Handles.BeginGUI();

            var view = UnityEditor.SceneView.currentDrawingSceneView;
            if (view != null && view.camera != null)
            {
                var tempColor = Gizmos.color;
                if (colour.HasValue)
                    Gizmos.color = colour.Value;

                Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
                Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
                GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height, size.x, size.y), text);

                Gizmos.color = tempColor;
            }

            UnityEditor.Handles.EndGUI();
        }
    }

#endif
}
