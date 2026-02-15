using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hint;
    [SerializeField] private Image _previewImage;

    public void InitButton(Sprite sprite, string hintButtonText)
    {
        _hint.text = hintButtonText;
        _previewImage.sprite = sprite;
    }

    public void UpdateSprite(Sprite sprite)
    {
        _previewImage.sprite = sprite;
    }

    public void UpdateHint(string hintButtonText)
    {
        _hint.text = hintButtonText;
    }
}
