using MaldonServer;

namespace MaldonServer.Accounting
{
    public class Account : IAccount
    {
        private Mobile[] Mobiles;

        public int Count
        {
            get
            { 
                int count = 0;

                for (int i = 0; i < this.Length; ++i)
                {
                    if (this[i] != null)
                        ++count;
                }

                return count;
            }
        }

        public Mobile this[int index]
        {
            get
            {
                if (index >= 0 && index < Mobiles.Length)
                {
                    Mobile m = Mobiles[index];

                    if (m != null )//&& m.Deleted)
                    {
                        m.Account = null;
                        Mobiles[index] = m = null;
                    }
                    return m;
                }
                return null;
            }
            set
            {
                if (index >= 0 && index < Mobiles.Length)
                {
                    if (Mobiles[index] != null)
                        Mobiles[index].Account = null;

                    Mobiles[index] = value;

                    if (Mobiles[index] != null)
                        Mobiles[index].Account = this;
                }
            }
        }

        public int Length
        {
            get { return Mobiles.Length; }
        }

        public static void Initialize()
        {
        }
    }
}