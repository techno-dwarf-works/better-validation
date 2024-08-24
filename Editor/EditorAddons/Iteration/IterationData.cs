using Better.Validation.EditorAddons.ContextResolver;
using UnityEditor;
using UnityEngine;

namespace Better.Validation.EditorAddons.Iteration
{
    public class IterationData
    {
        public Object Target { get; private set; }

        public IPathResolver PathResolver { get; private set; }

        public SerializedProperty Property { get; private set; }
        public SerializedObject Context { get; private set; }


        public void SetResolver(IPathResolver path)
        {
            PathResolver = path;
        }

        public void SetTarget(Object target)
        {
            Target = target;
        }

        public void SetProperty(SerializedProperty serializedProperty)
        {
            Property = serializedProperty;
        }

        public void SetContext(SerializedObject serializedObject)
        {
            Context = serializedObject;
        }
    }
}