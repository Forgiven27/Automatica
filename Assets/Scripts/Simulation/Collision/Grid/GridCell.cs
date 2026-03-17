using System;

namespace Simulator
{
    public struct GridCell : IEquatable<GridCell>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public GridCell(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public bool Equals(GridCell other)
        {
            if (X == other.X && Y == other.Y && Z == other.Z) return true;
            return false;
        }

        public override bool Equals(object obj) => obj is GridCell other && Equals(other);

    }
}
