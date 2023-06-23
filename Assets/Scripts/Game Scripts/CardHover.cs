using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;


public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalPosition;
    private Color originalColor;

    [SerializeField]
    private Color hoverColor = HexToColor("FF4657");

    [SerializeField]
    private float hoverElevation = 20f;

    public Image cardImage;

    private Transform parentToGo;
    private CardInfo cardInfo;

    void Awake()
    {
        cardInfo = GetComponent<CardInfo>();
        originalPosition = transform.position;
        if (cardImage != null)
        {
            originalColor = cardImage.color;
        }
        parentToGo = gameObject.transform.parent;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!TurnManager.Instance.myTurn)
            return;
        // On mouse hover, change color and elevate the card.
        if (cardImage != null &&cardInfo.canAttack)
        {
            cardImage.color = hoverColor;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + hoverElevation, transform.position.z);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!TurnManager.Instance.myTurn)
            return;
        // On mouse exit, return color and position to the original state.
        if (cardImage != null&&cardInfo.canAttack)
        {
            cardImage.color = originalColor;
        }
        //transform.position = originalPosition;
        transform.position = new Vector3(transform.position.x, transform.position.y - hoverElevation, transform.position.z);
        //iTween.MoveTo(gameObject, iTween.Hash("position", parentToGo, "time", hoverTransitionTime, "easetype", "easeInOutQuad"));
    }
    public static Color HexToColor(string hex)
{
    byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
    byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
    byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
    return new Color32(r, g, b, 255);
}
}