using System.Collections.Generic;

namespace Better.Validation.EditorAddons.PreBuildValidation
{
    public interface IBuildValidationStep
    {
        public List<ValidationCommandData> GatherValidationData(ValidatorCommands commands);
    }
}