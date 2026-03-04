using UnityEngine;
using System.Collections; 

public class GameIntro : MonoBehaviour
{
    [Header("Χρόνος Εμφάνισης (σε δευτερόλεπτα)")]
    public float timeOnScreen = 5f;

    void Start()
    {
        StartCoroutine(HideIntroAfterTime());
    }

    IEnumerator HideIntroAfterTime()
    {
        yield return new WaitForSeconds(timeOnScreen);

        gameObject.SetActive(false);
    }
}