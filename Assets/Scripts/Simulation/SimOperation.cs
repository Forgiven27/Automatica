using System.Collections.Generic;

namespace Simulator
{
    public struct SimOperation
    {

        public SimOpType type;
        public PortRef entityOpAuthor;
        public PortRef entityOpAnother;
        public List<ItemType> items;
    }


    public enum SimOpType
    {
        TakeFromConnection,
        PutToConnection,
        ProduceItem
    }
}