using System;
using System.Collections.Generic;
using System.Text;

namespace MaldonServer
{
    public class Utility
    {
		private static readonly Random random = new Random();

		public static int Random(int from, int count)
		{
			if (count == 0)
			{
				return from;
			}
			else if (count > 0)
			{
				return from + random.Next(count);
			}
			else
			{
				return from - random.Next(-count);
			}
		}

		public static double GetAngle(Point3D startLoc, Point3D endLoc)
		{
			float xDiff = endLoc.X - startLoc.X;
			float yDiff = endLoc.Y - startLoc.Y;
			double angle = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
			angle += 180;
			angle = (angle / 2);
			angle += 160;
			return angle;
		}

		public static double GetDistance(Point3D startLoc, Point3D endLoc)
		{
			float deltaX = endLoc.X - startLoc.X;
			float deltaY = endLoc.Y - startLoc.Y;
			float deltaZ = endLoc.Z - startLoc.Z;

			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
		}

        public static Point3D GetLocation(Point3D location, Direction d)
        {
            int x = location.X;
            int y = location.Y;

            switch (d)
            {
                case Direction.North:
                    x--;
                    break;
                case Direction.South:
                    x++;
                    break;
                case Direction.East:
                    y--;
                    break;
                case Direction.West:
                    y++;
                    break;
                case Direction.NorthEast:
                    x--;
                    y--;
                    break;
                case Direction.NorthWest:
                    x--;
                    y++;
                    break;
                case Direction.SouthEast:
                    x++;
                    y--;
                    break;
                case Direction.SouthWest:
                    x++;
                    y++;
                    break;
            }
            return new Point3D(x, y, location.Z);
        }

        public static Array ResizeArray(Array arr, int[] newSizes)
        {
            if (newSizes.Length != arr.Rank)
                throw new ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        }

    }
}
