using UnityEngine;
using UnityEngine.UI;

public class CardIconsUI : MonoBehaviour
{
    public static CardIconsUI Instance;

    public Image[] cardIcons;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateIcons(int collected)
    {
        for (int i = 0; i < cardIcons.Length; i++)
        {
            if (i < collected)
                cardIcons[i].color = Color.white; // unlocked
            else
                cardIcons[i].color = new Color(1f, 1f, 1f, 0.3f); // locked
        }
    }
}
