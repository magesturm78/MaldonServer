using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
	public interface IPoint2D
	{
		int X { get;  }
		int Y { get;  }
	}

	public interface IPoint3D : IPoint2D
	{
		byte Z { get;  }
	}

	public struct Point3D : IPoint3D
	{
		public int X { get; private set; }
		public int Y { get; private set; }
		public byte Z { get; private set; }

		public static readonly Point3D Zero = new Point3D(0, 0, 0);

		public Point3D(int x, int y, byte z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Point3D(IPoint3D p) : this(p.X, p.Y, p.Z)
		{
		}

		public Point3D(IPoint2D p, byte z) : this(p.X, p.Y, z)
		{
		}


		public override string ToString()
		{
			return String.Format("({0}, {1}, {2})", X, Y, Z);
		}

		public override bool Equals(object o)
		{
			if (o == null || !(o is IPoint3D)) return false;

			IPoint3D p = (IPoint3D)o;

			return X == p.X && Y == p.Y && Z == p.Z;
		}

		public override int GetHashCode()
		{
			return X ^ Y ^ Z;
		}

		public static Point3D Parse(string value)
		{
			int start = value.IndexOf('(');
			int end = value.IndexOf(',', start + 1);

			string param1 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			string param2 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(')', start + 1);

			string param3 = value.Substring(start + 1, end - (start + 1)).Trim();

			return new Point3D(Convert.ToInt32(param1), Convert.ToInt32(param2), Convert.ToByte(param3));
		}

		public static bool operator ==(Point3D l, Point3D r)
		{
			return l.X == r.X && l.Y == r.Y && l.Z == r.Z;
		}

		public static bool operator !=(Point3D l, Point3D r)
		{
			return l.X != r.X || l.Y != r.Y || l.Z != r.Z;
		}

		public static bool operator ==(Point3D l, IPoint3D r)
		{
			return l.X == r.X && l.Y == r.Y && l.Z == r.Z;
		}

		public static bool operator !=(Point3D l, IPoint3D r)
		{
			return l.X != r.X || l.Y != r.Y || l.Z != r.Z;
		}
	}
}
