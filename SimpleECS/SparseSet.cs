using System.Collections;

namespace SimpleECS
{
    public class SparseSet : IEnumerable<uint>
    {
        struct Enumerator(uint[] dense, uint size) : IEnumerator<uint>
        {
            uint[] dense = dense;
            uint size = size;
            uint current = 0;
            uint next = 0;

            public uint Current => current;

            object IEnumerator.Current => current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (next < size)
                {
                    current = dense[next];
                    next++;
                    return true;
                }

                return false;
            }

            public void Reset() => next = 0;
        }

        readonly uint max;
        uint size;
        uint[] dense;
        uint[] sparse;

        public uint Count => size;

        public SparseSet(uint maxValue)
        {
            max = maxValue + 1;
            size = 0;
            dense = new uint[max];
            sparse = new uint[max];
        }

        public void Add(uint value)
        {
            if (value >= 0 && value < max && !Contains(value))
            {
                dense[size] = value;
                sparse[value] = size;
                size++;
            }
        }

        public void Remove(uint value)
        {
            if (Contains(value))
            {
                dense[sparse[value]] = dense[size - 1];
                sparse[dense[size - 1]] = sparse[value];
                size--;
            }
        }

        public uint Index(uint value) => dense[value];

        public bool Contains(uint value)
        {
            if (value >= max || value < 0)
                return false;
            else
                return sparse[value] < size && dense[sparse[value]] == value;
        }

        public void Clear() => size = 0;

        public IEnumerator<uint> GetEnumerator() => new Enumerator(this.dense, size);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override int GetHashCode() =>
            System.HashCode.Combine(max, size, dense, sparse, Count);
    }
}
