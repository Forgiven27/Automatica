using System;
using System.Collections.Generic;


namespace Simulator
{
    public class ConveyorLine : IEntity, IItemSource, IItemSink, IConnectable
    {
        public uint ID { get; set; }
        bool _isNextFree;
        ProductionTimer _timer = new();
        int _cooldown = 100;
        List<ConveyorSegment> _segments = new(); // направление движения от нулевого

        public ConveyorLine(uint id, uint[] segmentsID)
        {
            ID = id;

            for (int i = 0; i < segmentsID.Length; i++)
            {
                var segment = new ConveyorSegment(segmentsID[i]);    
                if (i > 0)
                {
                    segment.ConnectInputPort(new PortRef(1, _segments[i-1].ID));
                    _segments[i-1].ConnectOutputPort(new PortRef(0,segment.ID));
                }
                _segments.Add(segment);
            }
        }


        public void Work()
        {
            if (_timer.IsReady())
            {
                _isNextFree = false;

                for (int i = _segments.Count - 1; i >= 0; i--)
                {
                    if (i == _segments.Count - 1)
                    {
                        _isNextFree = !_segments[i].IsFull; 
                        continue;
                    }

                    if (_segments[i].IsFull && _isNextFree)
                    {
                        _segments[i].Export(_segments[i].currentType);
                        _segments[i+1].Import(_segments[i].currentType);
                    } else 
                    {
                        _isNextFree = !_segments[i].IsFull;
                    }   
                }
                _timer.Start(_cooldown);
            }
            else
            {
                _timer.TryTick();
            }
            
        }


        public bool TryImport(ItemType itemType)
        {
            return _segments[_segments.Count - 1].TryImport(itemType);
        }
        public void Import(ItemType itemType)
        {
            _segments[0].Import(itemType);
        }

        public bool TryExport(List<ItemType> itemRequiredTypes, out List<ItemType> itemExistTypes)
        {
            return _segments[_segments.Count - 1].TryExport(itemRequiredTypes, out itemExistTypes);
        }
        public void Export(ItemType itemType)
        {
            _segments[_segments.Count - 1].Export(itemType);
        }

        void RemoveItemAtSegment(uint ID)
        {
            _segments.Find(i => i.ID == ID)?.RemoveItem();
        }

        void CreateItemAtSegment(uint ID, ItemType itemType)
        {
            _segments.Find(i => i.ID == ID)?.SetItem(itemType);
        }


        public void Connect(PortRef thisPort, PortRef externalPort)
        {
            if (thisPort.entityId == _segments[0].ID && thisPort.portId == 0)
            {
                _segments[0].ConnectInputPort(externalPort);
            }else if (thisPort.entityId == _segments[_segments.Count - 1].ID && thisPort.portId == 1)
            {
                _segments[_segments.Count - 1].ConnectOutputPort(externalPort);
            }
        }

        public void Disconnect(PortRef thisPort, PortRef externalPort)
        {
            Disconnect(thisPort);
        }
        public void Disconnect(PortRef thisPort)
        {
            if (thisPort.entityId == _segments[0].ID && thisPort.portId == 0)
            {
                _segments[0].DisconnectInputPort();
            }
            else if (thisPort.entityId == _segments[_segments.Count - 1].ID && thisPort.portId == 1)
            {
                _segments[_segments.Count - 1].DisconnectOutputPort();
            }
        }

        public ConveyorItem[] GetItems() 
        { 
            var items = new ConveyorItem[_segments.Count];
            for (int i = 0;i < _segments.Count; i++)
            {
                if (_segments[i].IsFull)
                {
                    var item = new ItemStack(_segments[i].currentType, 1);
                    var conveyorItem = new ConveyorItem(i, item);
                    items[i] = conveyorItem;
                }
                else
                {
                    items[i] = null;
                }
            }

            return items;
        }

    }


    [Serializable]
    public class ItemStack
    {
        [NonSerialized] public int countItems;
        public ItemType itemType;

        public ItemStack(ItemType itemType, int count)
        {
            this.itemType = itemType;
            countItems = count;
        }
    }

    public class ConveyorItem
    {
        public ItemStack itemStack;
        public int orderID;

        public ConveyorItem(int orderID, ItemStack itemStack)
        {
            this.orderID = orderID;
            this.itemStack = itemStack;
        }
    }

    public class ConveyorPort : Port
    {
        public bool isConnected;
        public PortRef connectedObject;
    }
}