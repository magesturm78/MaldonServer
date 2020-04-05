using System;
using System.Collections.Generic;
using System.Text;

namespace MaldonServer
{
    public class Utility
    {
		private static Random random = new Random();

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
	}
}
