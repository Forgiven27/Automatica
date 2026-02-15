using UnityEngine;
using Simulator;

public class SimulationRunner : MonoBehaviour
{
    public float tickDuration = 1f;
    float timer = 0;
    Simulation sim;

    private void Awake()
    {
        sim = new Simulation();
        SimulationAPI.Bind(sim);
    }


    private void Update()
    {
        if (timer > 0) { timer -= Time.deltaTime; return; }
        sim.Tick();
    }


}
