using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptTemplateEditor : EditorWindow {

    [SerializeField] private string text;
    private TextAsset textAsset;

    private SerializedObject so;
    private SerializedProperty propText;
    
    [MenuItem("Window/General/Script Editor &f")]
    private static void OpenWindow() => GetWindow<ScriptTemplateEditor>();

    private bool textChanged;

    private GUIStyle headerStyle;
    
    public static readonly string SCRIPT_TEMPLATE_FOLDER_PATH = "Assets/ScriptTemplates";
    
    private void OnEnable() {

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
        
        if (textAsset == null)
            return;
        
        if (EditorGUI.EndChangeCheck()) 
            LoadNewFile(ref textAsset);
        
        propText.stringValue = EditorGUILayout.TextArea(propText.stringValue, GUILayout.ExpandHeight(true));
        
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
    
    private void LoadNewFile(ref TextAsset newTextAsset) {
        propText.stringValue = newTextAsset.text;
        textAsset = newTextAsset;
    }


    private void DrawHeader() {
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Script Template Editor", headerStyle, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
    }

    private void DrawToolBar() {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("New Template", GUILayout.Width(100))) {
            ScriptWizard.OpenWindow(position);
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool ScriptTemplateFolderExists() {
        return AssetDatabase.IsValidFolder(SCRIPT_TEMPLATE_FOLDER_PATH);
    }
    
    private void CreateScriptTemplateFolder() {
        AssetDatabase.CreateFolder("Assets", "ScriptTemplates");
    }
}
