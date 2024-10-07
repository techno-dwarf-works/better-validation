using System.Collections.Generic;
using Better.Attributes.Runtime.Select;
using Better.Attributes.Runtime.Validation;
using Better.Internal.Core.Runtime;
using Better.ProjectSettings.Runtime;
using Better.Singletons.Runtime.Attributes;
using Better.Validation.EditorAddons.PreBuildValidation;
using UnityEditor;
using UnityEngine;

namespace Better.Validation.EditorAddons.Settings
{
    [ScriptableCreate(SettingsPath)]
    public class ValidationSettings : ScriptableSettings<ValidationSettings>
    {
        public const string SettingsPath = PrefixConstants.BetterPrefix + "/" + nameof(Editor) + "/" + nameof(Validation);
        
        [SerializeField] private bool _disableBuildValidation;
        [SerializeField] private ValidationType _buildLoggingLevel = ValidationType.Warning;

        [Select]
        [SerializeReference] private IBuildValidationStep[] _validationSteps = new IBuildValidationStep[]
            { new ProjectValidationStep(), new AllSceneValidationStep() };

        public ValidationType BuildLoggingLevel => _buildLoggingLevel;

        public bool DisableBuildValidation => _disableBuildValidation;

        public IReadOnlyList<IBuildValidationStep> GetSteps()
        {
            return _validationSteps;
        }
    }
}