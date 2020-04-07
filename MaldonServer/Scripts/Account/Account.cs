using MaldonServer;

namespace MaldonServer.Scripts.Accounting
{

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