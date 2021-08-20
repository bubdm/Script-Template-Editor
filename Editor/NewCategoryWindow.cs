using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NewCategoryWindow : EditorWindow {

    private static Action<int, string> OnCreateCategory;

    private int contextMenuOrderIndex;
    private string orderIndexString;
    private string[] orderIndexes;
    private string textField;

    private static readonly Rect buttonPosition = new Rect(540, 80, 60, 20);

    public static void OpenWindow(Rect parentWindowPosition, Action<int, string> onCreateCategory) {
        NewCategoryWindow window = GetWindow<NewCategoryWindow>("New Category");
        
        window.minSize = new Vector2(600, 100);
        window.maxSize = new Vector2(600, 100);
        
        window.LooselyDockToWindowCorner(parentWindowPosition, ScriptTemplateExtensions.DockingPosition.TopRight);

        OnCreateCategory += onCreateCategory;
    }

    private void OnEnable() {
        int[] intOrderValues = Enumerable.Range(70, 30).ToArray();

        orderIndexes = new string[intOrderValues.Length];
        
        for (int i = 0; i < intOrderValues.Length; i++)
            orderIndexes[i] = intOrderValues[i].ToString();
    }
    
    private void OnGUI() {
        
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        contextMenuOrderIndex = EditorGUILayout.Popup("Menu Sort Order", contextMenuOrderIndex, orderIndexes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        textField = EditorGUILayout.TextField("Category name", textField, GUILayout.ExpandWidth(true)); 
        EditorGUILayout.EndHorizontal();
        
        if (GUI.Button(buttonPosition, "Create")) {

            string newCategory = String.IsNullOrEmpty(textField) ? "" : orderIndexes[contextMenuOrderIndex] + "-" + textField;
            
            if (newCategory.Equals(""))
                Debug.LogWarning("Category not valid: field was empty");
            else
                OnCreateCategory.Invoke(int.Parse(orderIndexes[contextMenuOrderIndex]), newCategory);

            OnCreateCategory = null;
            Close();
            
        }

    }

}
