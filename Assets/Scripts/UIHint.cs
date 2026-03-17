using TMPro;
using UnityEngine;

public class UIHint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionText;
    [SerializeField] private TextMeshProUGUI _hintText;
    [SerializeField] private RectTransform _actionContainer;
    public void Init(string action, string hint)
    {
        _actionContainer.gameObject.SetActive(true);
        _actionText.text = action;
        _hintText.text = hint;
    }
    public void Init(string hint)
    {
        _actionContainer.gameObject.SetActive(false);
        _hintText.text = hint;
    }
}
