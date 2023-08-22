using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;

//TODO 自定义虚拟摇杆可以在这里设置

public class JoyStick_Move : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Transform stickBK;
    public Transform stickBtn;

    public void OnPointerDown(PointerEventData eventData)
    {
        stickBK.gameObject.SetActive(true);

        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);

        stickBK.transform.localPosition = m_PointerDownPos;
        //stickBtn.transform.localPosition = m_PointerDownPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
        var delta = position - m_PointerDownPos;

        delta = Vector2.ClampMagnitude(delta, movementRange);
        ((RectTransform)stickBtn).anchoredPosition = m_StartPos + (Vector3)delta;

        var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
        SendValueToControl(newPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ((RectTransform)stickBtn).anchoredPosition = m_StartPos;
        SendValueToControl(Vector2.zero);

        stickBK.gameObject.SetActive(false);
    }

    private void Start()
    {
        m_StartPos = ((RectTransform)stickBtn).anchoredPosition;
    }

    public float movementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }

    [FormerlySerializedAs("movementRange")]
    [SerializeField]
    private float m_MovementRange = 50;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    private Vector3 m_StartPos;
    private Vector2 m_PointerDownPos;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}