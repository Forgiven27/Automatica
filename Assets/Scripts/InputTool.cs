using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;


public static class InputTool
{
    public static bool TryGetSurfacePoint(out Vector3 point)
    {

        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayMouse, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("BuildingSurface")))
        {
            point = hitInfo.point;
            return true;
        }
        else
        {
            point = Vector3.zero;
            return false;
        }
    }

    public static bool TryGetSurfaceGridPoint(out Vector3 point, float cellSize)
    {
        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayMouse, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("BuildingSurface")))
        {
            point = hitInfo.point;
            float x = point.x - point.x % cellSize + cellSize / 2;
            float z = point.z - point.z % cellSize + cellSize / 2;
            point = new Vector3(x, point.y, z);
            return true;
        }
        else
        {
            point = Vector3.zero;
            return false;
        }
    }



    public static bool TryGetRaycastHit(string nameOfLayer, out RaycastHit hitInfo)
    {
        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(rayMouse, out hitInfo, 100, 1 << LayerMask.NameToLayer(nameOfLayer));
    }

    public static bool TryGetGridPoint(string nameOfLayer, out RaycastHit hitInfo)
    {
        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayMouse, out hitInfo, 100, 1 << LayerMask.NameToLayer(nameOfLayer)))
        {
            
            return true;
        }
        return false;
    }

    public static bool TryGetRaycastHit(int layer, out RaycastHit hitInfo)
    {
        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(rayMouse, out hitInfo, 100, layer);
    }
}
