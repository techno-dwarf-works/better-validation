using System.Collections.Generic;
using System.Linq;

namespace Better.Validation.EditorAddons.WindowModule.CollectionDrawing
{
    public class DefaultDrawer : CollectionDrawer
    {
        private List<ValidationCommandData> _dataList;
        
        public override int Order { get; } = 0;
        public override int Count => _dataList.Count;

        public override CollectionDrawer Initialize(List<ValidationCommandData> data)
        {
            _dataList = data;
            CreateDataBoxes();
            name = GetOptionName();
            return this;
        }

        private void CreateDataBoxes()
        {
            Clear();
            foreach (var commandData in _dataList)
            {
                var box = CreateBox(commandData);
                Add(box);
            }
        }

        public override string GetOptionName()
        {
            return "Default";
        }
        
        public override void ClearResolved()
        {
            _dataList.RemoveAll(data =>
            {
                data.Revalidate();
                return data.IsValid;
            });
            
            CreateDataBoxes();
        }

        public override ValidationCommandData GetNext()
        {
            return GetAtDirection(1);
        }
        
        public override ValidationCommandData GetPrevious()
        {
            return GetAtDirection(-1);
        }

        private ValidationCommandData GetAtDirection(int direction)
        {
            if (_currentItem == null)
            {
                _currentItem = _dataList.First();
            }
            else
            {
                var index = _dataList.IndexOf(_currentItem);
                index += direction;
                if (index >= _dataList.Count)
                {
                    index = 0;
                }

                _currentItem = _dataList[index];
            }

            return _currentItem;
        }
        
        public override void Revalidate()
        {
            foreach (var commandData in _dataList)
            {
                commandData.Revalidate();
            }
        }

        public override bool IsValid()
        {
            return _dataList != null;
        }

        protected override List<ValidationCommandData> GetRemaining()
        {
            return _dataList;
        }
    }
}