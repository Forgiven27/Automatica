using Simulator;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using static UnityEngine.Rendering.HableCurve;

public class ConveyorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject conveyorPrefab;
    [SerializeField] private GameObject segmentPrefab;

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


        var conveyorParent = Instantiate(conveyorPrefab, evt.segmentsTransform[0].position, Quaternion.identity);
        SegmentView segmentView;
        for (int i = 0; i < evt.segmentsTransform.Length; i++)
        {
            TransformSim transformSim = evt.segmentsTransform[i];
            
            GameObject segment = Instantiate(segmentPrefab, transformSim.position, transformSim.rotation, conveyorParent.transform);
            segment.transform.localScale = transformSim.scale;
            
            if (segment.TryGetComponent<SegmentView>(out segmentView))
            {
                segmentView.Bind(evt.segmentsID[i]);
            }
        }

        if (conveyorParent.TryGetComponent<ConveyorView>(out ConveyorView conveyorView))
        {
            conveyorView.Bind(evt.conveyorID);
            conveyorView.SetSegments(evt.segmentsTransform);
        }

        

    }
}
