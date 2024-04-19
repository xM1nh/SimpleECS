namespace SimpleECS
{
    public interface IComponentStore
    {
        bool Contains(uint entityId);

        void RemoveIfContains(Entity entity) => RemoveIfContains(entity.Id);

        void RemoveIfContains(uint entityId);

        SparseSet Entities { get; }
    }

    public class ComponentStore<T>(uint maxComponents) : IComponentStore
    {
        public event Action<uint> OnAdd = _ => { };
        public event Action<uint> OnRemove = _ => { };

        public SparseSet Set = new(maxComponents);
        public SparseSet Entities => Set;
        T[] _instances = new T[maxComponents];

        public uint Count => Set.Count;

        public void Add(Entity entity, T value)
        {
            Set.Add(entity.Id);
            _instances[Set.Index(entity.Id)] = value;
            OnAdd?.Invoke(entity.Id);
        }

        public ref T Get(uint entityId) => ref _instances[Set.Index(entityId)];

        public bool Contains(uint entityId) => Set.Contains(entityId);

        public void RemoveIfContains(uint entityId)
        {
            if (Set.Contains(entityId))
                Remove(entityId);
        }

        void Remove(uint entityId)
        {
            Set.Remove(entityId);
            OnRemove?.Invoke(entityId);
        }
    }
}
