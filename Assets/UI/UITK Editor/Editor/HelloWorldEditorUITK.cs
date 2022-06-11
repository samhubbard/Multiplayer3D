using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class HelloWorldEditorUITK : EditorWindow
{
    [MenuItem("Window/UI Toolkit/HelloWorldEditorUITK")]
    public static void ShowExample()
    {
        HelloWorldEditorUITK wnd = GetWindow<HelloWorldEditorUITK>();
        wnd.titleContent = new GUIContent("HelloWorldEditorUITK");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/UITK Editor/Editor/HelloWorldEditorUITK.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
    }
}