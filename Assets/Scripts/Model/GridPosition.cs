using InfinitePorker.Enums;

namespace InfinitePorker.Model
{
    public struct GridPosition
    {
        public int X { get; }
        public int Y { get; }

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static GridPosition FromIndex(int index, int gridWidth = 4)
        {
            return new GridPosition(index % gridWidth, index / gridWidth);
        }

        public int ToIndex(int gridWidth = 4)
        {
            return Y * gridWidth + X;
        }

        public Direction GetDirectionTo(GridPosition other)
        {
            int deltaX = other.X - X;
            int deltaY = other.Y - Y;

            if (deltaX == 1 && deltaY == 0) return Direction.Right;
            if (deltaX == -1 && deltaY == 0) return Direction.Left;
            if (deltaX == 0 && deltaY == 1) return Direction.Down;
            if (deltaX == 0 && deltaY == -1) return Direction.Up;

            return Direction.None;
        }

        // 指定されたGridPositionが隣接（上下左右）しているかを判定
        public bool IsAdjacentTo(GridPosition other)
        {
            return GetDirectionTo(other) != Direction.None;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}