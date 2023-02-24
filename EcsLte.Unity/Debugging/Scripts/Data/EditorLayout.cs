using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EcsLte.Unity.Debugging.Scripts.Data
{
    public static class EditorLayout
    {
        private const int DEFAULT_FOLDOUT_MARGIN = 11;

        public static Texture2D LoadTexture(string label)
        {
            string[] array = AssetDatabase.FindAssets(label);
            if (array.Length != 0)
            {
                string text = array[0];
                if (text != null)
                {
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(text));
                }
            }

            return null;
        }

        public static Rect DrawTexture(Texture2D texture)
        {
            if (texture != null)
            {
                Rect aspectRect = GUILayoutUtility.GetAspectRect((float)texture.width / (float)texture.height, GUILayout.ExpandWidth(expand: true));
                GUI.DrawTexture(aspectRect, texture, ScaleMode.ScaleAndCrop);
                return aspectRect;
            }

            return default(Rect);
        }

        public static bool ObjectFieldButton(string label, string buttonText)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(146f));
            if (buttonText.Length > 24)
            {
                buttonText = "..." + buttonText.Substring(buttonText.Length - 24);
            }

            bool result = GUILayout.Button(buttonText, EditorStyles.objectField);
            EditorGUILayout.EndHorizontal();
            return result;
        }

        public static string ObjectFieldOpenFolderPanel(string label, string buttonText, string defaultPath)
        {
            if (ObjectFieldButton(label, buttonText))
            {
                string text = defaultPath ?? "Assets/";
                if (!Directory.Exists(text))
                {
                    text = "Assets/";
                }

                text = EditorUtility.OpenFolderPanel(label, text, string.Empty);
                return text.Replace(Directory.GetCurrentDirectory() + "/", string.Empty);
            }

            return null;
        }

        public static string ObjectFieldOpenFilePanel(string label, string buttonText, string defaultPath, string extension)
        {
            if (ObjectFieldButton(label, buttonText))
            {
                string text = defaultPath ?? "Assets/";
                if (!File.Exists(text))
                {
                    text = "Assets/";
                }

                text = EditorUtility.OpenFilePanel(label, text, extension);
                return text.Replace(Directory.GetCurrentDirectory() + "/", string.Empty);
            }

            return null;
        }

        public static bool MiniButton(string c)
        {
            return miniButton(c, EditorStyles.miniButton);
        }

        public static bool MiniButtonLeft(string c)
        {
            return miniButton(c, EditorStyles.miniButtonLeft);
        }

        public static bool MiniButtonMid(string c)
        {
            return miniButton(c, EditorStyles.miniButtonMid);
        }

        public static bool MiniButtonRight(string c)
        {
            return miniButton(c, EditorStyles.miniButtonRight);
        }

        private static bool miniButton(string c, GUIStyle style)
        {
            GUILayoutOption[] options = ((c.Length != 1) ? new GUILayoutOption[0] : new GUILayoutOption[1] { GUILayout.Width(19f) });
            bool num = GUILayout.Button(c, style, options);
            if (num)
            {
                GUI.FocusControl(null);
            }

            return num;
        }

        public static bool Foldout(bool foldout, string content, int leftMargin = 11)
        {
            return Foldout(foldout, content, EditorStyles.foldout, leftMargin);
        }

        public static bool Foldout(bool foldout, string content, GUIStyle style, int leftMargin = 11)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(leftMargin);
            foldout = EditorGUILayout.Foldout(foldout, content, style);
            EditorGUILayout.EndHorizontal();
            return foldout;
        }

        public static string SearchTextField(string searchString)
        {
            bool changed = GUI.changed;
            GUILayout.BeginHorizontal();
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button(string.Empty, GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                searchString = string.Empty;
            }

            GUILayout.EndHorizontal();
            GUI.changed = changed;
            return searchString;
        }

        public static bool MatchesSearchString(string str, string search)
        {
            string[] array = search.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 0)
            {
                return array.Any(str.Contains);
            }

            return true;
        }

        public static bool DrawSectionHeaderToggle(string header, bool value)
        {
            return GUILayout.Toggle(value, header, Styles.sectionHeader);
        }

        public static void BeginSectionContent()
        {
            EditorGUILayout.BeginVertical(Styles.sectionContent);
        }

        public static void EndSectionContent()
        {
            EditorGUILayout.EndVertical();
        }

        public static Rect BeginVerticalBox()
        {
            return EditorGUILayout.BeginVertical(GUI.skin.box);
        }

        public static void EndVerticalBox()
        {
            EditorGUILayout.EndVertical();
        }
    }

    public static class Styles
    {
        private static GUIStyle _sectionHeader;

        private static GUIStyle _sectionContent;

        public static GUIStyle sectionHeader
        {
            get
            {
                if (_sectionHeader == null)
                {
                    _sectionHeader = new GUIStyle("OL Title");
                }

                return _sectionHeader;
            }
        }

        public static GUIStyle sectionContent
        {
            get
            {
                if (_sectionContent == null)
                {
                    _sectionContent = new GUIStyle("OL Box");
                    _sectionContent.stretchHeight = false;
                }

                return _sectionContent;
            }
        }
    }
}