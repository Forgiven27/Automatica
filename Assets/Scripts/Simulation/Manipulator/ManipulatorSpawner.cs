using Simulator;
using UnityEngine;

public class ManipulatorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject manipulator;

    void OnEnable()
    {
        SimulationAPI.Events.Subscribe<ManipulatorCreatedEvent>(OnFactoryCreated);
    }

    void OnDisable()
    {
        SimulationAPI.Events.Unsubscribe<ManipulatorCreatedEvent>(OnFactoryCreated);
    }

    void OnFactoryCreated(ManipulatorCreatedEvent evt)
    {
        
        var go = Instantiate(manipulator, evt.transform.position, evt.transform.rotation);
        go.transform.localScale = evt.transform.scale;
        var manipulatorView = go.GetComponent<ManipulatorView>();

        manipulatorView.ID = evt.ID;
        manipulatorView.Init(evt.baseYaw, evt.bones);
        
    }
}
