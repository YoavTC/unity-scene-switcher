// The SceneSwitcher class allows scene selection from the Unity editor toolbar
// This was made by Yoav TC: https://github.com/YoavTC
// Uses the UnityToolbarExtender made by marijnz: https://github.com/marijnz/unity-toolbar-extender

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class SceneSwitcher
{
    private static readonly List<string> ScenePathsList = new List<string>();
    private static string selectedSceneName = SELECT_TEXT;
    
    // UI config
    private const string SELECT_TEXT = "Select a scene..";
    private const string LABEL_TEXT = "Scene Switcher:";
    private const int DROPDOWN_WIDTH = 150;
    private const int SEPARATOR_SPACE_WIDTH = 10;

    // Static constructor to initialize toolbar and scene list
    static SceneSwitcher()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI); // Add the GUI to the toolbar
        EditorBuildSettings.sceneListChanged += UpdateSceneList;
        UpdateSceneList(); // Populate the initial scene list

        // Set the selected scene name to the first enabled scene if available
        if (ScenePathsList.Count > 0)
        {
            selectedSceneName = GetSceneNameFromPath(ScenePathsList[0]);
        }
    }

    // Updates the list of ScenePathsList based on enabled ScenePathsList in the Editor Build Settings
    private static void UpdateSceneList()
    {
        ScenePathsList.Clear();
        
        EditorBuildSettingsScene[] editorScenes = EditorBuildSettings.scenes;
        for (int i = 0; i < editorScenes.Length; i++)
        {
            if (editorScenes[i].enabled)
            {
                ScenePathsList.Add(editorScenes[i].path);
            }
        }

        // Set the selected scene name based on the updated scene list
        selectedSceneName = ScenePathsList.Count > 0 ? GetSceneNameFromPath(ScenePathsList[0]) : SELECT_TEXT;
    }

    // Draws the toolbar GUI for scene selection
    private static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();
        GUILayout.Label(LABEL_TEXT);

        // Create a dropdown button to select ScenePathsList
        bool dropdownButton = EditorGUILayout.DropdownButton(
            new GUIContent(selectedSceneName),
            FocusType.Passive,
            GUILayout.Width(DROPDOWN_WIDTH));

        // Show the menu if the dropdown button is clicked
        if (dropdownButton)
        {
            GenericMenu menu = new GenericMenu();
            
            // Display scenes in list
            for (int i = 0; i < ScenePathsList.Count; i++)
            {
                string scenePath = ScenePathsList[i];
                string sceneName = GetSceneNameFromPath(scenePath);
                
                menu.AddItem(
                    new GUIContent($"{i}: {sceneName}"), // Display index and name
                    sceneName == selectedSceneName, // Mark as selected if it's the current scene
                    OnSceneSelectedCallback, // Callback to handle selection
                    scenePath); // Pass the scene path to the callback
            }

            menu.ShowAsContext();
        }

        GUILayout.Space(SEPARATOR_SPACE_WIDTH);
    }
    
    private static void OnSceneSelectedCallback(object userData)
    {
        string scenePath = (string) userData;
        selectedSceneName = GetSceneNameFromPath(scenePath);
        
        if (!string.IsNullOrEmpty(scenePath))
        {
            // Open selected scene
            EditorSceneManager.OpenScene(scenePath);
        }
    }
    
    private static string GetSceneNameFromPath(string path) => Path.GetFileNameWithoutExtension(path);
}