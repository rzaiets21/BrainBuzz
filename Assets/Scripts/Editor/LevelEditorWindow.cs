using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelEditorWindow : EditorWindow
{
    private LevelData _levelData;
    
    [MenuItem("Window/Levels/Edit level info")]
    public static void ShowWindow()
    {
        var window = GetWindow<LevelEditorWindow>();
        window.titleContent = new GUIContent("Levels Editor");
    }
    
    public void CreateGUI()
    {
        var root = rootVisualElement;

        var json = new ObjectField("Json levels");
        json.objectType = typeof(TextAsset);
        json.name = "ObjectField";
        Label label = new Label("Hello World!");
        root.Add(label);
        root.Add(json);

        Button button = new Button();
        button.name = "LoadLevelInfo";
        button.text = "Button";
        button.clicked += LoadLevelInfo;
        root.Add(button);
    }

    private void ReloadGUI(LevelData levelData)
    {
        var root = rootVisualElement;
        var list = root.Q<VisualElement>("LevelList");
        if (list == null)
        {
            list = new VisualElement()
            {
                name = "LevelList"
            };
            root.Add(list);
        }
        
        list.Clear();
        // var listView = new ListView(levelData.Questions);
        
        // foreach (var level in levelData.Questions)
        // {
        //     var textField = new TextField(level.Answer)
        //     {
        //         name = level.Answer,
        //         value = level.ImageUrl
        //     };
        //     
        //     list.Add(textField);
        // }
        
        var button = root.Q<Button>("SaveLevelInfo");
        if (button == null)
        {
            button = new Button()
            {
                name = "SaveLevelInfo",
                text = "Save Level Info",
            };
            button.clicked += SaveLevelInfo;
            root.Add(button);
        }
    }
    
    private void LoadLevelInfo()
    {
        var root = rootVisualElement;
        var objectField = root.Q("ObjectField") as ObjectField;
        if (objectField?.value != null && objectField?.value is TextAsset)
        {
            var json = string.Empty;
            var textAssetName = objectField.value.name;
            
            using (FileStream fs = new FileStream($"Assets/Resources/Levels/{textAssetName}.json", FileMode.Open)){
                using (StreamReader reader = new StreamReader(fs))
                {
                    json = reader.ReadToEnd();
                }
            }
            
            _levelData = JsonConvert.DeserializeObject<LevelData>(json);
            ReloadGUI(_levelData);
        }
    }
    
    private void SaveLevelInfo()
    {
        var root = rootVisualElement;
        var objectField = root.Q("ObjectField") as ObjectField;
        if (objectField?.value != null && objectField?.value is TextAsset)
        {
            var json = string.Empty;
            var textAssetName = objectField.value.name;
            var list = root.Q<VisualElement>("LevelList");

            // foreach (var levelData in _levelData.Questions)
            // {
            //     var textField = list.Q<TextField>(levelData.Answer);
            //     levelData.ImageUrl = textField.value;
            // }
            
            json = JsonConvert.SerializeObject(_levelData);
            using (FileStream fs = new FileStream($"Assets/Resources/Levels/{textAssetName}.json", FileMode.OpenOrCreate)){
                using (StreamWriter writer = new StreamWriter(fs)){
                    writer.Write(json);
                }
            }
        }
    }
}
