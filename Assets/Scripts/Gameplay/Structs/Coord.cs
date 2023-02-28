using System;
using Unity.Netcode;

namespace Gameplay.Structs
{
    [Serializable]
    public struct Coord : INetworkSerializable, IEquatable<Coord>
    {
        public int x;
        public int y;

        public Coord(int x, int y) { this.x = x; this.y = y; }
        
        public override string ToString()
        {
            return $"({x},{y})";
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
        }
        public bool Equals(Coord other)
        {
            return x == other.x && y == other.y;
        }

    }
}