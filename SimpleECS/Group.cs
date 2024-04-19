using System.Collections;

namespace SimpleECS
{
    public struct Group : IEnumerable<uint>
    {
        Registry _registry;
        Registry.GroupData _groupData;

        internal Group(Registry registry, Registry.GroupData groupData)
        {
            _registry = registry;
            _groupData = groupData;
        }

        public IEnumerator<uint> GetEnumerator() => _groupData.Entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
