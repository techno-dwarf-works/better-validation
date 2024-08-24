using System;
using System.Collections.Generic;
using Better.Commons.EditorAddons.Utility;
using Better.Commons.Runtime.Interfaces;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule.CollectionDrawing
{
    public abstract class CollectionDrawer : VisualElement, ICopyable<CollectionDrawer>
    {
        protected ValidationCommandData _currentItem = null;
        protected event Action<ValidationCommandData> CurrentUpdated;
        public abstract int Count { get; }

        public abstract int Order { get; }

        public abstract string GetOptionName();

        public abstract CollectionDrawer Initialize(List<ValidationCommandData> data);
        
        public void Copy(CollectionDrawer source)
        {
            UpdateCurrent(source._currentItem);
            Initialize(source.GetRemaining());
        }

        protected DataBox CreateBox(ValidationCommandData data)
        {
            var box = new DataBox(data);
            box.UpdateStyle(_currentItem);
            box.Selected += ShowClicked;
            CurrentUpdated += box.UpdateStyle;
            return box;
        }

        private void ShowClicked(DataBox dataBox)
        {
            var data = dataBox.Data;
            SelectionUtility.OpenReference(data.Target);
            UpdateCurrent(data);
        }

        public void UpdateCurrent(ValidationCommandData data)
        {
            _currentItem = data;
            CurrentUpdated?.Invoke(_currentItem);
        }

        public abstract void ClearResolved();

        public abstract ValidationCommandData GetNext();
        public abstract ValidationCommandData GetPrevious();
        public abstract void Revalidate();
        public abstract bool IsValid();

        protected abstract List<ValidationCommandData> GetRemaining();
    }
}