using System.Collections.Generic;
using Better.Commons.Runtime.Comparers;
using UnityEngine;

namespace Better.Validation.EditorAddons.Comparers
{
    public class GameObjectGroupingComparer : BaseComparer<GameObjectGroupingComparer, Object>, IEqualityComparer<Object>
    {
        public bool Equals(Object x, Object y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            var xGameObject = GetGameObject(x);
            var yGameObject = GetGameObject(y);

            if (xGameObject != null && yGameObject != null)
                return xGameObject == yGameObject;

            var xComponent = GetComponent(x);
            var yComponent = GetComponent(y);

            if (xComponent != null && yComponent != null)
                return xComponent == yComponent;

            var xScriptableObject = GetScriptableObject(x);
            var yScriptableObject = GetScriptableObject(y);

            if (xScriptableObject != null && yScriptableObject != null)
                return xScriptableObject == yScriptableObject;

            return false;
        }

        public int GetHashCode(Object obj)
        {
            if (obj == null)
                return 0;

            var gameObject = GetGameObject(obj);
            if (gameObject != null)
                return gameObject.GetHashCode();

            var component = GetComponent(obj);
            if (component != null)
                return component.GetHashCode();

            var scriptableObject = GetScriptableObject(obj);
            if (scriptableObject != null)
                return scriptableObject.GetHashCode();

            return obj.GetHashCode();
        }

        private GameObject GetGameObject(Object obj)
        {
            if (obj is GameObject gameObject)
                return gameObject;
            if (obj is Component component)
                return component.gameObject;
            return null;
        }

        private Component GetComponent(Object obj)
        {
            if (obj is Component component)
                return component;
            return null;
        }

        private ScriptableObject GetScriptableObject(Object obj)
        {
            if (obj is ScriptableObject scriptableObject)
                return scriptableObject;
            return null;
        }
    }
}