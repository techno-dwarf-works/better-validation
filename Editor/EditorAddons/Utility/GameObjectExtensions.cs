using System;
using System.Collections.Generic;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Better.Validation.EditorAddons.Utility
{
    public static class GameObjectExtensions
    {
        private static readonly IReadOnlyList<GameObject> Empty = new List<GameObject>();

        public static string FullPath(this GameObject self)
        {
            if (self.IsNullOrDestroyed())
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(self));
                return string.Empty;
            }

            return self.transform.parent.IsNullOrDestroyed()
                ? self.name
                : FullPath(self.transform.parent.gameObject) + "/" + self.name;
        }

        public static string FullPath(this Object self)
        {
            if (self.IsNullOrDestroyed())
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(self));
                return string.Empty;
            }

            if (!PrefabUtility.IsPartOfPrefabAsset(self))
            {
                switch (self)
                {
                    case GameObject go:
                        return FullPath(go);
                    case Component component:
                        return FullPath(component.gameObject);
                }
            }

            return AssetDatabase.GetAssetPath(self) + "/" + self.name;
        }

        public static string FullPathNoRoot(this GameObject self)
        {
            if (self.IsNullOrDestroyed())
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(self));
                return string.Empty;
            }

            return self.transform.parent.IsNullOrDestroyed()
                ? string.Empty
                : FullPathNoRoot(self.transform.parent.gameObject) + "/" + self.name;
        }

        public static string FullPathNoRoot(this Object self)
        {
            if (self.IsNullOrDestroyed())
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(self));
                return string.Empty;
            }

            if (!PrefabUtility.IsPartOfPrefabAsset(self))
            {
                switch (self)
                {
                    case GameObject go:
                        return FullPathNoRoot(go);
                    case Component component:
                        return FullPathNoRoot(component.gameObject);
                }
            }

            return AssetDatabase.GetAssetPath(self) + "/" + self.name;
        }

        public static IReadOnlyList<GameObject> GetAllChildren(this GameObject self)
        {
            if (self.IsNullOrDestroyed())
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(self));
                return Empty;
            }

            var children = new List<GameObject>();

            var queue = new Queue<Transform>();
            queue.Enqueue(self.transform);
            while (queue.Count > 0)
            {
                var transform = queue.Dequeue();

                children.Add(transform.gameObject);

                foreach (Transform item in transform)
                    queue.Enqueue(item);
            }

            return children;
        }

        public static IReadOnlyList<Object> GetAllChildren(this Object self)
        {
            if (self.IsNullOrDestroyed())
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(self));
                return Empty;
            }

            if (self is GameObject gameObject) return gameObject.GetAllChildren();

            return new[] { self };
        }
    }
}