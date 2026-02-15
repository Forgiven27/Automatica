using UnityEngine;


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

    public static bool TryGetPoint(string nameOfLayer, out RaycastHit hitInfo)
    {
        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(rayMouse, out hitInfo, 100, 1 << LayerMask.NameToLayer(nameOfLayer));
    }
    public static bool TryGetPoint(int layer, out RaycastHit hitInfo)
    {
        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(rayMouse, out hitInfo, 100, layer);
    }
}
