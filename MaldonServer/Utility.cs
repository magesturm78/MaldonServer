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
	}
}
