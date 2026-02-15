using UnityEngine;
using Simulator;

public class FactorySpawner : MonoBehaviour
{
    [SerializeField]private GameObject factory10;
    [SerializeField]private GameObject factory11;

    void OnEnable()
    {
        SimulationAPI.Events.Subscribe<FactoryCreatedEvent>(OnFactoryCreated);
    }

    void OnDisable()
    {
        SimulationAPI.Events.Unsubscribe<FactoryCreatedEvent>(OnFactoryCreated);
    }

    void OnFactoryCreated(FactoryCreatedEvent evt)
    {
        GameObject factoryPrefab;
        switch (evt.factoryType)
        {
            case FactoryType.ExportImport10:
                factoryPrefab = factory10;
                break;
            case FactoryType.ExportImport11:
                factoryPrefab = factory11;
                break;
            default:
                Debug.LogWarning("Данный тип завода не реализован! ;(");
                factoryPrefab = factory10;
                break;
        }

        var go = Instantiate(factoryPrefab, evt.position, evt.rotation);
        go.GetComponent<FactoryView>().Bind(evt.factoryId);
    }

   
}
