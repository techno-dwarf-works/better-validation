using Better.Singletons.Runtime;
using Better.Validation.EditorAddons.Utility;
using UnityEngine;

namespace Better.Validation.EditorAddons.ContextResolver
{
    public class SceneResolver : PocoSingleton<SceneResolver>, IPathResolver
    {
        public string Resolve(Object obj)
        {
            return obj.FullPath();
        }
    }
}