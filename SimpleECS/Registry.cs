namespace SimpleECS
{
    public class Registry(uint maxEntities)
    {
        internal class GroupData(
            Registry _registry,
            int _hashCode,
            params IComponentStore[] _components
        )
        {
            public int HashCode = _hashCode;
            public SparseSet Entities = new(_registry._maxEntities);
            IComponentStore[] _componentStores = _components;

            internal void OnEntityAdded(uint entityId)
            {
                if (!Entities.Contains(entityId))
                {
                    foreach (var store in _componentStores)
                        if (!store.Contains(entityId))
                            return;
                    Entities.Add(entityId);
                }
            }

            internal void OnEntityRemoved(uint entityId)
            {
                if (Entities.Contains(entityId))
                    Entities.Remove(entityId);
            }
        }

        readonly uint _maxEntities = maxEntities;
        Dictionary<Type, IComponentStore> _data = [];
        uint _nextEntity = 0;
        List<GroupData> _groups = [];

        public ComponentStore<T> Assure<T>()
        {
            var type = typeof(T);
            if (_data.TryGetValue(type, out var store))
                return (ComponentStore<T>)_data[type];

            var newStore = new ComponentStore<T>(_maxEntities);
            _data[type] = newStore;
            return newStore;
        }

        public Entity Create() => new(_nextEntity++);

        public void Destroy(Entity entity)
        {
            foreach (var store in _data.Values)
                store.RemoveIfContains(entity.Id);
        }

        public void AddComponent<T>(Entity entity, T component) =>
            Assure<T>().Add(entity, component);

        public ref T GetComponent<T>(Entity entity) => ref Assure<T>().Get(entity.Id);

        public bool TryGetComponent<T>(Entity entity, ref T component)
        {
            var store = Assure<T>();
            if (store.Contains(entity.Id))
            {
                component = store.Get(entity.Id);
                return true;
            }

            return false;
        }

        public void RemoveComponent<T>(Entity entity) => Assure<T>().RemoveIfContains(entity.Id);

        public View<T> View<T>() => new(this);

        public View<T, U> View<T, U>() => new(this);

        public View<T, U, V> View<T, U, V>() => new(this);

        public Group Group<T, U>()
        {
            var hash = System.HashCode.Combine(typeof(T), typeof(U));

            foreach (var group in _groups)
                if (group.HashCode == hash)
                    return new Group(this, group);

            var groupData = new GroupData(this, hash, Assure<T>(), Assure<U>());
            _groups.Add(groupData);

            Assure<T>().OnAdd += groupData.OnEntityAdded;
            Assure<U>().OnAdd += groupData.OnEntityAdded;

            Assure<T>().OnRemove += groupData.OnEntityRemoved;
            Assure<U>().OnRemove += groupData.OnEntityRemoved;

            foreach (var entityId in View<T, U>())
                groupData.Entities.Add(entityId);

            return new Group(this, groupData);
        }

        public Group Group<T, U, V>()
        {
            var hash = System.HashCode.Combine(typeof(T), typeof(U), typeof(V));

            foreach (var group in _groups)
                if (group.HashCode == hash)
                    return new Group(this, group);

            var groupData = new GroupData(this, hash, Assure<T>(), Assure<U>(), Assure<V>());
            _groups.Add(groupData);

            Assure<T>().OnAdd += groupData.OnEntityAdded;
            Assure<U>().OnAdd += groupData.OnEntityAdded;
            Assure<V>().OnAdd += groupData.OnEntityAdded;

            Assure<T>().OnRemove += groupData.OnEntityRemoved;
            Assure<U>().OnRemove += groupData.OnEntityRemoved;
            Assure<V>().OnRemove += groupData.OnEntityRemoved;

            foreach (var entityId in View<T, U, V>())
                groupData.Entities.Add(entityId);

            return new Group(this, groupData);
        }
    }
}
