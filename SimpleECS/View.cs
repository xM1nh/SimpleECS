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

        Registry _registry = registry;

        public IEnumerator<uint> GetEnumerator() => new Enumerator(_registry);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public struct View<T, U, V>(Registry registry) : IEnumerable<uint>
    {
        struct Enumerator : IEnumerator<uint>
        {
            static IComponentStore[] _sorter = new IComponentStore[3];
            Registry _registry;
            IComponentStore _store1;
            IComponentStore _store2;
            IEnumerator<uint> _setEnumerator;

            public Enumerator(Registry registry)
            {
                _registry = registry;

                _sorter[0] = registry.Assure<T>();
                _sorter[1] = registry.Assure<U>();
                _sorter[2] = registry.Assure<V>();
                Array.Sort(
                    _sorter,
                    (first, second) => first.Entities.Count.CompareTo(second.Entities.Count)
                );

                _setEnumerator = _sorter[0].Entities.GetEnumerator();
                _store1 = _sorter[1];
                _store2 = _sorter[2];
            }

            public uint Current => _setEnumerator.Current;

            object IEnumerator.Current => _setEnumerator.Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                while (_setEnumerator.MoveNext())
                {
                    var entityId = _setEnumerator.Current;
                    if (!_store1.Contains(entityId) || !_store2.Contains(entityId))
                        continue;
                    return true;
                }
                return false;
            }

            public void Reset() => _setEnumerator.Reset();
        }

        Registry _registry = registry;

        public IEnumerator<uint> GetEnumerator() => new Enumerator(_registry);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
