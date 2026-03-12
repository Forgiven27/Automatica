
using System.Collections.Generic;

namespace Simulator
{
    public class ConveyorSegment :IEntity, IItemSource, IItemSink, IConnectable
    {
        public uint ID {  get; set; }

        public uint LineID { get; set; }

        public ConveyorPort inputPort { get; private set; }
        public ConveyorPort outputPort { get; private set; }
        public bool IsFull { get; private set; }

        public ItemType currentType { get; private set; }


        public ConveyorSegment(uint ID)
        {
            this.ID = ID;
            inputPort = new ConveyorPort() 
            {
                ID = 0,
                ioType = IOType.Input,
                isConnected = false,
            };
            outputPort = new ConveyorPort()
            {
                ID = 1,
                ioType = IOType.Output,
                isConnected = false,
            };
            IsFull = false;
        }

        public void ConnectInputPort(PortRef portRef)
        {
            inputPort = new ConveyorPort()
            {
                ID = 0,
                ioType = IOType.Input,
                isConnected = true,
                connectedObject = portRef
            };
        }

        public void ConnectOutputPort(PortRef portRef)
        {
            outputPort = new ConveyorPort()
            {
                ID = 1,
                ioType = IOType.Output,
                isConnected = true,
                connectedObject = portRef
            };
        }

        public void DisconnectInputPort()
        {
            inputPort = new ConveyorPort()
            {
                ID = 0,
                ioType = IOType.Input,
                isConnected = false,
                connectedObject = default
            };
        }

        public void DisconnectOutputPort()
        {
            outputPort = new ConveyorPort()
            {
                ID = 1,
                ioType = IOType.Output,
                isConnected = false,
                connectedObject = default
            };
        }

        public void SetItem(ItemType itemType)
        {
            IsFull = true;
            currentType = itemType;
        }

        public void RemoveItem()
        {
            IsFull = false;
            currentType = default;
        }

        public bool TryImport(ItemType itemType)
        {
            if (IsFull) return false;
            //if (itemType != currentType) return false;
            return true;
        }
        public void Import(ItemType itemType)
        {
            if (IsFull) return;
            IsFull = true;
            currentType = itemType;
        }

        public bool TryExport(List<ItemType> itemRequiredTypes, out List<ItemType> itemExistTypes)
        {
            itemExistTypes = null;
            if (!IsFull) return false;
            if (!itemRequiredTypes.Contains(currentType)) return false;
            
            itemExistTypes = new List<ItemType>();
            itemExistTypes.Add(currentType);
            return true;
        }

        public bool TryExport()
        {
            return IsFull;
        }

        public void Export(ItemType itemType)
        {
            if (!IsFull) return;
            if (itemType != currentType) return;
            IsFull = false;
            currentType = default;
        }
        public ItemType Export()
        {
            ItemType itemToExport = currentType;
            IsFull = false;
            currentType = default;
            return itemToExport;
        }


        public void Connect(PortRef thisPort, PortRef externalPort)
        {
            if (thisPort.portId == inputPort.ID)
            {
                inputPort.isConnected = true;
                inputPort.connectedObject = externalPort;
            }
            else if (thisPort.entityId == outputPort.ID)
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
}