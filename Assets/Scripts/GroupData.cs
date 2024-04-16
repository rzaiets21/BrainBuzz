using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GroupData
{
    public int GroupId;
    public string GroupName;
    public List<LevelData> Levels = new();

    public bool IsCompleted()
    {
        return Levels.All(levelData => levelData.IsCompleted());
    }
}