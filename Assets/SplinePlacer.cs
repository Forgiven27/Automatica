using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class SplinePlacer : MonoBehaviour
{
    
    enum Button
    {
        SwitchType,
        PlaceBuilding
    }

    public WorldGrid worldGrid;
    public Camera cameraGrid;
    public float height;
    public List<Building> buildings;

    float step;
    private Building currentBuilding;
    private int currentIndex;
    private float m_buttonCooldown = 0.1f;
    Camera cam;
    bool isVisibleGrid = false;
    int currentKnots = 0;
    SplineContainer splineContainer;

    private Dictionary<Button, bool> buttonsActiveState;


    void Start()
    {
        step = worldGrid.cell_step;
        cam = Camera.main;
        cameraGrid.gameObject.SetActive(false);
        isVisibleGrid = false;

        buttonsActiveState = new Dictionary<Button, bool>() {
            {Button.SwitchType, true},
            {Button.PlaceBuilding, true},
        };
    }

    private void OnDrawGizmosSelected()
    {
        
    }

    public void CreateBuildingByIndex(int index)
    {
        if (index < 0) return;
        if (index >= buildings.Count) index = 0;
        if (buildings[index] == null) return;
        bool isNew = false;
        if (currentBuilding != null) { 
            Destroy(currentBuilding.gameObject);
            isNew = true;
        }
        currentIndex = index;
        currentBuilding = Instantiate(buildings[index]);
        if (isNew) 
        { 
            splineContainer = currentBuilding.GetComponent<SplineContainer>(); 
            splineContainer.Spline.Add(new BezierKnot(new Unity.Mathematics.float3(0,0,0)));
            currentKnots++;
        }
    }




    void SetVisibleCamera(bool flag)
    {
        if (isVisibleGrid && !flag)
        {
            cameraGrid.gameObject.SetActive(false);
            isVisibleGrid = false;
        }
        if (!isVisibleGrid && flag)
        {
            cameraGrid.gameObject.SetActive(true);
            isVisibleGrid = true;
        }
    }


    void Update()
    {

        if (currentBuilding != null)
        {

            SetVisibleCamera(true);
            Ray rayMouse = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayMouse, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("BuildingSurface")))
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
                    if(currentKnots == 1)
                    {
                        currentBuilding.transform.position = newPosition;
                    }
                    else if(currentKnots == 2)
                    {
                        var firstKnot = splineContainer.Spline.Knots.ElementAt(1);
                        firstKnot.Position = newPosition;
                    }
                }
                isPlaceOccupied = false;

            }
        }
        else
        {
            if (isVisibleGrid) SetVisibleCamera(false);
        }

    }

    public async void SwirchTypeClick()
    {
        if (currentBuilding == null) return;
        Button currentButton = Button.SwitchType;
        if (buttonsActiveState[currentButton])
        {
            CreateBuildingByIndex(currentIndex++);
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
            if (currentKnots == 1)
            {
                splineContainer.Spline.Add(new BezierKnot(new Unity.Mathematics.float3(0, 0, 0)));
                currentKnots++;
            }else if (currentKnots == 2)
            {
                currentBuilding = null;
                currentKnots = 0;
            }
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
