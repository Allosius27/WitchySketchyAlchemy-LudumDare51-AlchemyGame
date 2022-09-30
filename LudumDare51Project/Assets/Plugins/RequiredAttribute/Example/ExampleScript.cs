using UnityEngine;

public class ExampleScript : MonoBehaviour
{

    [Required]
    public int myInt; // Works with any type

    [Required]
    public Vector3 testDefault; // I said, ANY type!

    [Required]
    public Vector3 testInitialized = Vector3.one; // Once it was been initialized, the color disappears

    public MyClass myClass; // Works with custom types!

    
    [System.Serializable]
    public class MyClass
    {
        [Required("40FF40")] // Define any color with its hex code!
        public GameObject myPrefab;
    }
}