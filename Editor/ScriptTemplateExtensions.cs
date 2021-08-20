using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptTemplateExtensions {

    public enum DockingPosition {
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft
    }
    
    public static void WriteToTextAsset(this EditorWindow window, string path, string text) {
        StreamWriter file = new StreamWriter(path, false);
        file.WriteLine(text);
        file.Close();
        AssetDatabase.Refresh();
    }

    public static void LooselyDockToWindowCorner(this EditorWindow window, Rect otherWindow, DockingPosition dockingPosition) {

        Vector2 cornerPosition = Vector2.zero;
        
        switch (dockingPosition) {
            case DockingPosition.TopRight:
                cornerPosition = new Vector2(otherWindow.xMax, otherWindow.yMin);
                break;
            case DockingPosition.TopLeft:
                cornerPosition = new Vector2(otherWindow.xMin, otherWindow.yMin);
                break;
            case DockingPosition.BottomLeft:
                cornerPosition = new Vector2(otherWindow.xMin, otherWindow.yMax);
                break;
            case DockingPosition.BottomRight:
                cornerPosition = new Vector2(otherWindow.xMax, otherWindow.yMax);
                break;
            
        }
        
        window.position = new Rect(cornerPosition, window.maxSize);
        
    }
}
