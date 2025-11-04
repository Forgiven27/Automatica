using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CommonPlacer : MonoBehaviour
{

    enum Button
    {
        RightRotate,
        LeftRotate,
        PlaceBuilding
    }

    public WorldGrid worldGrid;
    public Camera cameraGrid;
    public float heightGizmo;
    
    
    float step;
    private Building currentBuilding;
    private float m_buttonCooldown = 0.1f;
    Camera cam;
    bool isVisibleGrid = false;

    private Dictionary<Button, bool> buttonsActiveState;


    void Start()
    {
        step = worldGrid.cell_step;
        cam = Camera.main;
        cameraGrid.gameObject.SetActive(false);
        isVisibleGrid = false;

        buttonsActiveState = new Dictionary<Button, bool>() {
            {Button.RightRotate, true},
            {Button.LeftRotate, true},
            {Button.PlaceBuilding, true},
        };
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, heightGizmo);
        int x_size = (int)(transform.localScale.x / step);
        int y_size = (int)(transform.localScale.y / step);
        for (int i = 0;i < x_size; i++)
        {
            for (int j = 0;j < y_size; j++)
            {
                if((i + j) % 2 == 0)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color= Color.green;
                }
                Gizmos.DrawCube(transform.position - new Vector3(transform.localScale.x/2, 0, transform.localScale.y/2) + new Vector3(i * step + step/2,0, j * step + step / 2), new Vector3(step, heightGizmo, step));
            }
        }
    }
    
    public void CreateBuilding(Building building)
    {
        if (currentBuilding != null) Destroy(currentBuilding.gameObject);

        currentBuilding = Instantiate(building);
        var list = currentBuilding.GetComponentsInChildren<Transform>();//.Select(p => p.gameObject.layer = LayerMask.NameToLayer("GhostBuilding"));
        foreach (Transform t in list)
        {
            t.gameObject.layer = LayerMask.NameToLayer("GhostBuilding");
        }
    }

    

    
    void SetVisibleCamera(bool flag)
    {
        if (isVisibleGrid && !flag)
        {
            cameraGrid.gameObject.SetActive(false);
            isVisibleGrid = false;
        }if (!isVisibleGrid && flag)
        {
            cameraGrid.gameObject.SetActive(true);
            isVisibleGrid = true;
        }
    }


    void Update()
    {
        
        if (currentBuilding != null) {

            SetVisibleCamera(true);
            Ray rayMouse = cam.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(rayMouse, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("BuildingSurface")))
            {

                Vector3 newPosition = new Vector3(hitInfo.point.x + (hitInfo.point.x % step < step / 2 ? -hitInfo.point.x % step : step - hitInfo.point.x % step),
                    hitInfo.point.y,
                    hitInfo.point.z + (hitInfo.point.z % step < step / 2 ? -hitInfo.point.z % step : step - hitInfo.point.z % step))
                    + worldGrid.null_position;


                var colliders = Physics.OverlapBox(newPosition, new Vector3(currentBuilding.x_size / 2 * worldGrid.cell_step, 1 * worldGrid.cell_step, currentBuilding.y_size / 2 * worldGrid.cell_step));
                bool isPlaceOccupied = false;
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Building"))
                    {
                        if (isPlaceOccupied == false)
                        {
                            isPlaceOccupied = true;
                        }
                        else
                        {
                            print("OOOh NO, there's a building!");
                        }
                    }
                }
                if (isPlaceOccupied == false)
                {
                    currentBuilding.transform.position = newPosition;
                }
                isPlaceOccupied = false;
                
            }
        }
        else
        {
            if(isVisibleGrid) SetVisibleCamera(false);
        }
        
    }

    public async void RotateRightClick()
    {
        if (currentBuilding == null) return;
        Button currentButton = Button.RightRotate;
        if (buttonsActiveState[currentButton])
        {
            currentBuilding.gameObject.transform.Rotate(Vector3.up, -90);
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }
    public async void RotateLeftClick()
    {
        if (currentBuilding == null) return;
        Button currentButton = Button.LeftRotate;
        if (buttonsActiveState[currentButton])
        {
            currentBuilding.gameObject.transform.Rotate(Vector3.up, 90);
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }

    public async void PlaceBuildingClick()
    {
        if (currentBuilding == null) return;
        Button currentButton = Button.PlaceBuilding;
        if (buttonsActiveState[currentButton])
        {
            var list = currentBuilding.GetComponentsInChildren<Transform>();
            foreach (Transform t in list)
            {
                t.gameObject.layer = LayerMask.NameToLayer("Building");
            }
            currentBuilding = null;
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }

    private async UniTask TimerStandard(Button button)
    {
        await UniTask.WaitForSeconds(m_buttonCooldown);
        buttonsActiveState[button] = true;
    }


}
