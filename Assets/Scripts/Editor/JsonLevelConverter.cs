using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class JsonLevelConverter : EditorWindow
{
    [MenuItem("Window/Levels/Create levels from JSON")]
    public static void ShowExample()
    {
        var window = GetWindow<JsonLevelConverter>();
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
        button.name = "button";
        button.text = "Button";
        button.clicked += OnButtonClicked;
        root.Add(button);

        Toggle toggle = new Toggle();
        toggle.name = "toggle";
        toggle.label = "Toggle";
        root.Add(toggle);

        var textField = new TextField();
        textField.name = "TextField";
        root.Add(textField);
    }

    private void OnButtonClicked()
    {
        var root = rootVisualElement;
        var objectField = root.Q("ObjectField") as ObjectField;
        if (objectField?.value != null && objectField?.value is TextAsset json)
        {
            var dictionary = new Dictionary<int, GroupData>();

            var groupId = 0;
            var levelId = 0;

            var groupData = new GroupData();

            var levelData = new LevelData();
            Level verticalQuestion = null;

            var obj = JsonConvert.DeserializeObject(json.text);
            if (obj is JArray array)
            {
                Debug.LogError(array.Count);
                foreach (var jToken in array)
                {
                    // if(string.IsNullOrEmpty(jToken.Value<string>("level_id")))
                    //     continue;
                    
                    //var id = jToken.Value<int>("id");
                    //var length = jToken.Value<int>("length");
                    var group_id = jToken.Value<int>("level_id");
                    var level_id = jToken.Value<int>("group_id");
                    var is_level_image = jToken.Value<int>("level_image");
                    var answer = jToken.Value<string>("answer_text");
                    var question = jToken.Value<string>("clue");
                    //var link = jToken.Value<string>("link");


                    if (group_id != groupId)
                    {
                        groupId = group_id;
                        if (!dictionary.TryGetValue(groupId, out groupData))
                        {
                            groupData = new GroupData()
                            {
                                GroupId = groupId,
                                GroupName = answer,
                                Levels = new List<LevelData>()
                            };
                            
                            dictionary.Add(groupId, groupData);
                        }
                        
                        continue;
                    }
                    
                    if (levelId != level_id)
                    {
                        levelId = level_id;

                        if (verticalQuestion != null)
                        {
                            levelData.Questions.Add(verticalQuestion);
                        }
                        
                        levelData = new LevelData()
                        {
                            GroupId = groupId,
                            LevelId = levelId,
                            Questions = new List<Level>()
                        };
                        
                        groupData.Levels.Add(levelData);
                    }

                    var level = new Level()
                    {
                        LevelImage = is_level_image,
                        Answer = answer,
                        Question = question,
                    };

                    if (is_level_image == 0)
                        verticalQuestion = level;
                    else
                        levelData.Questions.Add(level);
                }
            }
            
            SaveJson(dictionary);
        }
    }

    private void SaveJson(Dictionary<int, GroupData> dictionary)
    {
        foreach (var keyPair in dictionary)
        {
            var levelData = JsonConvert.SerializeObject(keyPair.Value, Formatting.Indented);
            using (FileStream fs = new FileStream($"Assets/Resources/Groups/Group_{keyPair.Key}.json", FileMode.Create)){
                using (StreamWriter writer = new StreamWriter(fs)){
                    writer.Write(levelData);
                }
            }
        }
        
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
    
#region Old

//     private void OnButtonClicked()
//     {
//         var root = rootVisualElement;
//         var objectField = root.Q("ObjectField") as ObjectField;
//         if (objectField?.value != null && objectField?.value is TextAsset json)
//         {
//             var dictionary = new Dictionary<int, LevelData>();
//             var levelId = 1;
//
//             var levelData = new LevelData()
//             {
//                 LevelId = levelId,
//                 Questions = new List<Level>()
//             };
//
//             dictionary.Add(levelId, levelData);
//             
//             var obj = JsonConvert.DeserializeObject(json.text);
//             if (obj is JArray array)
//             {
//                 Debug.LogError(array.Count);
//                 foreach (var jToken in array)
//                 {
//                     if(string.IsNullOrEmpty(jToken.Value<string>("level_id")))
//                         continue;
//                     
//                     var id = jToken.Value<int>("id");
//                     var level_id = jToken.Value<int>("level_id");
//                     var is_level_image = jToken.Value<string>("level_image");
//                     var answer_text = jToken.Value<string>("answer_text");
//                     var image = jToken.Value<string>("image");
//                     var question = jToken.Value<string>("question");
//                     //var link = jToken.Value<string>("link");
//
//                     if(string.IsNullOrEmpty(question))
//                         continue;
//                     
//                     if (levelId != level_id)
//                     {
//                         levelId = level_id;
//                         if (!dictionary.TryGetValue(levelId, out levelData))
//                         {
//                             levelData = new LevelData()
//                             {
//                                 LevelId = levelId,
//                                 Questions = new List<Level>()
//                             };
//                             
//                             dictionary.Add(levelId, levelData);
//                         }
//                     }
//
//                     var level = new Level()
//                     {
//                         QuestionId = id,
//                         LevelImage = int.Parse(is_level_image),
//                         Answer = answer_text,
//                         Question = question,
//                     };
//                     dictionary[levelId].Questions.Add(level);
//                 }
//             }
//             
//             SaveJson(dictionary);
//         }
//     }
//
//     private void SaveJson(Dictionary<int, LevelData> dictionary)
//     {
//         foreach (var keyPair in dictionary)
//         {
//             var levelData = JsonConvert.SerializeObject(keyPair.Value, Formatting.Indented);
//             using (FileStream fs = new FileStream($"Assets/Resources/Levels/Level_{keyPair.Key}.json", FileMode.Create)){
//                 using (StreamWriter writer = new StreamWriter(fs)){
//                     writer.Write(levelData);
//                 }
//             }
//         }
//         
// #if UNITY_EDITOR
//         AssetDatabase.Refresh();
// #endif
//     }

    #endregion
}
