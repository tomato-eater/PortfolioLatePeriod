using System;

/// <summary>
/// JsonUtilityで配列が使用できない
/// </summary>
[Serializable]
public class ItemList
{
    public Item[] items;
}

/// <summary>
/// サーバーとやり取りする１件分のデータ
/// JsonUtilityで使用、Jsonと名前を揃える
/// </summary>
[Serializable]
public class Item
{
    public int id;
    public string name;
}