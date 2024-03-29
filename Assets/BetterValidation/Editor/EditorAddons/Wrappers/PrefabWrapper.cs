﻿using Better.EditorTools.EditorAddons.Helpers;
using Better.EditorTools.EditorAddons.Helpers.Caching;
using UnityEditor;

namespace Better.Validation.EditorAddons.Wrappers
{
    public class PrefabWrapper : NotNullWrapper
    {
        public override CacheValue<string> Validate()
        {
            var baseResult = base.Validate();
            if (!baseResult.IsValid)
            {
                return baseResult;
            }

            var obj = Property.objectReferenceValue;
            if (!PrefabUtility.IsPartOfPrefabAsset(obj))
            {
                var str = DrawersHelper.BeautifyFormat(Property.displayName);
                if (!PrefabUtility.IsPartOfNonAssetPrefabInstance(obj))
                {
                    return GetNotValidCache($"Object in {str} field is not prefab");
                }

                return GetNotValidCache($"Object in {str} field is prefab instance in scene");
            }

            return GetClearCache();
        }
    }
}