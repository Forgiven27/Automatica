using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private RectTransform _uiPanel;

    [SerializeField]private TextMeshProUGUI _textMeshPro;

    private string _currentText = "";
    public void UpdateTextUI(string text)
    {
        if (_textMeshPro == null) return;
        if (_currentText == text) return;
        _textMeshPro.text = text;
        _currentText = text;
    }

    public void EnablePanel()
    {
        if (_uiPanel == null) return;
        _uiPanel.gameObject.SetActive(true);
    }
    public void DisablePanel()
    {
        if (_uiPanel == null) return;
        _uiPanel.gameObject.SetActive(false);
    }
}
