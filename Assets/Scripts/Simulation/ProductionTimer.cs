using UnityEngine;

public class ProductionTimer
{
    int currentTicks;
    
    public void Start(int ticks)
    {
        currentTicks = ticks;
    }

    public bool IsReady()
    {
        if (currentTicks <= 0) return true;
        return false;
    }
    public void TryTick()
    {
        if (currentTicks > 0)
            currentTicks--;
    }
}
