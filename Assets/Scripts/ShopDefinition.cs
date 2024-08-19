using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreDB", menuName = "Create StoreDB")]
public class ShopDefinition : ScriptableObject
{
    [SerializeField] public ProductInfo[] products;
}

[Serializable]
public class ProductInfo
{
    public ShopProductNames name;
    public Sprite icon;
    public int coins;
    public PowerupProductInfo[] powerups;
    public bool isPopular;
    public bool bestPrice;
    public bool adsFree;
    public string price;
}

[Serializable]
public class PowerupProductInfo
{
    public PowerupType PowerupType;
    public int count;
}