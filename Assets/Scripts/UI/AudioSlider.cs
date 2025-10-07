using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite _handleSprite, _handleIdleSprite;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _handleImage;
    [SerializeField] private TMP_Text _handleText;

    private bool _isPointerOver = false;

    void Update()
    {
        if (_isPointerOver)
            _handleText.text = Mathf.RoundToInt(_slider.value * 100).ToString() + "%";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _handleImage.sprite = _handleSprite;
        _handleText.gameObject.SetActive(true);
        _isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _handleImage.sprite = _handleIdleSprite;
        _handleText.gameObject.SetActive(false);
        _isPointerOver = false;
    }
}
