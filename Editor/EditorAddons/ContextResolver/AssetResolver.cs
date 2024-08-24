using Better.Singletons.Runtime;
using Better.Validation.EditorAddons.Utility;
using UnityEditor;
using UnityEngine;

namespace Better.Validation.EditorAddons.ContextResolver
{
    public class AssetPathResolver : PocoSingleton<AssetPathResolver>, IPathResolver
    {
        public string Resolve(Object obj)
        {
            if (obj is GameObject gameObject)
            {
                var hasParent = gameObject.transform.parent != null;
                return AssetDatabase.GetAssetPath(obj) + (hasParent ? obj.FullPathNoRoot() : string.Empty);
            }

            return obj.FullPathNoRoot();
        }
    }
}