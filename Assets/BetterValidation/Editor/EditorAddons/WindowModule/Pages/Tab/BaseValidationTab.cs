using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule
{
    public abstract class BaseValidationTab : VisualElement
    {
        protected ValidatorCommands _commands;

        public event Action<List<ValidationCommandData>> CommandSelected; 
        
        public abstract int Order { get; }
        public abstract string GetTabName();

        public virtual void Initialize()
        {
            _commands = new ValidatorCommands();
        }

        protected void SelectCommands(List<ValidationCommandData> obj)
        {
            CommandSelected?.Invoke(obj);
        }
    }
}