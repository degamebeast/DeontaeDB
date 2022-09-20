using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

//DN = Deontae's Note
namespace SceneSelector.Editor
{
    public class SceneSelector : EditorWindow
    {
        [SerializeField]
        bool isShowAllScene = false;

        [SerializeField]
        bool isUseBuildSetting = false;

        [SerializeField]
        Vector2 scrollPos;

        [SerializeField]
        List<SceneAsset> scenes = new List<SceneAsset>();

        [SerializeField]
        SceneAsset customMainScene;

        [SerializeField]
        SceneAsset targetEditorScene;//DN: this varaibles doesn't do anything lol. I shall give it purpose :)

        static string editModeScene;

        //DN: Reference to scriptable object that will store the given target scene
        [SerializeField]
        EditorDataHolder editData;

        [MenuItem("Window/General/SceneSelector")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SceneSelector));
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += _OnPlayModeStateChanged;
        }

        void OnGUI()
        {
            _GUIHandler();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void _GUIHandler()
        {
            titleContent.text = "SceneSelector";
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            _PlayHandler();
            EditorGUILayout.Space();
            _ShowAllSceneAsset();
            EditorGUI.EndDisabledGroup();
        }

        static void _OnPlayModeStateChanged(PlayModeStateChange state)
        {
            var shouldResetSceneInEditMode = (state == PlayModeStateChange.EnteredEditMode);

            if (shouldResetSceneInEditMode) {
                EditorSceneManager.playModeStartScene = (SceneAsset)AssetDatabase.LoadAssetAtPath(editModeScene, typeof(SceneAsset));
            }
        }

        [MenuItem("Edit/Play first scene %#p")]
        static void StartPlayFirstScene()
        {
            if (EditorApplication.isPlaying) {
                EditorApplication.isPlaying = false;
                return;
            }

            if (EditorBuildSettings.scenes.Length <= 0) {
                EditorUtility.DisplayDialog("Error", "Can't load the first scene in Build Setting.", "OK");
                return;
            }

            var path = EditorBuildSettings.scenes[0].path;
            var objExpectScene = (SceneAsset)AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset));

            if (objExpectScene == null) {
                EditorUtility.DisplayDialog("Error", "No scene has found in Build Setting", "OK");
                return;
            }

            editModeScene = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.playModeStartScene = objExpectScene;

            EditorApplication.isPlaying = true;
        }

        void _PlayHandler()
        {
            GUILayout.Label ("Play", EditorStyles.boldLabel);
            isUseBuildSetting = EditorGUILayout.Toggle("Use Build Setting", isUseBuildSetting);

            if (!isUseBuildSetting) {
                if(editData)
                    editData.usetestScene = EditorGUILayout.Toggle("Use Test Scene", editData.usetestScene);
                customMainScene = (SceneAsset)EditorGUILayout.ObjectField(
                        new GUIContent("Main/Init Scene:"),
                        customMainScene,
                        typeof(SceneAsset),
                        false);
                targetEditorScene = (SceneAsset)EditorGUILayout.ObjectField(
                        new GUIContent("Target/Test Scene:"),
                        targetEditorScene,
                        typeof(SceneAsset),
                        false);
                editData = (EditorDataHolder)EditorGUILayout.ObjectField(
                        new GUIContent("Data Container:"),
                        editData,
                        typeof(EditorDataHolder),
                        false);


            }

            if (GUILayout.Button("Play", GUILayout.Height(40)))
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode) {
                    if (isUseBuildSetting) {
                        if (editData)
                            editData.usetestScene = false;
                        if (EditorBuildSettings.scenes.Length > 0) {

                            var path = EditorBuildSettings.scenes[0].path;
                            var objExpectScene = (SceneAsset)AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset));

                            if (objExpectScene) {
                                customMainScene = objExpectScene;
                            }
                            else {
                                customMainScene = null;
                                EditorUtility.DisplayDialog("Error", "Can't load the first scene in Build Setting.", "OK");
                                isUseBuildSetting = false;
                            }

                        }
                        else {
                            customMainScene = null;
                            EditorUtility.DisplayDialog("Error", "No scene in Build Setting..", "OK");
                            isUseBuildSetting = false;
                        }
                    }

                    if(targetEditorScene != null && editData)
                    {
                        editData.testScene = targetEditorScene.name;
                    }

                    if (customMainScene != null) {
                        editModeScene = EditorSceneManager.GetActiveScene().path;
                        EditorSceneManager.playModeStartScene = customMainScene;
                        EditorApplication.isPlaying = true;
                    }
                    else {
                        EditorUtility.DisplayDialog("Error", "No scene selected for playing.", "OK");
                    }
                }
            }
        }

        void _ShowAllSceneAsset()
        {
            GUILayout.Label("Editor", EditorStyles.boldLabel);
            isShowAllScene = EditorGUILayout.Foldout(isShowAllScene, "Scenes");

            if (isShowAllScene) {

                if (GUILayout.Button("Refresh")) {

                    scenes.Clear();
                    var assetsGUID = AssetDatabase.FindAssets("t:SceneAsset");

                    foreach (var id in assetsGUID) {

                        var path = AssetDatabase.GUIDToAssetPath(id);
                        var asset = (SceneAsset)AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset));

                        scenes.Add(asset);
                    }
                }

                EditorGUILayout.Space();

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

                for (int i = 0; i < scenes.Count; i++) {
                    scenes[i] = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent(""),
                            scenes[i],
                            typeof(SceneAsset),
                            false);
                }

                EditorGUILayout.EndScrollView();
            }
        }
    }

}
