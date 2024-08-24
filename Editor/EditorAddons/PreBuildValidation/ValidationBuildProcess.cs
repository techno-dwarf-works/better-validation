using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Better.Attributes.Runtime.Validation;
using Better.Validation.EditorAddons.Settings;
using Better.Validation.EditorAddons.WindowModule;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Better.Validation.EditorAddons.PreBuildValidation
{
    public class ValidationBuildProcess : IPreprocessBuildWithReport
    {
        private readonly ValidationSettings _settings;
        public int callbackOrder { get; }

        public ValidationBuildProcess()
        {
            _settings = ValidationSettings.Instance;
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            if(_settings.DisableBuildValidation) return;
            var validationCommands = new ValidatorCommands();
            var commandDatas = new List<ValidationCommandData>();

            var validationSteps = _settings.GetSteps();
            foreach (var buildValidationStep in validationSteps)
            {
                commandDatas.AddRange(buildValidationStep.GatherValidationData(validationCommands));
            }

            commandDatas = commandDatas.Where(x => x.Type >= _settings.BuildLoggingLevel).ToList();
            if (!commandDatas.Any()) return;
            
            var str = CreateDialogMessage(commandDatas);
            EditorApplication.Beep();
            if (!EditorUtility.DisplayDialog("Validation failed", str.ToString(), "Ignore", "Resolve"))
            {
                ValidationWindow.OpenWindow(commandDatas);
                throw new BuildFailedException("Pre build validation failed");
            }
        }

        private static StringBuilder CreateDialogMessage(List<ValidationCommandData> commandDatas)
        {
            var str = new StringBuilder("Pre build validation failed.");
            str.AppendLine();
            str.AppendLine("There are:");
            var values = (ValidationType[])Enum.GetValues(typeof(ValidationType));
            for (var index = values.Length - 1; index >= 0; index--)
            {
                var value = values[index];
                var count = commandDatas.Count(x => x.Type == value);
                var appendix = count > 1 ? "s" : "";
                str.AppendFormat("- {0} {1}{2}", count, value.ToString(), appendix);
                str.AppendLine();
            }

            str.AppendLine("Do you want to ignore those issues?");
            str.AppendLine();
            str.AppendLine();
            str.AppendFormat("(You can disable validation in Edit > Project Settings > {0})", ValidationSettingsProvider.Path.Replace("/", " > "));
            return str;
        }
    }
}