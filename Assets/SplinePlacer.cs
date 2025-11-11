using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class SplinePlacer : MonoBehaviour
{
    
    enum Button
    {
        SwitchTypeStart,
        SwitchTypeEnd,
        PlaceBuilding
    }

    public WorldGrid worldGrid;
    public Camera cameraGrid;
    public Building conveyor;
    public float height;
    

    float step;
    private Building currentBuilding;
    private float m_buttonCooldown = 0.3f;
    Camera cam;
    bool isVisibleGrid = false;
    int currentKnots = 0;
    SplineContainer splineContainer;
    bool isRaycastCross = false;

    private Dictionary<Button, bool> buttonsActiveState;


    void Start()
    {
        step = worldGrid.cell_step;
        cam = Camera.main;
        cameraGrid.gameObject.SetActive(false);
        isVisibleGrid = false;

        buttonsActiveState = new Dictionary<Button, bool>() {
            {Button.SwitchTypeStart, true},
            {Button.SwitchTypeEnd, true},
            {Button.PlaceBuilding, true},
        };
    }


    public void CreateBuilding(bool isNew)
    {
        if (currentBuilding != null && isNew) { 
            Destroy(currentBuilding.gameObject);
        }
        currentBuilding = Instantiate(conveyor.gameObject).GetComponent<Building>();

        if (isNew) 
        { 
            splineContainer = currentBuilding.GetComponent<SplineContainer>(); 
            splineContainer.Spline.Clear();
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
                isRaycastCross = true;
                Vector3 newPosition = new Vector3(hitInfo.point.x + (hitInfo.point.x % step < step / 2 ? -hitInfo.point.x % step : step - hitInfo.point.x % step),
                    hitInfo.point.y,
                    hitInfo.point.z + (hitInfo.point.z % step < step / 2 ? -hitInfo.point.z % step : step - hitInfo.point.z % step))
                    + worldGrid.null_position;


                var colliders = Physics.OverlapBox(newPosition, new Vector3(currentBuilding.x_size / 2 * worldGrid.cell_step, 1 * worldGrid.cell_step, currentBuilding.y_size / 2 * worldGrid.cell_step));
                bool isPlaceOccupied = false;
                Collider belt = null;

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
                    if (collider.CompareTag("Belt") && currentBuilding.gameObject != collider.GetComponentInParent<Building>().gameObject)
                    {
                        belt = collider;
                        Debug.LogWarning("Найден Belt");
                    }
                }
                if (isPlaceOccupied == false)
                {
                    if (currentKnots == 1)
                    {
                        
                        if (belt)
                        {
                            var anotherConveyour = belt.GetComponentInParent<ConveyorModule>();
                            Transform start = anotherConveyour.startPoint;
                            Transform end = anotherConveyour.endPoint;
                            Vector3 startWC = start.position;//anotherConveyour.transform.TransformPoint(start.position);
                            Vector3 endWC = end.position;//anotherConveyour.transform.TransformPoint(end.position);
                            if (start && end)
                            {
                                bool isNearToStart = Vector3.Distance(startWC, newPosition) < Vector3.Distance(endWC, newPosition);
                                Vector3 vector = (endWC - startWC).normalized;
                                
                                if (isNearToStart)
                                {
                                    Debug.DrawLine(startWC, newPosition, Color.red);
                                    currentBuilding.transform.position = startWC - vector * 1;
                                }
                                else
                                {
                                    Debug.DrawLine(endWC, newPosition, Color.green);
                                    currentBuilding.transform.position = endWC + vector * 1;
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Transform End and Start is null");
                                anotherConveyour.UpdateTransform();
                            }
                        }
                        else
                        {
                            currentBuilding.transform.position = newPosition;
                        }
                    }
                    else if (currentKnots == 2)
                    {
                        if (splineContainer != null) { 
                            var p = splineContainer.Spline.Knots.ToList();

                            if (belt)
                            {
                                var anotherConveyour = belt.GetComponentInParent<ConveyorModule>();
                                Transform start = anotherConveyour.startPoint;
                                Transform end = anotherConveyour.endPoint;
                                if (start && end)
                                {
                                    bool isNearToStart = Vector3.Distance(start.position, newPosition) < Vector3.Distance(end.position, newPosition);
                                    Vector3 vector = (end.position - start.position).normalized;
                                    
                                    if (isNearToStart)
                                    {
                                        Debug.DrawLine(start.position, newPosition, Color.red);
                                        newPosition = start.position;// - vector * 0.25f;
                                    }
                                    else
                                    {
                                        Debug.DrawLine(start.position, newPosition, Color.green);
                                        newPosition = end.position; //+ vector * 0.25f;
                                    }
                                    p[1] = new BezierKnot(-(currentBuilding.transform.position - newPosition));
                                    try
                                    {
                                        splineContainer.Spline.Knots = p;
                                    }
                                    catch
                                    {
                                        Debug.LogWarning("Приколы с несуществующим GameObject");
                                    }
                                }

                            }
                            else
                            {
                                p[1] = new BezierKnot(-(currentBuilding.transform.position - newPosition));
                                try
                                {
                                    splineContainer.Spline.Knots = p;
                                }
                                catch
                                {
                                    Debug.LogWarning("Приколы с несуществующим GameObject");
                                }
                            }
                        }
                    }
                }
                isPlaceOccupied = false;
            }
            else
            {
                isRaycastCross = false;
            }
        }
        else
        {
            if (isVisibleGrid) SetVisibleCamera(false);
        }
    }

    public async void SwitchTypeStartClick()
    {
        if (currentBuilding == null) return;
        Button currentButton = Button.SwitchTypeStart;
        if (buttonsActiveState[currentButton])
        {
            currentBuilding.GetComponent<ConveyorModule>().NextStartConveyorDesc();
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }

    public async void SwitchTypeEndClick()
    {
        if (currentBuilding == null) return;
        Button currentButton = Button.SwitchTypeEnd;
        if (buttonsActiveState[currentButton])
        {
            currentBuilding.GetComponent<ConveyorModule>().NextEndConveyorDesc();
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
                if (isRaycastCross) { 
                    splineContainer.Spline.Add(new BezierKnot(new Unity.Mathematics.float3(0, 0, 0)));
                    currentKnots++;
                }

            }
            else if (currentKnots == 2)
            {
                currentBuilding = null;
                splineContainer = null;
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
