using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    public WorldGrid WorldGrid;
    public int x_size;
    public int y_size;
    public float height = 0.5f;
    public BuildingType buildingType;

    private void OnDrawGizmosSelected()
    {
        float step = WorldGrid.cell_step;
        
        for (int i = 0; i < x_size; i++)
        {
            for (int j = 0; j < y_size; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(transform.position - new Vector3((step * x_size) / 2,0, (step * y_size) / 2) + new Vector3(i * step + step / 2, 0, j * step + step / 2), new Vector3(step, height, step));
            }
        }
    }

}
public enum BuildingType
{
    Common,
    Spline,
    Factory,
    Conveyor,
}

