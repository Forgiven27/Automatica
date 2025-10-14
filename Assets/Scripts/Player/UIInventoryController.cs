using UnityEngine;

public class UIInventoryController : MonoBehaviour
{

    private bool m_IsFirstSClick;
    private bool m_IsSecondSClick;
    private bool m_IsThirdSClick;
    private bool m_IsForthSClick;
    private bool m_IsFifthSClick;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsFirstSClick)
        {
            print("b1");
        }
        if (m_IsSecondSClick)
        {
            print("b2");
        }
        if (m_IsThirdSClick)
        {
            print("b3");
        }
        if (m_IsForthSClick)
        {
            print("b4");
        }
        if (m_IsFifthSClick)
        {
            print("b5");
        }
    }

    public void SetInventoryButtonState(bool b1, bool b2, bool b3, bool b4, bool b5)
    {
        m_IsFirstSClick = b1;
        m_IsSecondSClick = b2;
        m_IsThirdSClick = b3;
        m_IsForthSClick = b4;
        m_IsFifthSClick = b5;
    }

}
