namespace SimpleECS
{
    public readonly struct Entity(uint id)
    {
        public readonly uint Id = id;

        public static implicit operator Entity(uint id) => new(id);

        public override int GetHashCode() => Id.GetHashCode();
    }
}
