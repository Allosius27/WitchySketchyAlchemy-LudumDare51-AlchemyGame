using UnityEngine;

[CreateAssetMenu(fileName = "RandomTextureData", menuName = "RandomTexture")]
public class RandomTextureData : ScriptableObject
{

    [SerializeField]
    private Texture2D[] textures;


    public Texture2D[] Textures { get { return textures; } }
}