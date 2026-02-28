using UnityEngine;
using Simulator;

public class SimulationRunner : MonoBehaviour
{
    [SerializeField] private SimulationConfiguration config;
    float timer = 0;
    Simulation sim;

    private void Awake()
    {
        sim = new Simulation(config);
        SimulationAPI.Bind(sim);
    }


    private void Update()
    {
        if (timer > 0) { timer -= Time.deltaTime; return; }
        sim.Tick();
        timer = config.tickDuration;
    }


}
