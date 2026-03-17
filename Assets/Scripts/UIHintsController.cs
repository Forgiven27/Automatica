using System.Collections.Generic;
using UnityEngine;

public class UIHintsController : MonoBehaviour
{
    [SerializeField] private UIHint _hintPrefab;
    [SerializeField] private RectTransform _hintContainer;

    List<UIHint> _hintList = new();

    public void SetHints(Hint[] hints)
    {
        ClearHintsList();
        foreach (Hint hintInfo in hints)
        {
            UIHint hint = Instantiate(_hintPrefab, _hintContainer);
            if (hintInfo.Type != HintType.Text)
            {
                hint.Init(hintInfo.AdditionalText, hintInfo.MainText);
            }
            else
            {
                hint.Init(hintInfo.MainText);
            }
            hint.gameObject.SetActive(true);
            _hintList.Add(hint);
        }
    }

    public void AddHint(Hint hintInfo)
    {
        UIHint hint = Instantiate(_hintPrefab, _hintContainer);
        if (hintInfo.Type != HintType.Text)
        {
            hint.Init(hintInfo.AdditionalText, hintInfo.MainText);
        }
        else
        {
            hint.Init(hintInfo.MainText);
        }
        hint.gameObject.SetActive(true);
        _hintList.Add(hint);
    }

    private void ClearHintsList()
    {
        for (int i = 0;  i < _hintList.Count; i++)
        {
            Destroy(_hintList[i].gameObject);
        }
        _hintList.Clear();
    }
}
