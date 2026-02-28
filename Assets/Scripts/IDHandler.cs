public static class IDHandler
{
    static uint nextID = 1;// ID = 0 for not initialized objects

    public static uint GetID()
    {
        return nextID++;
    }
}
