using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Simulator
{
    public class Factory : IEntity, IItemSource, IItemSink, IConnectable
    {
        public string ID { get; set; }


        public FactoryGenerator generator;
        public ProductionTimer timer = new();
        public List<FactoryPort> ports;
        public List<FactorySlot> slots;
        public bool isWorking;
        public bool isDidAction = false;

        public Factory(string id, FactoryGenerator generator, List<FactorySlot> slots, List<FactoryPort> ports)
        {
            this.ID = id;
            this.generator = generator;
            this.slots = slots;
            this.ports = ports;
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].ID = i.ToString();
            }
            for (int i = 0; i < ports.Count; i++)
            {
                ports[i].ID = i.ToString();
            }
        }

        public void Work(FactoryContext context)
        {
            ImportExportAction(context);
            if (timer.IsReady())
            {
                if (generator.TryGenerate(slots))
                {
                    timer.Start(generator.timeCooldown);
                    generator.Generate(slots);
                }
            }
            else
            {
                timer.TryTick();
            }

        }

        void ImportExportAction(FactoryContext context)
        {
            var itemRequiredList = new List<ItemType>();
            foreach (var port in ports)
            {
                if (!port.isConnected) continue;
                PortRef portRef = new PortRef() { entityId = ID, portId = port.ID };
                if (port.ioType == IOType.Input)
                {
                    if (TryGetLackOfItems(port.ItemTypes,out itemRequiredList))
                    {

                        context.operations.Add(new SimOperation()
                        {
                            entityOpAuthor = portRef,
                            type = SimOpType.TakeFromConnection,
                            entityOpAnother = port.connectedObject,
                            items = itemRequiredList.DeepCopy(),
                        });
                    }
                }
                if (port.ioType == IOType.Output)
                {
                    context.operations.Add(new SimOperation()
                    {
                        entityOpAuthor = portRef,
                        type = SimOpType.PutToConnection,
                        entityOpAnother = port.connectedObject,
                        items = port.ItemTypes.ToList(),
                    });
                }
            }
        }

        public bool TryImport(ItemType itemType)
        {
            return TryImportOneItem(itemType);
        }

        public void Import(ItemType itemType)
        {
            foreach (var slot in slots)
            {
                if (slot.ioType != IOType.Input) continue;
                if (slot.CanImportOneItem(itemType))
                {
                    slot.ImportOneItem(itemType);
                    break;
                }
            }
        }

        public bool TryExport(List<ItemType> itemRequiredTypes, out List<ItemType> itemExistTypes)
        {
            itemExistTypes = new List<ItemType>();
            foreach (var slot in slots)
            {
                if (slot.ioType != IOType.Output) continue;
                if (slot.TryGetAbundanceOfItems(itemRequiredTypes, out ItemType itemType))
                {
                    itemExistTypes.Add(itemType);
                }
            }
            if (itemExistTypes.Count > 0) return true;
            return false;
        }
        public void Export(ItemType itemType)
        {
            foreach (var slot in slots)
            {
                if (slot.ioType != IOType.Output) continue;
                if (slot.CanExportOneItem(itemType))
                {
                    slot.ExportOneItem();
                    return;
                }
            }
        }


        public bool TryGetLackOfItems(List<ItemType> listForCheck,out List<ItemType> itemTypes)
        {
            var itemList = new List<ItemType>();
            itemTypes = new List<ItemType>();
            foreach (var slot in slots)
            {
                if (slot.ioType != IOType.Input) continue;

                if (slot.TryGetLackOfItems(listForCheck, out itemList))
                {
                    foreach (var item in itemList)
                    {
                        if (!itemTypes.Contains(item))
                        {
                            itemTypes.Add(item);
                        }
                    }
                }
            }
            if (itemTypes.Count > 0) return true;
            return false;
        }

        private bool TryImportOneItem(ItemType itemType)
        {
            
            foreach (var slot in slots)
            {
                if (slot.ioType != IOType.Input) continue;
                if (slot.CanImportOneItem(itemType))
                {
                    //slot.ImportOneItem(itemType);
                    return true;
                }
            }
            return false;
        }
        private bool TryExportOneItem(ItemType itemType)
        {
            foreach (var slot in slots)
            {
                if (slot.ioType != IOType.Output) continue;
                if (slot.CanExportOneItem(itemType))
                {
                    slot.ExportOneItem();
                    return true;
                }
            }
            return false;
        }

        
        public void Connect(PortRef thisPort, PortRef externalPort)
        {
            if (ID != thisPort.entityId) return;

            var port = ports.Find(x => x.ID == thisPort.portId);

            if (port != null) 
            {
                port.isConnected = true;
                port.connectedObject = externalPort;
            }
        }
        public void Disconnect(PortRef thisPort, PortRef externalPort)
        {
            if (ID != thisPort.entityId) return;

            var port = ports.Find(x => x.ID == thisPort.portId && 
            x.isConnected &&
            x.connectedObject.Equals(externalPort));

            if (port != null)
            {
                port.isConnected = false;
                port.connectedObject = default;
            }
        }
        public void Disconnect(PortRef thisPort)
        {
            if (ID != thisPort.entityId) return;

            var port = ports.Find(x => x.ID == thisPort.portId);

            if (port != null)
            {
                port.isConnected = false;
                port.connectedObject = default;
            }
        }

    }
    [Serializable]
    public class FactoryPort : Port
    {
        [NonSerialized] public bool isConnected;
        [NonSerialized] public PortRef connectedObject;
        public List<ItemType> ItemTypes;
    }

    [Serializable]
    public enum IOType
    {
        Input,
        Output,
    }

    public enum FactoryType
    {
        ExportImport10,
        ExportImport11,
        ExportImport12,
        ExportImport13,
        ExportImport20,
        ExportImport21,
        ExportImport22,
        ExportImport23,
    }
}
