using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TX
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyInInspectorAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class HelpBoxAttribute : PropertyAttribute   // TODO: implement property drawer
    {
        public enum MessageLevel
        {
            None,
            Info,
            Warning,
            Error,
        }

        public readonly string Message;
        public readonly MessageLevel Level;

        public HelpBoxAttribute(string message, MessageLevel level = MessageLevel.None)
        {
            Message = message;
            Level = level;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumFlagAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class PreviewAssetAttribute : PropertyAttribute
    {
        public readonly int Size;

        public PreviewAssetAttribute(int size = 3)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("Size must be positive");
            }
            Size = size;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InlineAttribute : PropertyAttribute
    {
        public readonly int Lines;
        public readonly bool DrawLabel;

        public InlineAttribute(int lines = 1)
        {
            if (lines <= 0)
            {
                throw new ArgumentOutOfRangeException("Line count should be positive");
            }
            Lines = lines;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InlineWtAttribute : PropertyAttribute
    {
        public readonly int Weight;

        public InlineWtAttribute(int weight)
        {
            if (weight <= 0)
            {
                throw new ArgumentOutOfRangeException("Weight value must be positive");
            }
            Weight = weight;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public readonly float Min, Max;
        public readonly float Step;

        public MinMaxRangeAttribute(float min, float max, float step = 1)
        {
            Min = min;
            Max = max;
            Step = step;
        }
    }

    public class PreviewColorAttribute : PropertyAttribute
    {
        public readonly Color Col;

        public PreviewColorAttribute(float r, float g, float b)
        {
            Col = new Color(r, g, b);
        }

        public PreviewColorAttribute(string code)
        {
            Col = UnityUtil.StrToColor(code);
        }

        public static Dictionary<T, Color> GetDictionary<T>(Color defaultColor)
        {
            return EnumUtil.GetValues<T>().ToDictionary(
                v => v,
                v =>
                {
                    Enum enumObj = (Enum)Enum.ToObject(typeof(T), Convert.ToInt32(v));
                    PreviewColorAttribute attr = enumObj.GetAttribute<PreviewColorAttribute>();
                    return attr != null ? attr.Col : defaultColor;
                });
        }
    }

    /// <summary>
    /// Marks a field as needing to be added as subasset(s).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SubAssetAttribute : Attribute { }

    /// <summary>
    /// Exposes a method in the inspector.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class InspectorButtonAttribute : Attribute
    {
        public string text;
        public bool playModeOnly;

        public InspectorButtonAttribute(string text, bool playModeOnly = false)
        {
            this.text = text;
            this.playModeOnly = playModeOnly;
        }
    }
}
