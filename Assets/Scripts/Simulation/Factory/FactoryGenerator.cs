using System;
using System.Collections.Generic;


namespace Simulator
{
    [Serializable]
    public class FactoryGenerator
    {
        public int timeCooldown;
        public List<Pair<ItemType, int>> ingridients;
        public List<Pair<ItemType, int>> resultItems;

        public FactoryGenerator(FactoryGenerator factoryGenerator)
        {
            timeCooldown = factoryGenerator.timeCooldown;
            ingridients = new List<Pair<ItemType, int>>(factoryGenerator.ingridients);
            resultItems = new List<Pair<ItemType, int>>(factoryGenerator.resultItems);
        }

        public void Generate(List<FactorySlot> slots)
        {

            if (ingridients == null || ingridients.Count == 0)
            {
                foreach (var pair in resultItems)
                {
                    var key = pair.key;
                    var value = pair.value;
                    foreach (FactorySlot slot in slots)
                    {
                        if (slot.ioType == IOType.Input) continue;
                        if (slot.CanImportOneItem(key))
                        {
                            slot.ImportItems(key, value);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var pair in ingridients)
                {
                    var key = pair.key;
                    var value = pair.value;
                    foreach (FactorySlot slot in slots)
                    {
                        if (slot.ioType == IOType.Output) continue;
                        if (slot.CanExportItems(key, value))
                        {
                            slot.ExportItems(value);
                            break;
                        }
                    }
                }
                foreach (var pair in resultItems)
                {
                    var key = pair.key;
                    var value = pair.value;
                    foreach (FactorySlot slot in slots)
                    {
                        if (slot.ioType == IOType.Input) continue;
                        if (slot.CanImportOneItem(key))
                        {
                            slot.ImportItems(key, value);
                            break;
                        }
                    }
                }
            }


        }


        public bool TryGenerate(List<FactorySlot> slots)
        {
            bool cond_1 = true;
            bool cond_2 = true;
            foreach (var pair in ingridients)
            {
                var key = pair.key;
                var value = pair.value;
                bool f = false;
                foreach (FactorySlot slot in slots)
                {

                    if (slot.ioType == IOType.Output) continue;
                    if (slot.CanExportItems(key, value))
                    {
                        f = true;
                        break;
                    }
                }
                if (!f)
                {
                    cond_1 = false;
                    break;
                }
            }
            foreach (var pair in resultItems)
            {
                var key = pair.key;
                var value = pair.value;
                bool f = false;
                foreach (FactorySlot slot in slots)
                {
                    if (slot.ioType == IOType.Input) continue;
                    if (slot.CanImportOneItem(key))
                    {
                        f = true;
                        break;
                    }
                }
                if (!f)
                {
                    cond_2 = false;
                    break;
                }
            }



            return cond_1 && cond_2;
        }

    }
}