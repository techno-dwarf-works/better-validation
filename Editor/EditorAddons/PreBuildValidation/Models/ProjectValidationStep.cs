using System;
using System.Collections.Generic;

namespace Better.Validation.EditorAddons.PreBuildValidation
{
    [Serializable]
    public class ProjectValidationStep : IBuildValidationStep
    {
        public List<ValidationCommandData> GatherValidationData(ValidatorCommands commands)
        {
            return commands.ValidateAttributesInProject();
        }
    }
}