using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
    
[CustomEditor(typeof(GenerateMaze))]
public class GenrateMazeEditor : Editor
{
    private GenerateMaze generateMaze;
    private SerializedProperty floor;
    private SerializedProperty cellPrefab;
    private SerializedProperty roomPrefab;
    private SerializedProperty parent;
    private SerializedProperty roomParent;
    private SerializedProperty tileSize;
    private SerializedProperty hallwayMat;
    private SerializedProperty emptyMat;
    private SerializedProperty roomMat;
    private SerializedProperty roomSizeMinMax;

    public void OnEnable()
    {
        generateMaze = (GenerateMaze)target;
        floor = serializedObject.FindProperty("floor");
        cellPrefab = serializedObject.FindProperty("cellPrefab");
        roomPrefab = serializedObject.FindProperty("roomPrefab");
        parent = serializedObject.FindProperty("parent");
        roomParent = serializedObject.FindProperty("roomParent");
        tileSize = serializedObject.FindProperty("tileSize");
        hallwayMat = serializedObject.FindProperty("hallwayMat");
        emptyMat = serializedObject.FindProperty("emptyMat");
        roomMat = serializedObject.FindProperty("roomMat");
        roomSizeMinMax = serializedObject.FindProperty("roomSizeMinMax");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(floor);
        EditorGUILayout.PropertyField(cellPrefab);
        EditorGUILayout.PropertyField(roomPrefab);
        EditorGUILayout.PropertyField(parent);      
        EditorGUILayout.PropertyField(roomParent);
        EditorGUILayout.PropertyField(tileSize);

        EditorGUILayout.PropertyField(hallwayMat);
        EditorGUILayout.PropertyField(roomMat);
        EditorGUILayout.PropertyField(emptyMat);

        EditorGUILayout.PropertyField(roomSizeMinMax);

        if (GUILayout.Button("generate"))
        {
            generateMaze.DrawMaze();
        }

        if (GUILayout.Button("reset")) 
        {
            generateMaze.ClearMaze();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
