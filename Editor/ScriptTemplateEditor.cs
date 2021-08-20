using UnityEditor;
using UnityEngine;

public class ScriptTemplateEditor : EditorWindow {

    [SerializeField] private string text;
    private TextAsset textAsset;

    private SerializedObject so;
    private SerializedProperty propText;

    private string _loadedTemplatePath;
    
    [MenuItem("Window/General/Script Editor &f")]
    private static void OpenWindow() => GetWindow<ScriptTemplateEditor>();

    private bool textChanged;

    private GUIStyle headerStyle;
    
    public static readonly string SCRIPT_TEMPLATE_FOLDER_PATH = "Assets/ScriptTemplates";

    private Vector2 _scroll;
    
    private void OnEnable() {
        
        _loadedTemplatePath = null;
        
        if (ScriptTemplateFolderExists() == false)
            CreateScriptTemplateFolder();
        
        so = new SerializedObject(this);
        propText = so.FindProperty("text");

        headerStyle = new GUIStyle {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 25,
            fontStyle = FontStyle.BoldAndItalic,
            padding = new RectOffset(20, 20, 20, 20),
            normal = {textColor = new Color(100, 30, 200, .9f)}
        };
    }

    private void OnGUI() {
        DrawHeader();
        DrawToolBar();
        EditorGUI.BeginChangeCheck();
        
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        textAsset = EditorGUILayout.ObjectField("Current Template", textAsset, typeof(TextAsset), false) as TextAsset;
        GUILayout.EndHorizontal();

        if (textAsset == null) {
            _loadedTemplatePath = null;
            return;
        }
            
        
        if (EditorGUI.EndChangeCheck()) 
            LoadNewFile(textAsset);


        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        
        EditorGUI.BeginChangeCheck(); //Check if text has been edited
        
        propText.stringValue = EditorGUILayout.TextArea(propText.stringValue, GUILayout.ExpandHeight(true));
        
        if (EditorGUI.EndChangeCheck())
            textChanged = true;
        
        EditorGUILayout.EndScrollView();


        using (new EditorGUILayout.HorizontalScope(GUI.skin.textField)) {

            GUI.enabled = textChanged;

            if (GUILayout.Button("Save")) {
                this.WriteToTextAsset(AssetDatabase.GetAssetPath(textAsset), propText.stringValue);
                AssetDatabase.Refresh();
                textChanged = false;
            }
            
            GUI.enabled = true;

            if (GUILayout.Button("Cancel"))
                GetWindow<ScriptTemplateEditor>().Close();
            
        }
        
    }
    
    private void LoadNewFile(TextAsset newTextAsset) {
        propText.stringValue = newTextAsset.text;
        textAsset = newTextAsset;
        _loadedTemplatePath = AssetDatabase.GetAssetPath(textAsset);
    }


    private void DrawHeader() {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Script Template Editor", headerStyle, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
    }

    private void DrawToolBar() {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("New Template", GUILayout.Width(110)))
            ScriptWizard.OpenWindow(position, LoadNewFile);

        GUI.enabled = _loadedTemplatePath != null;
        if (GUILayout.Button("Delete Template", GUILayout.Width(110)))
            DeleteLoadedTemplate();
        GUI.enabled = true;
        
        EditorGUILayout.EndHorizontal();
    }

    private bool ScriptTemplateFolderExists() {
        return AssetDatabase.IsValidFolder(SCRIPT_TEMPLATE_FOLDER_PATH);
    }
    
    private void CreateScriptTemplateFolder() {
        AssetDatabase.CreateFolder("Assets", "ScriptTemplates");
    }

    private void DeleteLoadedTemplate() {
        bool deleteConfirmed = EditorUtility.DisplayDialog("Delete Template", "Are you sure you want to delete " + textAsset.name + "?", "Yes", "No");

        if (deleteConfirmed) {
            AssetDatabase.DeleteAsset(_loadedTemplatePath);
            textAsset = null;
            _loadedTemplatePath = null;
        }
        
    }
}
