using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptWizard : EditorWindow {

    private static readonly Vector2 WindowSize = new Vector2(600, 130);
    private static Action<TextAsset> OnCreateTemplate;
    private GUIStyle _headerStyle;
    
    private string _menuName;
    private string _defaultClassName;
    
    private List<string> _internalCategoryList; //To skip doing move operations on primitive array

    private string[] _categoryList;
    private int _categoryListIndex;
    
    public static void OpenWindow(Rect parentWindowRect, Action<TextAsset> onCreateTemplate) {
        ScriptWizard window = GetWindow<ScriptWizard>();
        
        window.minSize = WindowSize;
        window.maxSize = WindowSize;
        
        window.LooselyDockToWindowCorner(parentWindowRect, ScriptTemplateExtensions.DockingPosition.TopRight);
        OnCreateTemplate += onCreateTemplate;
    }

    private void OnEnable() {
        _headerStyle = new GUIStyle {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 25,
            fontStyle = FontStyle.BoldAndItalic,
            padding = new RectOffset(20, 20, 20, 20),
            normal = {textColor = new Color(100, 30, 200, .9f)}
        };
        
        _internalCategoryList = new List<string>();
        
        PopulateCategoryList(ref _categoryList);
    }

    private void OnGUI() {
        DrawHeader();
        DrawToolBar();

        if (_categoryList.Length > 0)
            _categoryListIndex = EditorGUILayout.Popup("Category", _categoryListIndex, _categoryList);
        else {
            GUI.enabled = false;
            EditorGUILayout.TextField("Category: ", "No available categories");
            GUI.enabled = true;
            _categoryListIndex = -1;
        }
            
        _menuName = EditorGUILayout.TextField("Menu Name", _menuName);
        _defaultClassName = EditorGUILayout.TextField("Default Class Name", _defaultClassName);

        GUI.enabled = AllFieldsHaveValues();
        
        if (GUILayout.Button("Create template", GUI.skin.button)) {
            CreateTemplate();
        }
    }

    private void CreateTemplate() {
        string newFileName = $"{_categoryList[_categoryListIndex]}__{_menuName}-{_defaultClassName}.cs.txt";

        TextAsset newAsset = new TextAsset(null);

        AssetDatabase.CreateAsset(newAsset, ScriptTemplateEditor.SCRIPT_TEMPLATE_FOLDER_PATH + "/" + newFileName);

        OnCreateTemplate?.Invoke(newAsset);
        OnCreateTemplate = null;
        Close();
    }


    private void DrawHeader() {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("New Template Wizard", _headerStyle, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
    }
    
    private bool AllFieldsHaveValues() {
        if (_menuName == null || _defaultClassName == null)
            return false;

        return _menuName.Length > 0 && _defaultClassName.Length > 0 && _categoryListIndex != -1;
    }


    private void PopulateCategoryList(ref string[] categoryList) {
        
        FileInfo[] files = new DirectoryInfo("Assets/ScriptTemplates").GetFiles();
        
        for (int i = 0; i < files.Length; i += 2) { //Skip every other index to avoid invisible meta files. 
            
            string currentFileName = files[i].Name;
            
            string intOrderIndex = currentFileName.Substring(0, currentFileName.IndexOf('-'));
            string category = currentFileName.Substring(intOrderIndex.Length, currentFileName.IndexOf('_') - 2);
            
            _internalCategoryList.Add(intOrderIndex + category);
        }
        
        categoryList = _internalCategoryList.ToArray();

        _internalCategoryList.Clear();

    }

    private void DrawToolBar() {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("New Category", GUILayout.Width(100))) 
            NewCategoryWindow.OpenWindow(position, AddToCategoryList);
        
        EditorGUILayout.EndHorizontal();
    }

    //TODO _internalCategoryList.AddRange lägger till dubbla värden hela tiden, sluta upp med det.
    private void AddToCategoryList(int sortIndex, string newCategory) {
        
        _internalCategoryList.AddRange(_categoryList);

        int index;
        
        for (index = 0; index < _internalCategoryList.Count; index++) {
            int currentSortOrderIndex = int.Parse(_internalCategoryList[index].Substring(0, _internalCategoryList[index].IndexOf('-')));

            if (currentSortOrderIndex < sortIndex)
                continue; //Current sort index is smaller, keep looking until sortIndex is higher
            
            break; //Insert point, sortIndex is higher than all previous 
        }
        
        _internalCategoryList.Insert(index, newCategory);
        
        _categoryList = _internalCategoryList.ToArray();
        _categoryListIndex = index;
    }


}
