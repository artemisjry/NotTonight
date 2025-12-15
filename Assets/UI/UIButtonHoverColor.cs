using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class UIButtonHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    public TMP_Text buttonText; // TextMeshPro component

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    [Header("Audio")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = hoverColor;

        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}
