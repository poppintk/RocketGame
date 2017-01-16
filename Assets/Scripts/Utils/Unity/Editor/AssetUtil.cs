using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TX
{
    public static class AssetUtil
    {
        private class RecursiveObject
        {
            public object Value;
            public bool IncludeSelf;

            public RecursiveObject(object parent, bool includeParent)
            {
                Value = parent;
                IncludeSelf = includeParent;
            }
        }

        public static ScriptableObject Instantiate(Type t)
        {
            MethodInfo createMethod = typeof(ScriptableObject)
                .GetGenericMethod("CreateInstance", null)
                .MakeGenericMethod(t);
            return createMethod.Invoke(null, null) as ScriptableObject;
        }

        /// <summary>
        /// Saves the changes to an existing asset. Works only if asset is linked to a file.
        /// </summary>
        /// <param name="asset">The asset.</param>
        public static void SaveChanges(ScriptableObject asset)
        {
            AddAllSubAssets(asset);
            EditorUtility.SetDirty(asset);      // just to make sure it's marked as dirty
            AssetDatabase.SaveAssets();
        }

        public static void Save(ScriptableObject asset, string folder = null, string name = null)
        {
            if (name == null)
            {
                name = "New_" + asset.GetType().Name + ".asset";
            }
            if (folder == null)
            {
                folder = "Assets";
            }
            string path = Path.Combine(folder, name);
            AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(asset, path);

            AddAllSubAssets(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        public static T Create<T>(string folder, string filename) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            Save(asset, folder, filename);
            return asset;
        }

        public static ScriptableObject Create(Type t, string folder, string filename)
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(t);
            Save(asset, folder, filename);
            return asset;
        }

        public static void AddSubAsset(ScriptableObject mainAsset, ScriptableObject subAsset)
        {
            // Since AssetDatabase.IsSubAsset doesn't work at all or just not with HideInHierarchy
            subAsset.hideFlags = HideFlags.HideInHierarchy;
            string subPath = AssetDatabase.GetAssetPath(subAsset);
            string mainPath = AssetDatabase.GetAssetPath(mainAsset);

            if (subPath != mainPath)
            {
                if (!string.IsNullOrEmpty(subPath))
                {
                    Debug.LogErrorFormat("SubAsset exists in a different path: {0}", subPath);
                    return;
                }
                AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
                //Debug.LogFormat("Added {0}", subAsset, mainAsset);
            }
        }

        private static void TraverseSubAssets(ScriptableObject mainAsset, Action<ScriptableObject> visit, RecursiveObject fieldObj = null)
        {
            if (fieldObj != null)
            {
                // Add object as sub-asset if possible
                if (fieldObj.IncludeSelf)
                {
                    var objType = fieldObj.Value.GetType();

                    // Direct child
                    if (objType.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        visit((ScriptableObject)fieldObj.Value);
                    }
                    // Array / List
                    else if (
                        ReflectionUtil.IsArrayOf(objType, typeof(ScriptableObject)) ||
                        ReflectionUtil.IsListOf(objType, typeof(ScriptableObject)))
                    {
                        foreach (var sa in (IEnumerable)fieldObj.Value)
                            TraverseSubAssets(mainAsset, visit, new RecursiveObject(sa, true));

                        return; // skip fields of list and array
                    }
                }

                // Follow all sub fields that are marked as SubAsset
                foreach (var fieldInfo in fieldObj.Value.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (fieldInfo.GetCustomAttributes(typeof(SubAssetAttribute), false).Length > 0)
                        TraverseSubAssets(mainAsset, visit, new RecursiveObject(fieldInfo.GetValue(fieldObj.Value), true));
                }
            }
            else
            {
                TraverseSubAssets(mainAsset, visit, new RecursiveObject(mainAsset, false));
            }
        }

        private static void AddAllSubAssets(ScriptableObject mainAsset)
        {
            TraverseSubAssets(mainAsset, sub => AddSubAsset(mainAsset, sub));
        }
    }
}
