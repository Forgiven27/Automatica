using System;
using System.Collections.Generic;

namespace Simulator
{
    public class EntityRegistry
    {
        private readonly Dictionary<uint, IEntity> _entities = new();
        private readonly Dictionary<uint, TransformSim> _transforms = new();

        public event Action<uint> EntityRemoved;

        public void Add(IEntity entity, TransformSim transform)
        {
            _entities[entity.ID] = entity;
            _transforms[entity.ID] = transform;
        }

        public void Add(IEntity entity)
        {
            _entities[entity.ID] = entity;
        }

        public bool TryGetEntity<T>(uint id, out T entity) where T : class, IEntity
        {
            if (_entities.TryGetValue(id, out var e) && e is T typed)
            {
                entity = typed;
                return true;
            }
            entity = null;
            return false;
        }

        public bool TryGetTransform(uint id, out TransformSim transform) =>
            _transforms.TryGetValue(id, out transform);
        public void SetTransform(uint id, TransformSim transform) =>
            _transforms[id] = transform;

        public bool Delete(uint id)
        {
            var removed = _entities.Remove(id) | _transforms.Remove(id);
            if (removed)
                EntityRemoved?.Invoke(id);
            return removed;
        }
    }
}