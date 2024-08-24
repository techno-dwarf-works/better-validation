# Better Validation

[![openupm](https://img.shields.io/npm/v/com.uurha.bettervalidation?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.uurha.bettervalidation/)

<p align="center">
<img src="https://github.com/uurha/BetterValidation/assets/22265817/f0b6b50d-0d4c-4646-a37e-24ae1433f926" alt="image" style="width:49%;">
<img src="https://github.com/uurha/BetterValidation/assets/22265817/b5286158-78b2-473b-a2c9-ebcd7544c3f2" alt="image" style="width:49%;">
</p>

The Better Validation package provides attributes to perform validation in scenes and the whole project.

To initiate validation, you can go to `Toolbar > Better > Validation > Open Validation Window`, or it may start automatically prior to building (validation before play mode will be available in the future).

If you wish to disable automatic pre-build validation or change the logging level, you can open the settings at `Toolbar > Better > Validation > Highlight settings` or `Edit > Project Settings > Better > Validation`.

### Available Attributes

This is a list of available attributes for validation in any type of class that supports Unity serialization:

1. **NotNull**: Fails if a `UnityEngine.Object` is null or has a missing reference.

2. **PrefabField**: Inherits from NotNull and fails if the field references a scene object or an object context.

3. **SceneReference**: Inherits from NotNull and fails if the field references a prefab in the Project.

4. **Find**: Searches for a Component of Type provider in the constructor. It has additional settings:
   - **ValidateIfFieldEmpty**: Configures the search only if the field is null or missing.
   - **RequireDirection**: Configures the direction of the search.

5. **DataValidation**: Validates data in the field using the provided method name. It only supports methods in the same class. The method can return one of the following types:
   - `void`: Validation will be called, but nothing will be displayed in the editor or the Validation Window.
   - `bool`: `true` represents a successful validation, while `false` will draw a default error in the editor and the Validation Window.
   - `string`: The returned string is regarded as a failed validation, while an empty string indicates success.

All attributes have a `ValidationType` that represents the importance of the validation result.

## Usage

```c#
[Serializable]
public class TestManaged
{
    [SceneReference]
    [SerializeField] private List<GameObject> _gameObject;

    [Find(typeof(Test), ValidateIfFieldEmpty = true)]
    [SerializeField] private Test _test;

}

public class Test : MonoBehaviour
{
    [SerializeField] private TestManaged _testManaged;
    
    [PrefabField]
    [SerializeField] private List<GameObject> _gameObjects;
    
    [NotNullAttribute]
    [SerializeField] private GameObject _gameObject;
    
    [DataValidation(nameof(ValidateIntValueString), Type = ValidationType.Info)]
    [SerializeField] private int intValue;

    private void ValidateIntValue(int value)
    {
        Debug.Log(value);
    }
    
    private bool ValidateIntValueBool(int value)
    {
        return value == 10;
    }
    
    private string ValidateIntValueString(int value)
    {
        if (ValidateIntValueBool(value))
        {
            return string.Empty;
        }

        return "Wrong value";
    }
}
```


## Install
[How to install](https://github.com/uurha/BetterPluginCollection/wiki/How-to-install)
