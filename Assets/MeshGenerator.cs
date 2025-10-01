using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    public WorldGrid WorldGrid;
    Mesh mesh;
    public int x_size;
    public int y_size;
    public float transparentValue;

    private int x_current;
    private int y_current;
    private Vector3 pos_current;

    void Start()
    {
        mesh = new Mesh();
        mesh.name = "Grid";
        GetComponent<MeshFilter>().mesh = mesh;

        UpdateGrid();

        GetComponent<MeshRenderer>().material = new Material(Shader.Find("GUI/Text Shader"));
    }

    // Update is called once per frame
    void Update()
    {
        if (x_size != x_current || y_size != y_current || pos_current != transform.position)
        {
            UpdateGrid();
        }
    }

    void UpdateGrid()
    {
        mesh.Clear();
        var verticesList = new List<Vector3>();
        List<Color> colors = new List<Color>();
        var indicesList = new List<int>();
        int numberOfCell = 0;
        Vector3 parentPos;
        if (transform.parent != null)
        {
            parentPos = transform.parent.position;
        }
        else
        {
            parentPos = transform.position;
        }
        parentPos = (parentPos - WorldGrid.null_position);
        float x_correction = (parentPos.x % WorldGrid.cell_step);
        float y_correction = (parentPos.z % WorldGrid.cell_step);

        x_correction = x_correction < WorldGrid.cell_step / 2 ? -x_correction : WorldGrid.cell_step - x_correction;
        y_correction = y_correction < WorldGrid.cell_step / 2 ? -y_correction : WorldGrid.cell_step - y_correction;
        for (int i = 0; i < x_size; i++)
        {
            for (int j = 0; j < y_size; j++)
            {
                verticesList.Add(new Vector3(0 * WorldGrid.cell_step + i * WorldGrid.cell_step - (x_size/2f) + x_correction, transform.localPosition.y,
                    0 * WorldGrid.cell_step + j * WorldGrid.cell_step - (y_size / 2f) + y_correction));
                verticesList.Add(new Vector3(0 * WorldGrid.cell_step + i * WorldGrid.cell_step - (x_size / 2f) + x_correction, transform.localPosition.y,
                    1 * WorldGrid.cell_step + j * WorldGrid.cell_step - (y_size / 2f) + y_correction));
                verticesList.Add(new Vector3(1 * WorldGrid.cell_step + i * WorldGrid.cell_step - (x_size / 2f) + x_correction, transform.localPosition.y,
                    1 * WorldGrid.cell_step + j * WorldGrid.cell_step - (y_size / 2f) + y_correction));
                verticesList.Add(new Vector3(1 * WorldGrid.cell_step + i * WorldGrid.cell_step - (x_size / 2f) + x_correction , transform.localPosition.y,
                    0 * WorldGrid.cell_step + j * WorldGrid.cell_step - (y_size / 2f) + y_correction));

                if ((i + j) % 2 == 0)
                {
                    colors.Add(new Color(1, 0, 0, transparentValue));
                    colors.Add(new Color(1, 0, 0, transparentValue));
                    colors.Add(new Color(1, 0, 0, transparentValue));
                    colors.Add(new Color(1, 0, 0, transparentValue));
                }
                else
                {
                    colors.Add(new Color(0, 0, 1, transparentValue));
                    colors.Add(new Color(0, 0, 1, transparentValue));
                    colors.Add(new Color(0, 0, 1, transparentValue));
                    colors.Add(new Color(0, 0, 1, transparentValue));
                }
                indicesList.Add(0 + numberOfCell * 4);
                indicesList.Add(1 + numberOfCell * 4);
                indicesList.Add(2 + numberOfCell * 4);

                indicesList.Add(0 + numberOfCell * 4);
                indicesList.Add(2 + numberOfCell * 4);
                indicesList.Add(3 + numberOfCell * 4);
                numberOfCell++;
            }
        }
        
        mesh.SetVertices(verticesList);
        mesh.SetIndices(indicesList, MeshTopology.Triangles, 0);
        mesh.SetColors(colors);

        x_current = x_size;
        y_current = y_size;
        pos_current = transform.position;

        mesh.RecalculateNormals();
    }

}
