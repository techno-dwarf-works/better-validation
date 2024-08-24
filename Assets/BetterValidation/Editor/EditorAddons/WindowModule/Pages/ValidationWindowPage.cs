using System;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule
{
    public abstract class ValidationWindowPage : VisualElement
    {
        public event Action<ValidationWindowPage> PageOpenRequest;
        public abstract void Initialize();
        public abstract void Deconstruct();

        protected virtual void OpenPage(ValidationWindowPage obj)
        {
            PageOpenRequest?.Invoke(obj);
        }
    }
}