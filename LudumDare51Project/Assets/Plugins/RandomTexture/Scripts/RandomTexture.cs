using UnityEngine;

public class RandomTexture : MonoBehaviour
{
    [SerializeField]
    [Tooltip("This material must be one of the materials of this object. Its main texture will be randomized using the data file below.")]
    private Material material;

    [SerializeField]
    private RandomTextureData randomTextureData;

    [SerializeField]
    private bool randomizeOnStart = true;


    private Renderer myRenderer;
    private Material[] materials;


	void Start()
    {
        if (randomizeOnStart)
        {
            RandomizeTexture();
        }
	}

    /// <summary>
    /// You can change the RandomTextureData at runtime.
    /// </summary>
    public void SetRandomTextureData(RandomTextureData data)
    {
        randomTextureData = data;
    }

    /// <summary>
    /// Randomizes texture.
    /// </summary>
    public void RandomizeTexture()
    {
        if (InitAndCheckEverything())
        {
            for (int i = 0; i < myRenderer.materials.Length; i++) // Since we modify the array, we're forced to use a 'for' instead of 'foreach'
            {
                if (materials[i] == material) // The object already has this material, we want to randomize its main texture
                {
                    myRenderer.materials[i].mainTexture = GetRandomTexture(randomTextureData.Textures);
                }
            }
        }
    }

    private Texture2D GetRandomTexture(Texture2D[] textures)
    {
        return textures[Random.Range(0, textures.Length)];
    }

    /// <summary>
    /// Returns false if there was an error.
    /// </summary>
    private bool InitAndCheckEverything()
    {
        bool error = false;

        if (material == null)
        {
            Debug.LogError("No material specified on GameObject '" + name + "'. RandomTexture cannot work.");
            error = true;
        }

        if (myRenderer == null)
        {
            myRenderer = GetComponent<Renderer>();

            if (myRenderer == null)
            {
                Debug.LogError("No renderer on GameObject '" + name + "'. RandomTexture cannot work.");
                error = true;
            }
        }

        if (myRenderer != null && materials == null)
        {
            materials = myRenderer.sharedMaterials; // Accessing sharedMaterials, instead of materials, doesn't create instances of the materials.
        }

        if (randomTextureData == null || randomTextureData.Textures == null || randomTextureData.Textures.Length == 0)
        {
            Debug.LogError("No textures specified on GameObject '" + name + "'. RandomTexture cannot work.");
            error = true;
        }

        return !error;
    }
}
