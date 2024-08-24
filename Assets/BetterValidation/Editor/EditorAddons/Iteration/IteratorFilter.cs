using System.Collections.Generic;
using System.Linq;
using System.Text;
using Better.Attributes.EditorAddons.Drawers.Validation.Extensions;
using Better.Attributes.EditorAddons.Drawers.Validation.Handlers;
using Better.Attributes.Runtime.Validation;
using Better.Commons.EditorAddons.Drawers;
using Better.Commons.EditorAddons.Drawers.Handlers;
using Better.Commons.EditorAddons.Extensions;
using Better.Commons.Runtime.Extensions;

namespace Better.Validation.EditorAddons.Iteration
{
    public static class IteratorFilter
    {
        private static readonly TypeHandlerBinder<PropertyValidationHandler> Wrappers = HandlerBinderRegistry.GetMap<PropertyValidationHandler>();

        private static readonly MissingReferenceHandler ReferenceHandler = new MissingReferenceHandler();

        public static IEnumerable<ValidationCommandData> PropertyIterationWithAttributes(IterationData data)
        {
            var fieldInfo = data.Property.GetFieldInfoAndStaticTypeFromProperty();
            var list = data.Property.GetAttributes<ValidationAttribute>();
            if (fieldInfo == null || list == null) return Enumerable.Empty<ValidationCommandData>();

            var fieldType = fieldInfo.FieldInfo.FieldType;
            if (fieldType.IsArrayOrList())
            {
                fieldType = fieldType.GetCollectionElementType();
            }

            var dataList = new List<ValidationCommandData>();
            foreach (var validationAttribute in list)
            {
                var handler = Wrappers.GetHandler(fieldType, validationAttribute.GetType());

                handler.Setup(data.Property, fieldInfo.FieldInfo, validationAttribute);

                var result = handler.IsSupported() ? handler.Validate() : ValidationHandler.GetClearValue();

                if (result.State) continue;

                dataList.Add(GenerateData(data, handler, result.Result));
            }

            return dataList;
        }

        public static IEnumerable<ValidationCommandData> MissingPropertyIteration(IterationData data)
        {
            var fieldInfo = data.Property.GetFieldInfoAndStaticTypeFromProperty();
            if (fieldInfo == null) return Enumerable.Empty<ValidationCommandData>();
            
            ReferenceHandler.Setup(data.Property, fieldInfo.FieldInfo, null);
            if (!ReferenceHandler.IsSupported()) return null;
            var result = ReferenceHandler.Validate();
            if (result.State) return null;
            var validationCommandData = new ValidationCommandData(data, ReferenceHandler);

            validationCommandData.SetResultCompiler(ObjectCompiler);

            validationCommandData.SetInitialResult(ObjectCompiler(validationCommandData, result.Result));
            return new[]
            {
                validationCommandData
            };
        }

        private static ValidationCommandData GenerateData(IterationData data, PropertyValidationHandler handler, string resultValue)
        {
            var copy = new ValidationCommandData(data, handler.Copy());
            copy.SetResultCompiler(ObjectCompiler);
            copy.SetInitialResult(ObjectCompiler(copy, resultValue));
            return copy;
        }

        private static string ObjectCompiler(ValidationCommandData commandData, string result)
        {
            var property = commandData.Property;
            var path = property.IsArrayElement() ? property.GetArrayPath() : property.displayName;
            var target = commandData.Target;
            var resolvedPath = commandData.PathResolver.Resolve(target);
            var typeName = target.GetType().Name;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Validation failed with: <b>{0}</b>.", result);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("Path: <i><b>{0}</b></i>.", resolvedPath);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("Component: <i><b>{0}</b></i>, Property: <i><b>{1}</b></i>", typeName, path);

            return stringBuilder.ToString();
        }
    }
}