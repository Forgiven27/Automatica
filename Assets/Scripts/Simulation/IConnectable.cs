using UnityEngine;

namespace Simulator
{
    public interface IConnectable
    {
        public void Connect(PortRef thisPort, PortRef externalPort);
        public void Disconnect(PortRef thisPort, PortRef externalPort);
        public void Disconnect(PortRef thisPort);
    }
}