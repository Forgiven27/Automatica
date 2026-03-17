using UnityEngine.InputSystem;

public class Hint
{
    public HintType Type;

    private string _additionalText;
    private string _mainText;
    public string MainText 
    { 
        get 
        {
            return _mainText;
        } 
        private set
        {
            _mainText = value;
        }
    }
    public string AdditionalText 
    { 
        get 
        {
            if (_additionalText == null)
            {
                _additionalText = string.Empty;
            }
            return _additionalText;
        } 
        private set
        {
            _additionalText = value;
        }
    }
    public Hint(string text)
    {
        _mainText = text;
        Type = HintType.Text;
    }

    public Hint(InputAction action, string hintText)
    {
        Type = HintType.ActionHint;
        _mainText = hintText;
        string bindingText = action.bindings[0].effectivePath;
        _additionalText = InputControlPath.ToHumanReadableString(bindingText, InputControlPath.HumanReadableStringOptions.OmitDevice);

    }

    public Hint(string firstPart, string secondPart)
    {
        Type = HintType.Double;
        _additionalText = firstPart;
        _mainText = secondPart;
    }


}

public enum HintType
{
    Text,
    ActionHint,
    Double,
}
