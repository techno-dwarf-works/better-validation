using System;
using Better.Attributes.EditorAddons.Drawers.Validation.Handlers;
using Better.Attributes.Runtime.Validation;
using Better.Validation.EditorAddons.ContextResolver;
using Better.Validation.EditorAddons.Iteration;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Better.Validation.EditorAddons
{
    public class ValidationCommandData
    {
        private Func<ValidationCommandData, string, string> _compiler;
        public IPathResolver PathResolver { get; private set; }
        public Object Target { get; private set; }
        public SerializedObject Context { get; }
        public SerializedProperty Property { get; private set; }
        public string Result { get; private set; }
        public ValidationHandler Handler { get; }
        public ValidationType Type => Handler.Type;
        public bool IsValid { get; private set; }

        public ValidationCommandData(IterationData data, ValidationHandler handler)
        {
            PathResolver = data.PathResolver;
            Context = data.Context;
            Property = data.Property;
            Target = data.Target;
            Handler = handler;
        }

        public void SetResultCompiler(Func<ValidationCommandData, string, string> compiler)
        {
            _compiler = compiler;
        }

        public void Revalidate()
        {
            Context.Update();
            var cache = Handler.Validate();
            IsValid = cache.State;
            if (!cache.State)
            {
                Result = _compiler.Invoke(this, cache.Result);
            }
        }

        public void SetInitialResult(string value)
        {
            Result = value;
        }
    }
}