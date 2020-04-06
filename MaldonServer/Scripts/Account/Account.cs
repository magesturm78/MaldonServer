using MaldonServer;

namespace MaldonServer.Accounting
{
    public enum AccessLevel
    {
		Peasant = 0,
		Citizen = 1,
		Moderator = 2,
		GameMaster = 3,
		Developer = 4,
		Administrator = 5
    }

    public class Account : IAccount
    {
        private IMobile[] Mobiles;
        public AccessLevel AccessLevel { get; set; }
        public string UserName { get; private set; }
        public bool Banned { get; private set; }
        private string password;

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

        public IMobile this[int index]
        {
            get
            {
                if (index >= 0 && index < Mobiles.Length)
                {
                    IMobile m = Mobiles[index];

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

        public Account(string username, string password)
        {
            this.UserName = username;
            this.password = password;
            Banned = false;
        }

        public bool ValidPassword(string pw)
        {
            return (string.Compare(password, pw) == 0);
        }
    }
}