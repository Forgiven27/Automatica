using Simulator;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class ConveyorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject conveyorPrefab;

    void OnEnable()
    {
        SimulationAPI.Events.Subscribe<ConveyorCreatedEvent>(OnConveyorCreated);
    }

    void OnDisable()
    {
        SimulationAPI.Events.Unsubscribe<ConveyorCreatedEvent>(OnConveyorCreated);
    }

    void OnConveyorCreated(ConveyorCreatedEvent evt)
    {
        
        // TODO: Создание линии конвейеров


        var conveyor = Instantiate(conveyorPrefab, evt.startPosition, Quaternion.identity);

        var splineContainer = conveyor.GetComponent<SplineContainer>();
        splineContainer.Spline.AddRange(new float3[] { float3.zero, new float3(evt.endPosition.x, evt.endPosition.y, evt.endPosition.z) });
        conveyor.GetComponent<ConveyorView>().Bind(evt.conveyorID);
    }
}
