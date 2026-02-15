using UnityEngine;

public class ConveyorElement : MonoBehaviour
{
    public ConveyorElement NextConveyor { get; set; }

    private bool _isFirstElement;
    private bool _isLastElement;
    public bool IsFirstElement { get { return _isFirstElement; } set 
        { 
            if (value)
            {
                _isLastElement = false;
                _isFirstElement = true;
            }
            else
            {
                _isFirstElement = false;
            }
        } 
    }
    public bool IsLastElement { get { return _isLastElement; } set
        {
            if (value)
            {
                _isLastElement = true;
                _isFirstElement = false;
            }
            else
            {
                _isLastElement = false;
            }
        }
    }

}
