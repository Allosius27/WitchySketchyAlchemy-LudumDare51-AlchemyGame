using UnityEngine;

[RequireComponent(typeof(RandomTexture))]
public class ClickMeToRandomizeTexture : MonoBehaviour
{

    private void OnMouseDown()
    {
        GetComponent<RandomTexture>().RandomizeTexture();
        Debug.Log("Texture randomized");
    }
}