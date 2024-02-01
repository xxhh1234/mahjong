using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class SpriteData : MonoBehaviour
{
    public Sprite[] sprites;

    private readonly Dictionary<string, int> spriteIndex = new Dictionary<string, int>();
    private int index = 0;

    public Sprite[] SetSprites
    {
        set
        {
            sprites = value;
        }
    }

    public Sprite GetSprite(string spName)
    {
        if (spriteIndex.Count == 0)
        {
            for (int i = 0; i < sprites.Length; i++)
                spriteIndex.Add(sprites[i].name, i);
        }
        if (spriteIndex.TryGetValue(spName, out index))
        {
            return sprites[index];
        }
        return null;
    }
}