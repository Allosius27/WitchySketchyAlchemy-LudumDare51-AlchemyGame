using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpText : MonoBehaviour
{
    public TextMeshProUGUI pointsText;

    public float lifeTime = 1f;
    public float moveSpeed = 1f;

    public float placementJitter = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, moveSpeed * Time.unscaledDeltaTime, 0f);
    }

    public void SetPoints(int amount)
    {
        if(amount > 0)
        {
            pointsText.text = "+ " + amount.ToString();
        }
        else
        {
            pointsText.text = "- " + amount.ToString();
        }

        
        transform.position += new Vector3(Random.Range(-placementJitter, placementJitter), Random.Range(-placementJitter, placementJitter), 0f);
    }
}
