using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TX
{
    /// <summary>
    /// Drawer of a serialized property.
    /// </summary>
    /// <param name="pos"> Rectangle on the screen to use for the property field. </param>
    /// <param name="prop"> The SerializedProperty to make a field for. </param>
    /// <param name="label"> The label to be displayed. </param>
    /// <returns>
    /// True if the property has children and is expanded and includeChildren was set to false;
    /// otherwise false.
    /// </returns>
    public delegate bool GUIDrawer(Rect pos, SerializedProperty prop, GUIContent label);

    /// <summary>
    /// Base class for custom <see cref="PropertyDrawer"/>.
    /// </summary>
    /// <seealso cref="UnityEditor.PropertyDrawer"/>
    public abstract class PropertyDrawerBase : PropertyDrawer
    {
        protected readonly float LineHeight = EditorGUIUtility.singleLineHeight;

        protected readonly float Padding = EditorGUIUtility.standardVerticalSpacing;

        /// <inheritdoc/>
        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Shouldn't call this function directly.
            try
            {
                OnGUISafe(position, property, label);
            }
            catch (Exception ex)
            {
                EditorUtil.HandleException(ex);
            }
        }

        /// <inheritdoc/>
        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Shouldn't call this function directly.
            try
            {
                return GetHeightSafe(property, label);
            }
            catch (Exception ex)
            {
                EditorUtil.HandleException(ex);
                return base.GetPropertyHeight(property, label);
            }
        }

        /// <summary>
        /// OnGUI call with exception handling.
        /// </summary>
        /// <param name="pos"> The position. </param>
        /// <param name="prop"> The property. </param>
        /// <param name="label"> The label. </param>
        public abstract void OnGUISafe(Rect pos, SerializedProperty prop, GUIContent label);

        /// <summary>
        /// GetPropertyHeight call with exception handling.
        /// </summary>
        /// <param name="prop"> The property. </param>
        /// <param name="label"> The label. </param>
        /// <returns> The property height. </returns>
        public virtual float GetHeightSafe(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label);
        }
    }

    /// <summary>
    /// Editor utility.
    /// </summary>
    public static class EditorUtil
    {
        public static readonly float LineHeight = EditorGUIUtility.singleLineHeight;

        /// <summary>
        /// Draws properties in single line.
        /// </summary>
        /// <param name="pos"> The position. </param>
        /// <param name="props"> The properties. </param>
        /// <param name="weights"> The weight of each property. </param>
        /// <param name="draw"> The property drawer. </param>
        /// <exception cref="System.Exception">
        /// Unmatched number of properties and weight values.
        /// </exception>
        public static void DrawInline(Rect pos, SerializedProperty[] props, int[] weights, GUIDrawer draw = null)
        {
            if (props.Length != weights.Length)
            {
                throw new Exception("Unmatched number of properties and weight values.");
            }
            int sumWeights = weights.Sum();

            float unitWidth = pos.width / sumWeights;
            float[] widths = weights.Select(w => w * unitWidth).ToArray();
            draw = draw ?? EditorGUI.PropertyField;
            float posX = pos.x;

            // draw properties in single line
            for (int i = 0; i < props.Length; i++)
            {
                float height = EditorGUI.GetPropertyHeight(props[i]);

                draw(
                    new Rect(
                        posX,
                        pos.y + (pos.height - height) / 2,  // centered vertically
                        widths[i],
                        height),
                    props[i],
                    GUIContent.none);
                posX += widths[i];
            }
        }

        /// <summary>
        /// Previews the asset.
        /// </summary>
        /// <param name="pos"> The position. </param>
        /// <param name="asset"> The asset to preview. </param>
        public static void PreviewAsset(Rect pos, UnityEngine.Object asset)
        {
            if (asset != null)
            {
                Texture2D tex = AssetPreview.GetAssetPreview(asset);
                if (tex != null)
                {
                    EditorGUI.DropShadowLabel(pos, new GUIContent(tex));
                }
            }
        }

        public static void HandleException(Exception ex)
        {
            if (ex is KnownBug)
            {
                // ignored
            }
            else if (ex is ExitGUIException)
            {
                // http://answers.unity3d.com/questions/385235/editorguilayoutcolorfield-inside-guilayoutwindow-c.html
                // Unity seems to throw this EVERY time one of the "Select" controls on **Field() is clicked.
                ////Debug.LogException(ex);
            }
            else
            {
                Debug.LogException(ex.InnerException ?? ex);
            }
        }
    }

    /// <summary>
    /// Editor extension methods.
    /// </summary>
    public static class EditorExtensions
    {
        /// <summary>
        /// Gets the underlying object from this <see cref="SerializedProperty"/>
        /// </summary>
        /// <param name="prop"> The property. </param>
        /// <param name="backStep">
        /// The relative upper level to the property, useful for finding parent.
        /// </param>
        /// <returns> The underlying object. </returns>
        /// <exception cref="KnownBug"> Unity haven't yet created the element in the array! </exception>
        public static object GetObject(this SerializedProperty prop, int backStep = 0)
        {
            try
            {
                var path = prop.propertyPath.Replace(".Array.data[", "[");
                object obj = prop.serializedObject.targetObject;
                var elements = path.Split('.');
                foreach (var element in elements.Take(elements.Length - backStep))
                {
                    if (element.Contains("["))
                    {
                        var elementName = element.Substring(0, element.IndexOf("["));
                        var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                        Array array = ReflectionUtil.GetFieldValue(obj, elementName) as Array;
                        obj = array.GetValue(index);
                    }
                    else
                    {
                        obj = ReflectionUtil.GetFieldValue(obj, element);
                    }
                }
                return obj;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new KnownBug("Unity hasn't yet created the element in the array!", ex);
            }
        }

        /// <summary>
        /// Gets the underlying object from this <see cref="SerializedProperty"/>
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="prop"> The property. </param>
        /// <param name="backStep">
        /// The relative upper level to the property, useful for finding parent.
        /// </param>
        /// <returns> The underlying object. </returns>
        public static T GetObject<T>(this SerializedProperty prop, int backStep = 0)
        {
            return (T)prop.GetObject(backStep);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <param name="prop"> The property. </param>
        /// <returns> All children of this property. </returns>
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty prop)
        {
            SerializedProperty it = prop.Copy();
            string parentPath = it.propertyPath;
            while (it.Next(true) && it.propertyPath.StartsWith(parentPath))
            {
                yield return it.Copy();
            }
        }
    }

    /// <summary>
    /// Selection utility.
    /// </summary>
    public static class SelectionUtil
    {
        /// <summary>
        /// Gets the selected folder.
        /// </summary>
        /// <value> The selected folder. </value>
        public static string ActiveFolder
        {
            get
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (path == string.Empty)
                {
                    path = "Assets";
                }
                else if (Path.GetExtension(path) != string.Empty)
                {
                    path = path.Replace(Path.GetFileName(path), string.Empty);
                }
                return path;
            }
        }
    }

    /// <summary>
    /// Wraps a <see cref="TextureImporter"/>. All changes to the importer will be applied at the end
    /// of the scope.
    /// </summary>
    public class TextureManager : IDisposable
    {
        /// <summary>
        /// The importer of the texture.
        /// </summary>
        public TextureImporter importer;

        /// <summary>
        /// Whether to obtain temporary read permission.
        /// </summary>
        private bool temporaryRead;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureManager"/> class.
        /// </summary>
        /// <param name="texture"> The texture. </param>
        /// <param name="temporaryRead">
        /// if set to <c> true </c>, the texture is guaranteed to be readable in the lifetime of this object.
        /// </param>
        public TextureManager(Texture2D texture, bool temporaryRead = false)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            importer = (TextureImporter)AssetImporter.GetAtPath(path);

            this.temporaryRead = temporaryRead;
            if (temporaryRead)
            {
                importer.isReadable = true;
                AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (temporaryRead)
            {
                importer.isReadable = false;
            }
            AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
        }
    }

    public class TempIndentLevel : TempValue<int>
    {
        public TempIndentLevel(int newVal)
            : base(newVal, v => EditorGUI.indentLevel = v, () => EditorGUI.indentLevel)
        { }
    }

    public class HandlesPainter : Painter
    {
        public static Painter Default = new HandlesPainter();

        public override Color Brush
        {
            get { return Handles.color; }
            set { Handles.color = value; }
        }

        public override void DrawLine(Vector3 from, Vector3 to)
        {
            Handles.DrawLine(from, to);
        }
    }

    #region Attribute Drawers

    /// <summary>
    /// Draws a read-only property.
    /// </summary>
    /// <seealso cref="TX.PropertyDrawerBase"/>
    [CustomPropertyDrawer(typeof(ReadOnlyInInspectorAttribute))]
    public class ReadOnlyDrawer : PropertyDrawerBase
    {
        /// <inheritdoc/>
        public override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

    /// <summary>
    /// Draws enum flags.
    /// </summary>
    /// <seealso cref="TX.PropertyDrawerBase"/>
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawerBase
    {
        /// <inheritdoc/>
        public override void OnGUISafe(Rect pos, SerializedProperty property, GUIContent label)
        {
            Enum targetEnum = property.GetObject() as Enum;

            Dictionary<bool, Enum[]> allValues = Enum.GetValues(targetEnum.GetType()).OfType<Enum>()
                .GroupBy(e => e.IsFlag())
                .ToDictionary(g => g.Key, g => g.ToArray());

            Rect lineRect = new Rect(pos.xMin, pos.yMin, pos.width, LineHeight);
            property.isExpanded = EditorGUI.Foldout(lineRect, property.isExpanded, label);
            if (property.isExpanded)
            {
                lineRect.position += new Vector2(0, LineHeight);

                // help enable undo
                EditorGUI.indentLevel++;
                EditorGUI.BeginProperty(pos, label, property);
                int newValue = 0;
                bool setMultiple = false;       // if any button is clicked, don't apply toggle for single flags

                foreach (bool forFlags in new[] { false, true })
                {
                    Enum[] values;
                    if (!allValues.TryGetValue(forFlags, out values))
                        continue;

                    for (int i = 0; i < values.Length; i++)
                    {
                        Enum value = values[i];
                        GUIContent enumLabel = new GUIContent(value.ToString());
                        if (!forFlags)
                        {
                            // multi-flag buttons
                            Rect indentedRect = EditorGUI.IndentedRect(lineRect);
                            if (GUI.Button(indentedRect, new GUIContent(value.ToString())))
                            {
                                newValue = Convert.ToInt32(value);
                                setMultiple = true;
                            }
                        }
                        else
                        {
                            // single-flag toggles
                            int iFlag = Convert.ToInt32(value);

                            Rect controlRect = EditorGUI.PrefixLabel(lineRect, enumLabel);
                            if (EditorGUI.Toggle(controlRect, targetEnum.HasFlag(value)))
                            {
                                if (!setMultiple)
                                {
                                    newValue |= iFlag;
                                }
                            }
                        }
                        lineRect.position += new Vector2(0, LineHeight);
                    }
                }

                property.intValue = newValue;

                EditorGUI.EndProperty();
                EditorGUI.indentLevel--;
            }
        }

        /// <inheritdoc/>
        public override float GetHeightSafe(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return LineHeight * (1 + Enum.GetValues(property.GetObject().GetType()).Length);
            }
            else
            {
                return LineHeight;
            }
        }
    }

    /// <summary>
    /// Previews an asset property.
    /// </summary>
    /// <seealso cref="TX.PropertyDrawerBase"/>
    [CustomPropertyDrawer(typeof(PreviewAssetAttribute))]
    public class PreviewAssetDrawer : PropertyDrawerBase
    {
        /// <inheritdoc/>
        public override void OnGUISafe(Rect pos, SerializedProperty prop, GUIContent label)
        {
            var attr = (PreviewAssetAttribute)attribute;
            Rect previewRect = new Rect(pos.xMax - pos.height, pos.y, pos.height, pos.height);
            Rect fieldRect = new Rect(pos.x, pos.y, pos.width - pos.height, pos.height / attr.Size);
            EditorGUI.PropertyField(fieldRect, prop);
            EditorUtil.PreviewAsset(previewRect, prop.GetObject<UnityEngine.Object>());
        }

        /// <inheritdoc/>
        public override float GetHeightSafe(SerializedProperty property, GUIContent label)
        {
            return LineHeight * ((PreviewAssetAttribute)attribute).Size;
        }
    }

    /// <summary>
    /// Draws the property in single line.
    /// </summary>
    /// <seealso cref="TX.PropertyDrawerBase"/>
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlineDrawer : PropertyDrawerBase
    {
        /// <inheritdoc/>
        public override void OnGUISafe(Rect pos, SerializedProperty prop, GUIContent label)
        {
            Type parentType = prop.GetObject().GetType();

            SerializedProperty[] children = prop.GetChildren().ToArray();

            Rect controlRect = EditorGUI.PrefixLabel(pos, label);

            using (var temp = new TempIndentLevel(0))
            {
                EditorUtil.DrawInline(
                    controlRect,
                    children,
                    children.Select(p =>
                    {
                        InlineWtAttribute attr = Attribute.GetCustomAttribute(parentType.GetField(p.name), typeof(InlineWtAttribute)) as InlineWtAttribute;
                        if (attr == null)
                        {
                            // default weight is 1
                            return 1;
                        }
                        return attr.Weight;
                    }).ToArray());
            }
        }

        /// <inheritdoc/>
        public override float GetHeightSafe(SerializedProperty prop, GUIContent label)
        {
            return LineHeight * ((InlineAttribute)attribute).Lines;
        }
    }

    [CustomPropertyDrawer(typeof(Vec2i))]
    public class Vec2iDrawer : InlineDrawer
    {
        public override float GetHeightSafe(SerializedProperty prop, GUIContent label)
        {
            return LineHeight;
        }
    }

    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeDrawer : PropertyDrawerBase
    {
        public override float GetHeightSafe(SerializedProperty property, GUIContent label)
        {
            return base.GetHeightSafe(property, label) + LineHeight;
        }

        // Draw the property inside the given rect
        public override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var range = attribute as MinMaxRangeAttribute;
            object rangeValue = property.GetObject();

            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.LabelField(
                new Rect(position.x, position.y + LineHeight, position.width, LineHeight),
                range.Min.ToString("0.##"));
            EditorGUI.LabelField(
                new Rect(position.x + position.width - 28f, position.y + LineHeight, position.width, LineHeight),
                range.Max.ToString("0.##"));

            DrawMinMaxSlider(position, rangeValue, label);
        }

        private void DrawMinMaxSlider(Rect position, object rangeValue, GUIContent label)
        {
            var range = attribute as MinMaxRangeAttribute;
            Rangei rangeiValue = rangeValue as Rangei;
            Rangef rangefValue = rangeValue as Rangef;

            var halfWidth = position.width / 2;
            Rect minFieldRect = new Rect(position.x, position.y, halfWidth - Padding, LineHeight);
            Rect maxFieldRect = new Rect(position.x + halfWidth, position.y, halfWidth - Padding, LineHeight);

            float newMin, newMax;
            if (rangeiValue != null)
            {
                newMin = EditorGUI.IntField(minFieldRect, rangeiValue.min);
                newMax = EditorGUI.IntField(maxFieldRect, rangeiValue.max);
            }
            else
            {
                newMin = EditorGUI.FloatField(minFieldRect, rangefValue.min);
                newMax = EditorGUI.FloatField(maxFieldRect, rangefValue.max);
            }

            EditorGUI.MinMaxSlider(
                new Rect(position.x + 32f, position.y + LineHeight, position.width - 64f, LineHeight),
                ref newMin, ref newMax, range.Min, range.Max);

            newMin = Mathf.Max(range.Min, ((int)(newMin / range.Step) * range.Step));
            newMax = Mathf.Max(range.Min, ((int)(newMax / range.Step) * range.Step));
            if (newMax < newMin)
            {
                GenericUtil.Swap(ref newMin, ref newMax);
            }

            if (rangeiValue != null)
            {
                rangeiValue.min = (int)(newMin);
                rangeiValue.max = (int)(newMax);
            }
            else
            {
                rangefValue.min = newMin;
                rangefValue.max = newMax;
            }
        }
    }

    #endregion

    #region Custom Editors

    /// <summary>
    /// Generic inspector for all types.
    /// </summary>
    /// <seealso cref="UnityEditor.Editor" />
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class InspectorBase : Editor
    {
        public override void OnInspectorGUI()
        {
            #region HelpBox
            {
                var attr = target.GetType().GetCustomAttributes(typeof(HelpBoxAttribute), true);
                if (attr.Length == 1)
                {
                    HelpBoxAttribute helpbox = (HelpBoxAttribute)attr[0];
                    EditorGUILayout.HelpBox(helpbox.Message, (MessageType)helpbox.Level);
                }
            }
            #endregion

            base.OnInspectorGUI();// DEFAULT GUI

            #region Buttons
            // Get the type descriptor for the MonoBehaviour we are drawing
            var type = target.GetType();

            // Iterate over each private or public instance method (no static methods atm)
            foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                // make sure it is decorated by our custom attribute
                var attribute = Attribute.GetCustomAttribute(method, typeof(InspectorButtonAttribute)) as InspectorButtonAttribute;
                if (attribute != null)
                {
                    if (!attribute.playModeOnly || EditorApplication.isPlaying)
                    {
                        if (GUILayout.Button(attribute.text))
                        {
                            method.Invoke(target, new object[0]);
                        }
                    }
                }
            }
            #endregion
        }
    }

    #endregion

    #region EditorWindow & Utils

    public abstract class EditorWindowBase : EditorWindow
    {
        private static MethodInfo isDockedMethod = typeof(EditorWindow)
                    .GetProperty("docked", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetGetMethod(true);

        protected bool IsDocked
        {
            get
            {
                return (bool)isDockedMethod.Invoke(this, null);
            }
        }

        private void OnGUI()
        {
            // Shouldn't call this function directly.
            try
            {
                OnGUISafe();
            }
            catch (Exception ex)
            {
                EditorUtil.HandleException(ex);
            }
            try
            {
                OnLateGUISafe();
            }
            catch (Exception ex)
            {
                EditorUtil.HandleException(ex);
            }
        }

        protected abstract void OnGUISafe();

        protected virtual void OnLateGUISafe() { }
    }

    /// <summary>
    /// http://webcache.googleusercontent.com/search?q=cache:9POBmU8IRPEJ:martinecker.com/martincodes/unity-editor-window-zooming/+&cd=1&hl=en&ct=clnk&gl=ca
    /// Ending all groups should be the only way to make corresponding draw unnoticed of the offset.
    /// Otherwise like in the example, the author had to manually offset when drawing zoom area.
    /// </summary>
    public class EditorPanArea
    {
        private static float kEditorWindowTabHeight
        {
            get { return isDocked ? 19f : 22f; }
        }

        private static bool isDocked;
        private static Matrix4x4 prevMatrix;

        public static void Begin(Vector2 panOrigin, Vector2 offset, bool docked)
        {
            GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

            isDocked = docked;

            prevMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(panOrigin + offset, Quaternion.identity, Vector3.one) * GUI.matrix;
        }

        public static void End()
        {
            GUI.matrix = prevMatrix;
            GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width, Screen.height));
        }
    }

    #endregion
}
