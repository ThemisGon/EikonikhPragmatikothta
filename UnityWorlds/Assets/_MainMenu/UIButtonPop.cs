using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonPop : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale")]
    [SerializeField] float hoverScale = 1.06f;
    [SerializeField] float pressedScale = 0.97f;
    [SerializeField] float speed = 16f;

    Vector3 baseScale;
    Vector3 targetScale;

    void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData e) => targetScale = baseScale * hoverScale;
    public void OnPointerExit(PointerEventData e) => targetScale = baseScale;
    public void OnPointerDown(PointerEventData e) => targetScale = baseScale * pressedScale;
    public void OnPointerUp(PointerEventData e) => targetScale = baseScale * hoverScale;
}

