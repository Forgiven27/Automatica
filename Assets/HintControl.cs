using TMPro;
using UnityEngine;

public class HintControl : MonoBehaviour
{
    [SerializeField] string textHint;
    private RectTransform rectTransform;
    GameObject cameraCanvas;
    BoxCollider boxCollider;
    
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rectTransform = GetComponentInChildren<RectTransform>();
        cameraCanvas = rectTransform.GetComponent<Canvas>().worldCamera.gameObject;
        TMP_Text textMeshPro = rectTransform.GetComponentInChildren<TMP_Text>();
        textMeshPro.text = textHint;
        textMeshPro.textWrappingMode = TextWrappingModes.NoWrap;
        rectTransform.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        rectTransform.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        rectTransform.gameObject.SetActive(false);
    }


    void Update()
    {
        rectTransform.LookAt(new Vector3(-cameraCanvas.transform.position.x, -cameraCanvas.transform.position.y, -cameraCanvas.transform.position.z));
    }
}
