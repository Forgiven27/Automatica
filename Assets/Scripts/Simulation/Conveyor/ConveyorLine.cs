using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine.Audio;


namespace Simulator
{
    public class ConveyorLine : IEntity, IItemSource, IItemSink, IConnectable
    {
        public string ID { get; set; }

        List<ConveyorItem> listItems = new();
        public List<ConveyorItem> GetItems () => listItems.ToList();
        byte[] line;
        int steps;
        bool isNextFree;
        ConveyorPort inputPort = new();
        ConveyorPort outputPort = new ();
        public ProductionTimer timer = new();
        public ConveyorLine(string id, int steps)
        {
            this.ID = id;
            this.steps = steps;
            this.line = new byte[steps];
            InitPorts();
        }
        public ConveyorLine(string id, int steps, byte[] line, List<ConveyorItem> items)
        {
            this.ID = id;
            this.steps = steps;
            this.line = line;
            listItems = items;
            InitPorts();
        }

        void InitPorts()
        {
            inputPort.ID = "0";
            outputPort.ID = "1";
        }


        public void Work()
        {
            if (timer.IsReady())
            {
                isNextFree = false;
                for (int i = line.Length - 1; i >= 0; i--)
                {
                    if (line[i] == (byte)0)
                    {
                        isNextFree = true;
                        continue;
                    }
                    if (isNextFree && line[i] == (byte)1)
                    {
                        line[i] = (byte)0;
                        line[i + 1] = (byte)1;
                        listItems.Find(x => x.linePlaceID == i).linePlaceID++;
                        isNextFree = false;
                    }
                }
                timer.Start(100);
            }
            else
            {
                timer.TryTick();
            }
            
        }


        public bool TryImport(ItemType itemType)
        {
            return !TryPeekFirst(out ItemType itemT);
        }
        public void Import(ItemType itemType)
        {
            CreateFirst(itemType);
        }

        public bool TryExport(List<ItemType> itemRequiredTypes, out List<ItemType> itemExistTypes)
        {
            itemExistTypes = new List<ItemType>();
            if (TryPeekLast(out ItemType itemT))
            {
                if (itemRequiredTypes.Contains(itemT))
                {
                    itemExistTypes.Add(itemT);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public void Export(ItemType itemType)
        {
            ConsumeLast();
        }


        public bool TryPeekLast(out ItemType itemType)
        {
            if (!outputPort.isConnected)
            {
                itemType = ItemType.None;
                return false;
            }

            if (line[line.Length - 1] == 0)
            {
                itemType = ItemType.None;
                return false;
            }
            foreach (var itemCon in listItems)
            {
                if (itemCon.linePlaceID == line.Length - 1)
                {
                    itemType = itemCon.itemType;
                    return true;
                }
            }
            itemType = ItemType.None;
            return false;
        }

        public bool TryPeekFirst(out ItemType itemType)
        {
            if (line[0] == (byte)0)
            {
                itemType = ItemType.None;
                return false;
            }
            foreach (var itemCon in listItems)
            {
                if (itemCon.linePlaceID == 0)
                {
                    itemType = itemCon.itemType;
                    return true;
                }
            }
            itemType = ItemType.None;
            return false;
        }

        public bool TryPeekAtLineIndex(out ItemType itemType, int index)
        {
            if (line[index] == 0)
            {
                itemType = ItemType.None;
                return false;
            }
            foreach (var itemCon in listItems)
            {
                if (itemCon.linePlaceID == index)
                {
                    itemType = itemCon.itemType;
                    return true;
                }
            }
            itemType =  ItemType.None;
            return false;
        }

        private void ConsumeLast()
        {
            foreach (var itemCon in listItems)
            {
                if (itemCon.linePlaceID == line.Length - 1)
                {
                    RemoveItem(itemCon);
                }
            }
        }

        private void CreateFirst(ItemType itemType)
        {
            listItems.Add(new ConveyorItem() 
            { 
                itemType = itemType,
                linePlaceID = 0, 
                countItems = 1,
                isMove = true
            });
            line[0] = 1;
        }


        void RemoveItem(ConveyorItem item)
        {
            listItems.Remove(item);
            line[item.linePlaceID] = 0;
        } 

        public void Connect(PortRef thisPort, PortRef externalPort)
        {
            if (thisPort.portId == inputPort.ID)
            {
                inputPort.isConnected = true;
                inputPort.connectedObject = externalPort;
            }
            else if(thisPort.entityId == outputPort.ID) 
            {
                outputPort.isConnected = true;
                outputPort.connectedObject = externalPort;
            }
        }

        public void Disconnect(PortRef thisPort, PortRef externalPort)
        {
            if (thisPort.portId == inputPort.ID && inputPort.connectedObject.Equals(externalPort))
            {
                inputPort.isConnected = false;
                inputPort.connectedObject = default;
            }
            else if (thisPort.entityId == outputPort.ID && outputPort.connectedObject.Equals(externalPort))
            {
                outputPort.isConnected = false;
                outputPort.connectedObject = default;
            }
        }
        public void Disconnect(PortRef thisPort)
        {
            if (thisPort.portId == inputPort.ID)
            {
                inputPort.isConnected = false;
                inputPort.connectedObject = default;
            }
            else if (thisPort.entityId == outputPort.ID)
            {
                outputPort.isConnected = false;
                outputPort.connectedObject = default;
            }
        }

    }


    [Serializable]
    public class ConveyorItem
    {
        [NonSerialized] public int countItems;
        public ItemType itemType;
        [NonSerialized] public bool isMove;
        [NonSerialized] public int linePlaceID = 0;
    }

    public class ConveyorPort : Port
    {
        public bool isConnected;
        public PortRef connectedObject;
    }
}