using System.Collections;

namespace SimpleECS
{
    public class SparseSet : IEnumerable<uint>
    {
        struct Enumerator(uint[] dense, uint size) : IEnumerator<uint>
        {
            uint[] _dense = dense;
            uint _size = size;
            uint _current = 0;
            uint _next = 0;

            public uint Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_next < _size)
                {
                    _current = _dense[_next];
                    _next++;
                    return true;
                }

                return false;
            }

            public void Reset() => _next = 0;
        }

        readonly uint _max;
        uint _size;
        uint[] _dense;
        uint[] _sparse;

        public uint Count => _size;

        public SparseSet(uint maxValue)
        {
            _max = maxValue + 1;
            _size = 0;
            _dense = new uint[_max];
            _sparse = new uint[_max];
        }

        public void Add(uint value)
        {
            if (value >= 0 && value < _max && !Contains(value))
            {
                _dense[_size] = value;
                _sparse[value] = _size;
                _size++;
            }
        }

        public void Remove(uint value)
        {
            if (Contains(value))
            {
                _dense[_sparse[value]] = _dense[_size - 1];
                _sparse[_dense[_size - 1]] = _sparse[value];
                _size--;
            }
        }

        public uint Index(uint value) => _dense[value];

        public bool Contains(uint value)
        {
            if (value >= _max || value < 0)
                return false;
            else
                return _sparse[value] < _size && _dense[_sparse[value]] == value;
        }

        public void Clear() => _size = 0;

        public IEnumerator<uint> GetEnumerator() => new Enumerator(this._dense, _size);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override int GetHashCode() => HashCode.Combine(_max, _size, _dense, _sparse, Count);
    }
}
