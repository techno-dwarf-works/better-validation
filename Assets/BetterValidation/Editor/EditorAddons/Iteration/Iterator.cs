using System;
using System.Collections.Generic;
using Better.Attributes.EditorAddons.Drawers.Validation.Handlers;
using Better.Commons.Runtime.Extensions;
using Better.Validation.EditorAddons.ContextResolver;
using Better.Validation.EditorAddons.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Better.Validation.EditorAddons.Iteration
{
    public static class Iterator
    {
        private static IPathResolver _path;
        private static readonly IterationData CacheData = new IterationData();

        public delegate IEnumerable<ValidationCommandData> OnPropertyIteration(IterationData commandData);

        public static void SetContext(IPathResolver path)
        {
            _path = path;
            CacheData.SetResolver(path);
        }

        public static List<ValidationCommandData> ObjectIteration(Object reference, OnPropertyIteration onPropertyIteration)
        {
            if (reference.IsNullOrDestroyed())
            {
                return new List<ValidationCommandData>();
            }

            var gameObject = reference as GameObject;
            var components = gameObject ? gameObject.GetComponents<Component>() : new[] { reference };
            var commandData = new List<ValidationCommandData>();

            try
            {
                EditorUtility.DisplayProgressBar("Validating components...", "", 0);
                for (var index = 0; index < components.Length; index++)
                {
                    var obj = components[index];
                    if (ValidateMissingReference(reference, obj, gameObject, commandData))
                    {
                        continue;
                    }

                    EditorUtility.DisplayProgressBar("Validating components...", $"Validating {obj.name}...", Mathf.Clamp01(index / (float)components.Length));

                    var so = new SerializedObject(obj);
                    CacheData.SetContext(so);
                    //so.forceChildVisibility = true;
                    so.Update();
                    using (var sp = so.GetIterator())
                    {
                        IterateProperties(onPropertyIteration, sp, commandData);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }


            return commandData;
        }

        private static bool ValidateMissingReference(Object reference, Object obj, GameObject gameObject, List<ValidationCommandData> commandData)
        {
            if (!obj)
            {
                CacheData.SetTarget(reference);
                var missingReference = new ValidationCommandData(CacheData, new MissingComponentHandler(gameObject));
                missingReference.SetResultCompiler((data, result) => $"Missing Component on GameObject: {_path.Resolve(data.Target)}");
                missingReference.Revalidate();
                commandData.Add(missingReference);
                return true;
            }

            CacheData.SetTarget(obj);
            return false;
        }

        public static void IterateProperties(OnPropertyIteration onPropertyIteration, SerializedProperty sp, List<ValidationCommandData> commandData)
        {
            // Initial setup to start iterating through properties
            SerializedProperty property = sp.Copy();
            bool visitChild = true;

            // Iterate through all properties
            if (property.Next(true)) // Enter the first property
            {
                do
                {
                    // Check property type and decide if we should enter child properties
                    if (property.propertyType == SerializedPropertyType.String)
                    {
                        visitChild = false; // Don't visit children if it's a string
                    }
                    else if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
                    {
                        // Handle arrays/lists
                        IterateArrayProperty(onPropertyIteration, property.Copy(), commandData);
                        continue; // Move to the next property after handling the array
                    }
                    else if (property.propertyType == SerializedPropertyType.Generic)
                    {
                        // Handle serializable classes/structs
                        IterateSerializedClass(onPropertyIteration, property.Copy(), commandData);
                        continue; // Move to the next property after handling the class
                    }

                    // Display progress (optional)
                    using (var remainingCopy = property.Copy())
                    {
                        EditorUtility.DisplayProgressBar("Validating property...", $"Validating {remainingCopy.propertyPath}...", 1);
                    }

                    // Cache the current property data and call the delegate
                    CacheData.SetProperty(property.Copy());
                    var list = onPropertyIteration?.Invoke(CacheData);

                    // If the delegate returns data, add it to commandData
                    if (list != null)
                    {
                        commandData.AddRange(list);
                    }
                } while (property.Next(visitChild)); // Move to the next property, visiting children if applicable
            }
        }

        private static void IterateArrayProperty(OnPropertyIteration onPropertyIteration, SerializedProperty arrayProperty, List<ValidationCommandData> commandData)
        {
            int arraySize = arrayProperty.arraySize;

            for (int i = 0; i < arraySize; i++)
            {
                SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);

                // Process the array element
                CacheData.SetProperty(element.Copy());
                var list = onPropertyIteration?.Invoke(CacheData);
                if (list != null)
                {
                    commandData.AddRange(list);
                }

                // If the element is a serializable class, recurse into it
                if (element.propertyType == SerializedPropertyType.Generic || element.propertyType == SerializedPropertyType.ManagedReference)
                {
                    IterateSerializedClass(onPropertyIteration, element.Copy(), commandData);
                }
                else if (element.isArray)
                {
                    // Handle arrays/lists
                    IterateArrayProperty(onPropertyIteration, element.Copy(), commandData);
                }
            }
        }

        private static void IterateSerializedClass(OnPropertyIteration onPropertyIteration, SerializedProperty classProperty, List<ValidationCommandData> commandData)
        {
            SerializedProperty childProperty = classProperty.Copy();
            SerializedProperty endProperty = classProperty.GetEndProperty();

            if (childProperty.Next(true)) // Enter the first child property
            {
                do
                {
                    if (SerializedProperty.EqualContents(childProperty, endProperty))
                        break;

                    // Process the child property
                    CacheData.SetProperty(childProperty.Copy());
                    var list = onPropertyIteration?.Invoke(CacheData);
                    if (list != null)
                    {
                        commandData.AddRange(list);
                    }

                    // Recursively handle nested serializable classes
                    if (!childProperty.isArray && childProperty.propertyType == SerializedPropertyType.Generic)
                    {
                        IterateSerializedClass(onPropertyIteration, childProperty.Copy(), commandData);
                    }
                    else if (childProperty.isArray && childProperty.propertyType == SerializedPropertyType.Generic)
                    {
                        IterateArrayProperty(onPropertyIteration, childProperty.Copy(), commandData);
                    }
                } while (childProperty.NextVisible(false)); // Move to the next visible property
            }
        }


        public static List<ValidationCommandData> ObjectsIteration(IReadOnlyList<Object> objs, OnPropertyIteration onPropertyIteration)
        {
            var list = new List<ValidationCommandData>();
            EditorUtility.DisplayProgressBar("Validating objects...", "", 0);
            for (var index = 0; index < objs.Count; index++)
            {
                var go = objs[index];
                EditorUtility.DisplayProgressBar("Validating objects...", $"Validating {go.FullPath()}...", index / (float)objs.Count);
                //await Task.Yield();
                list.AddRange(ObjectIteration(go, onPropertyIteration));
            }

            EditorUtility.ClearProgressBar();
            return list;
        }
    }
}