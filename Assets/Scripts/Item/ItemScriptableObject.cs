using UnityEngine;

public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public int maxStackSize = 1;
}
