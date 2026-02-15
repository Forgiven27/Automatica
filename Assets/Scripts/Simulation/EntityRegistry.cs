using System.Collections.Generic;
using UnityEngine;
namespace Simulator
{
    public class EntityRegistry
    {
        Dictionary<string, IEntity> entities = new();

        public void Add(IEntity entity)
            => entities[entity.ID] = entity;

        public void Delete(IEntity entity)
            => entities.Remove(entity.ID);

        public void Delete(string ID)
            => entities.Remove(ID);

        public T Get<T>(string ID) where T : class, IEntity
            => entities[ID] as T;
    }
}