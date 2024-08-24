using System.Collections.Generic;
using System.Linq;
using Better.Commons.Runtime.Extensions;

namespace Better.Validation.EditorAddons.WindowModule.CollectionDrawing
{
    public abstract class GroupDictionaryDrawer<TKey, TDictionary> : CollectionDrawer
        where TDictionary : class, IDictionary<TKey, List<ValidationCommandData>>, new()
    {
        private TDictionary _dataDictionary = null;
        private int _count;

        public override int Count => _count;

        public override ValidationCommandData GetNext()
        {
            return GetAtDirection(1);
        }

        public override ValidationCommandData GetPrevious()
        {
            return GetAtDirection(-1);
        }

        public override CollectionDrawer Initialize(List<ValidationCommandData> data)
        {
            if (data.IsNullOrEmpty())
            {
                _dataDictionary = new TDictionary();
                _count = _dataDictionary.Count;
            }
            else
            {
                _dataDictionary = OnInitialize(data);
                _count = _dataDictionary.Sum(x => x.Value.Count);
            }

            CreateDataBoxes();
            name = GetOptionName();
            return this;
        }

        protected abstract TDictionary OnInitialize(List<ValidationCommandData> data);

        protected abstract string FoldoutName(TKey key);

        public void CreateDataBoxes()
        {
            foreach (var (key, value) in _dataDictionary)
            {
                var foldoutGroup = new DataFoldout(value);
                foldoutGroup.name = FoldoutName(key);
                foldoutGroup.UpdateStyle(_currentItem);

                CurrentUpdated += foldoutGroup.UpdateStyle;
                
                foreach (var validationCommandData in value)
                {
                    var dataBox = CreateBox(validationCommandData);
                    foldoutGroup.Add(dataBox);
                }

                Add(foldoutGroup);
            }
        }

        private ValidationCommandData GetAtDirection(int direction)
        {
            if (_currentItem == null)
            {
                _currentItem = _dataDictionary.First().Value.First();
            }
            else
            {
                var list = _dataDictionary.Values.SelectMany(x => x).ToList();

                var index = list.IndexOf(_currentItem);
                index += direction;
                if (index >= list.Count)
                {
                    index = 0;
                }

                _currentItem = list[index];
            }

            return _currentItem;
        }

        public override void ClearResolved()
        {
            foreach (var keyValue in _dataDictionary.Values)
            {
                keyValue.RemoveAll(data =>
                {
                    data.Revalidate();
                    return data.IsValid;
                });
            }

            _count = _dataDictionary.Sum(x => x.Value.Count);
        }


        public override void Revalidate()
        {
            foreach (var list in _dataDictionary.Values)
            {
                foreach (var data in list)
                {
                    data.Revalidate();
                }
            }
        }

        public override bool IsValid()
        {
            return _dataDictionary != null;
        }

        protected override List<ValidationCommandData> GetRemaining()
        {
            if (!IsValid())
            {
                return new List<ValidationCommandData>();
            }

            return _dataDictionary.Values.SelectMany(x => x).ToList();
        }
    }
}