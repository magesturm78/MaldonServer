using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public class Serial : IComparable
    {
        private int serial;

        private static Serial lastMobile = Zero;
        private static Serial lastItem = Zero;
        private static Serial lastMail = Zero;

        public static readonly Serial MinusOne = new Serial(-1);
        public static readonly Serial Zero = new Serial(0);

        private Serial(int serial)
        {
            this.serial = serial;
        }

        public static Serial NewMobile
        {
            get
            {
                //while (World.FindMobile(lastMobile = (lastMobile + 1)) != null);
                return lastMobile;
            }
        }

        public static Serial NewItem
        {
            get
            {
                //while (World.FindItem(lastItem = (lastItem + 1)) != null) ;

                return lastItem;
            }
        }

        public static Serial NewMail
        {
            get
            {
                //while (World.FindMail(lastMail = (lastMail + 1)) != null) ;

                return lastMail;
            }
        }

        public int Value
        {
            get
            {
                return serial;
            }
        }

        public override int GetHashCode()
        {
            return serial;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            else if (!(obj is Serial)) throw new ArgumentException();

            int ser = ((Serial)obj).serial;

            if (serial > ser) return 1;
            else if (serial < ser) return -1;
            else return 0;
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Serial)) return false;

            return ((Serial)o).serial == serial;
        }

        public static bool operator ==(Serial l, Serial r)
        {
            return l.serial == r.serial;
        }

        public static bool operator !=(Serial l, Serial r)
        {
            return l.serial != r.serial;
        }

        public static bool operator >(Serial l, Serial r)
        {
            return l.serial > r.serial;
        }

        public static bool operator <(Serial l, Serial r)
        {
            return l.serial < r.serial;
        }

        public static bool operator >=(Serial l, Serial r)
        {
            return l.serial >= r.serial;
        }

        public static bool operator <=(Serial l, Serial r)
        {
            return l.serial <= r.serial;
        }

        public override string ToString()
        {
            return String.Format("0x{0:X8}", serial);
        }

        public static implicit operator int(Serial a)
        {
            return a.serial;
        }

        public static implicit operator Serial(int a)
        {
            return new Serial(a);
        }
    }
}
