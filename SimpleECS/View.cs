using System.Collections;

namespace SimpleECS
{
    public struct View<T>(Registry registry) : IEnumerable<uint>
    {
        Registry _registry = registry;

        public IEnumerator<uint> GetEnumerator() => _registry.Assure<T>().Set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public struct View<T, U>(Registry registry) : IEnumerable<uint>
    {
        struct Enumerator : IEnumerator<uint>
        {
            Registry _registry;
            IComponentStore _store;
            IEnumerator<uint> _setEnumerator;

            public Enumerator(Registry registry)
            {
                _registry = registry;
                var store1 = registry.Assure<T>();
                var store2 = registry.Assure<U>();

                if (store1.Count > store2.Count)
                {
                    _setEnumerator = store2.Entities.GetEnumerator();
                    _store = store1;
                }
                else
                {
                    _setEnumerator = store1.Entities.GetEnumerator();
                    _store = store2;
                }
            }

            public uint Current => _setEnumerator.Current;

            object IEnumerator.Current => _setEnumerator.Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                while (_setEnumerator.MoveNext())
                {
                    var entityId = _setEnumerator.Current;
                    if (!_store.Contains(entityId))
                        continue;
                    return true;
                }
                return false;
            }

            public void Reset() => _setEnumerator.Reset();
        }

        Registry registry = registry;

        public IEnumerator<uint> GetEnumerator() => new Enumerator(registry);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public struct View<T, U, V>(Registry registry) : IEnumerable<uint>
    {
        struct Enumerator : IEnumerator<uint>
        {
            static IComponentStore[] sorter = new IComponentStore[3];
            Registry registry;
            IComponentStore store1;
            IComponentStore store2;
            IEnumerator<uint> setEnumerator;

            public Enumerator(Registry registry)
            {
                this.registry = registry;

                sorter[0] = registry.Assure<T>();
                sorter[1] = registry.Assure<U>();
                sorter[2] = registry.Assure<V>();
                Array.Sort(
                    sorter,
                    (first, second) => first.Entities.Count.CompareTo(second.Entities.Count)
                );

                setEnumerator = sorter[0].Entities.GetEnumerator();
                store1 = sorter[1];
                store2 = sorter[2];
            }

            public uint Current => setEnumerator.Current;

            object IEnumerator.Current => setEnumerator.Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                while (setEnumerator.MoveNext())
                {
                    var entityId = setEnumerator.Current;
                    if (!store1.Contains(entityId) || !store2.Contains(entityId))
                        continue;
                    return true;
                }
                return false;
            }

            public void Reset() => setEnumerator.Reset();
        }

        Registry registry = registry;

        public IEnumerator<uint> GetEnumerator() => new Enumerator(registry);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
