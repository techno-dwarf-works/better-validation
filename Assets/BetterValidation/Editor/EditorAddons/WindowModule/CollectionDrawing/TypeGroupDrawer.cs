using System.Collections.Generic;
using Better.Attributes.Runtime.Validation;

namespace Better.Validation.EditorAddons.WindowModule.CollectionDrawing
{
    public class TypeGroupDrawer : GroupDictionaryDrawer<ValidationType, SortedDictionary<ValidationType, List<ValidationCommandData>>>
    {
        public override int Order { get; } = 1;

        public override string GetOptionName()
        {
            return "Type Group";
        }

        protected override SortedDictionary<ValidationType, List<ValidationCommandData>> OnInitialize(List<ValidationCommandData> data)
        {
            var dataDictionary =
                new SortedDictionary<ValidationType, List<ValidationCommandData>>(Comparer<ValidationType>.Create((x, y) => y.CompareTo(x)));
            foreach (var commandData in data)
            {
                var iconType = commandData.Type;
                if (!dataDictionary.TryGetValue(iconType, out var list))
                {
                    list = new List<ValidationCommandData>();
                    dataDictionary.Add(iconType, list);
                }

                list.Add(commandData);
            }

            return dataDictionary;
        }


        protected override string FoldoutName(ValidationType key)
        {
            return key.ToString();
        }
    }
}