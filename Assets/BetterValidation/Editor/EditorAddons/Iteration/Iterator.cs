﻿using System.Collections.Generic;
using Better.Commons.Runtime.Extensions;
using Better.Validation.EditorAddons.ContextResolver;
using Better.Validation.EditorAddons.Handlers;
using Better.Validation.EditorAddons.Utility;
using UnityEditor;
using UnityEngine;

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
            EditorUtility.DisplayProgressBar("Validating components...", "", 0);
            for (var index = 0; index < components.Length; index++)
            {
                var obj = components[index];
                if (!obj)
                {
                    CacheData.SetTarget(reference);
                    var missingReference = new ValidationCommandData(CacheData, new MissingComponentHandler(gameObject));
                    missingReference.SetResultCompiler((data, result) => $"Missing Component on GameObject: {_path.Resolve(data.Target)}");
                    missingReference.Revalidate();
                    commandData.Add(missingReference);
                    continue;
                }

                CacheData.SetTarget(obj);
                EditorUtility.DisplayProgressBar("Validating components...", $"Validating {obj.name}...",
                    Mathf.Clamp01(index / (float)components.Length));

                var so = new SerializedObject(obj);
                CacheData.SetContext(so);
                //so.forceChildVisibility = true;
                so.Update();
                var sp = so.GetIterator();

                var copy = sp.Copy();
                if (copy.Next(true))
                {
                    var count = copy.CountInProperty();
                    while (sp.Next(true))
                    {
                        var remainingCopy = sp.Copy();
                        EditorUtility.DisplayProgressBar("Validating property...", $"Validating {sp.propertyPath}...",
                            remainingCopy.CountRemaining() / (float)count);
                        CacheData.SetProperty(sp.Copy());
                        var list = onPropertyIteration?.Invoke(CacheData);
                        if (list != null) commandData.AddRange(list);
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            return commandData;
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