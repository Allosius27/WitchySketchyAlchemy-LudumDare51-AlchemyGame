using UnityEngine;

public class MemberCtrl : MonoBehaviour
{
    #region Fields

    private bool naturalState = true;

    #endregion

    #region Properties

    public bool NaturalState => naturalState;

    #endregion

    #region UnityInspector

    [Required]
    [SerializeField] private GameObject defaulVisual;

    [Required]
    [SerializeField] private GameObject shapeShiftVisual;

    public MemberType memberType;

    #endregion

    #region Behaviour

    private void Update()
    {
        defaulVisual.SetActive(naturalState);

        shapeShiftVisual.SetActive(!naturalState);
    }

    public void SetShapeShifting(Sprite spriteMemberTransformed, bool value)
    {
        naturalState = !value;

        defaulVisual.SetActive(naturalState);

        shapeShiftVisual.SetActive(!naturalState);
        if(shapeShiftVisual.activeSelf)
        {
            shapeShiftVisual.GetComponent<SpriteRenderer>().sprite = spriteMemberTransformed;
        }
    }

    #endregion
}
