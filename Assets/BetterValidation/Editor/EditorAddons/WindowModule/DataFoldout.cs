using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Better.Validation.EditorAddons.WindowModule
{
    public class DataFoldout : Foldout
    {
        private readonly List<ValidationCommandData> _datas;

        public DataFoldout(List<ValidationCommandData> datas)
        {
            _datas = datas;
        }

        public void UpdateStyle(ValidationCommandData data)
        {
            value = _datas.Contains(data);
        }
    }
}