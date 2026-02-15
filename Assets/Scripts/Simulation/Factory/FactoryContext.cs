using System.Collections.Generic;
using UnityEngine;
namespace Simulator
{
    public class FactoryContext
    {
        public string ID { get; private set; }
        //public List<Connection> connections { get; private set; }
        public List<SimOperation> operations { get; private set; }

        public FactoryContext(string factoryID,List<SimOperation> operations)
        {
            ID = factoryID;

            this.operations = operations;
        }

    }
}