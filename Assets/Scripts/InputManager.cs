using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    /*
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadRebinds();
    }*/
}
