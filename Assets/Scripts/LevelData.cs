using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int GroupId;
    public int LevelId;
    public List<Level> Questions;

    public LevelData()
    {
        Questions = new List<Level>();
    }

    public bool IsCompleted()
    {
        var savedModelKey = "Completed-Group_{0}/Level_{1}";

        var key = string.Format(savedModelKey, GroupId, LevelId);
        if (!PlayerPrefs.HasKey(key))
        {
            return false;
        }

        return true;
    }
}

[Serializable]
public class JsonLevel
{
    [JsonProperty("id")] public int QuestionId { get; set; }
    [JsonProperty("level_id")] public int LevelId { get; set; }
    [JsonProperty("level_image")] public int IsLevelImage { get; set; }
    [JsonProperty("answer_text")] public string Answer { get; set; }
    [JsonProperty("image")] public string Image { get; set; }
    [JsonProperty("question")] public string Question { get; set; }
    [JsonProperty("link")] public string Link { get; set; }
}

[Serializable]
public class Level
{
    [JsonProperty("level_image")] public int LevelImage { get; set; }
    [JsonProperty("answer_text")] public string Answer { get; set; }
    [JsonProperty("question")] public string Question { get; set; }
    //[JsonProperty("image_url")] public string ImageUrl { get; set; }
}