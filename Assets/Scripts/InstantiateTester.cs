using Unity.VisualScripting;
using UnityEngine;

public class InstantiateTester : MonoBehaviour
{
    public GameObject go;
    public float cooldown = 5f;
    public Vector3 sizeGizmo = new Vector3(0.5f,0.5f,0.5f);
    float m_Timer = 0;
    
    void Update()
    {
        if (m_Timer > 0) m_Timer -= Time.deltaTime;
        else
        {
            Transform.Instantiate(go, transform,false);
            m_Timer = cooldown;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, sizeGizmo);
    }
}
