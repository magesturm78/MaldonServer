using MaldonServer;

namespace MaldonServer.Scripts.Accounting
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
        public IMobile[] Mobiles { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public string UserName { get; set; }
        public bool Banned { get; private set; }
        private string password;

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

        public bool CanCreateCharacter() 
        { 
            return true; 
        }
    }
}