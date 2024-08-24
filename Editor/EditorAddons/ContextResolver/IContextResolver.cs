using UnityEngine;

namespace Better.Validation.EditorAddons.ContextResolver
{
    public interface IPathResolver
    {
        public string Resolve(Object obj);
    }
}