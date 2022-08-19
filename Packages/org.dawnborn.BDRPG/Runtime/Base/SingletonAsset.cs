using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BDRPG
{
    public static class SingletonAsset
    {
        public static string GetAssetPathForType(Type t) => $"Assets/BDRPG/{t.Name}.asset";
        public static UnityEngine.Object AcquireAsset(Type t, string assetPath = default)
        {
#if UNITY_EDITOR
            assetPath ??= GetAssetPathForType(t);
            var instance = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, t);

            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance(t);
                string directoryPath = Path.GetDirectoryName(assetPath);
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                UnityEditor.AssetDatabase.Refresh();
                UnityEditor.AssetDatabase.CreateAsset(instance, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }

            // Changing the preloaded assets is only effective if the editor is not in play mode
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var preloadedAssets = new List<UnityEngine.Object>(UnityEditor.PlayerSettings.GetPreloadedAssets());
                if (!preloadedAssets.Contains(instance))
                {
                    preloadedAssets.Add(instance);
                    UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }
            return instance;
#else
            return null;
#endif
        }
    }
    /// Good idea to call me with `[UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]` & `[UnityEditor.InitializeOnLoad]`
    public static class SingletonAsset<T> where T : ScriptableObject
    {
        static readonly string assetPath = SingletonAsset.GetAssetPathForType(typeof(T));
        static T instance;
        public static T Instance => instance ??= (T)SingletonAsset.AcquireAsset(typeof(T), assetPath);
        /// Provides a utility for the actual scriptableobject's enable method.
        public static T SetIfUnset(T thiz)
        {
            if (instance == null) instance = thiz;
            else if (instance != thiz) Debug.LogWarning(
                $"Singleton {typeof(T)} has loaded two instances {instance.GetInstanceID()} != {thiz.GetInstanceID()}!"
            );
            return instance;
        }
    }
}
