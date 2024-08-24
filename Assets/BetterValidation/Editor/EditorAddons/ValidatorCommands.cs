using System.Collections.Generic;
using System.Linq;
using Better.Validation.EditorAddons.ContextResolver;
using Better.Validation.EditorAddons.Iteration;
using Better.Validation.EditorAddons.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Better.Validation.EditorAddons
{
    public class ValidatorCommands
    {
        public List<ValidationCommandData> ValidateAttributesInProject()
        {
            var allAssets = AssetDatabase.GetAllAssetPaths();
            var objects = allAssets.Select(AssetDatabase.LoadAssetAtPath<Object>).Where(obj => obj);
            var objs = objects.SelectMany(obj => obj.GetAllChildren().Where(file => file is ScriptableObject || file is GameObject)).ToArray();

            Iterator.SetContext(AssetPathResolver.Instance);
            return Iterator.ObjectsIteration(objs, IteratorFilter.PropertyIterationWithAttributes);
        }

        public List<ValidationCommandData> ValidateAttributesInCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
            Iterator.SetContext(SceneResolver.Instance);

            var objects = scene.GetRootGameObjects().SelectMany(x => x.GetAllChildren()).ToList();
            return Iterator.ObjectsIteration(objects, IteratorFilter.PropertyIterationWithAttributes);
        }

        public List<ValidationCommandData> ValidateAttributesInAllScenes()
        {
            Iterator.SetContext(SceneResolver.Instance);
            var list = new List<ValidationCommandData>();
            foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
            {
                var sceneReference = EditorSceneManager.OpenScene(scene.path);
                var objects = sceneReference.GetRootGameObjects();

                list.AddRange(Iterator.ObjectsIteration(objects, IteratorFilter.PropertyIterationWithAttributes));
            }

            return list;
        }

        public List<ValidationCommandData> FindMissingInAllScenes()
        {
            Iterator.SetContext(SceneResolver.Instance);
            var list = new List<ValidationCommandData>();
            foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
            {
                var sceneReference = EditorSceneManager.OpenScene(scene.path);
                var objects = sceneReference.GetRootGameObjects();

                list.AddRange(Iterator.ObjectsIteration(objects, IteratorFilter.MissingPropertyIteration));
            }

            return list;
        }


        public List<ValidationCommandData> FindMissingReferencesInCurrentScene()
        {
            var countLoaded = SceneManager.sceneCount;

            Iterator.SetContext(SceneResolver.Instance);
            var list = new List<ValidationCommandData>();
            for (var i = 0; i < countLoaded; i++)
            {
                var objects = SceneManager.GetSceneAt(i).GetRootGameObjects();
                var data = Iterator.ObjectsIteration(objects, IteratorFilter.MissingPropertyIteration);
                list.AddRange(data);
            }

            return list;
        }

        public List<ValidationCommandData> FindMissingReferencesInProject()
        {
            var allAssets = AssetDatabase.GetAllAssetPaths();
            Iterator.SetContext(AssetPathResolver.Instance);

            var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(obj => obj).ToArray();

            return Iterator.ObjectsIteration(objs, IteratorFilter.MissingPropertyIteration);
        }
    }
}