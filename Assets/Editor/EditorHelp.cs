using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class EditorHelper : MonoBehaviour
{
    private static int totalSprites = 8; // change depending on the total sprites in this batch!

    [MenuItem("EditorHelper/SliceSprites")]
    static void SliceSprites()
    {
        Texture2D[] textures = new Texture2D[totalSprites];

        for (int z = 0; z < totalSprites; z++)
        {
            // path is Assets/Resources/Sprites/Sprite (1).png
            textures[z] = Resources.Load<Texture2D>("Sprites/Sprite (" + (z + 1) + ")");
            string path = AssetDatabase.GetAssetPath(textures[z]);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            int SliceWidth = 12;
            int SliceHeight = 12;

            for (int i = 0; i < textures[z].width; i += SliceWidth)
            {
                for (int j = textures[z].height; j > 0; j -= SliceHeight)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f , 0.5f);
                    smd.alignment = 9;
                    smd.name = (textures[z].height - j) / SliceHeight + ", " + i / SliceWidth;
                    smd.rect = new Rect(i , j - SliceHeight , SliceWidth , SliceHeight);

                    newData.Add(smd);
                }
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path , ImportAssetOptions.ForceUpdate);
        }
        Debug.Log("Done Slicing!");
    }
}