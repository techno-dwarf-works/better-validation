using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace Better.Validation.EditorAddons.Utility
{
    public class FoldoutHeaderGroupScope : GUI.Scope
    {
        public bool IsFolded { get; protected set; }

        public FoldoutHeaderGroupScope(bool foldout,
            string content,
            [DefaultValue("EditorStyles.foldoutHeader")]
            GUIStyle style = null,
            Action<Rect> menuAction = null, GUIStyle menuIcon = null)
        {
            IsFolded = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, content, style, menuAction, menuIcon);
        }

        protected override void CloseScope() => EditorGUILayout.EndFoldoutHeaderGroup();
    }
}