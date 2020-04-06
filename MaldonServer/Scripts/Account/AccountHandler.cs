using System;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Accounting
{
    public class AccountHandler
    {
        public static void Initialize()
        {
            EventSink.AccountLogin += new AccountLoginEventHandler(EventSink_AccountLogin);
            //EventSink.AccountCreate += new AccountCreateEventHandler(EventSink_AccountCreate);
            //EventSink.GameLogin += new GameLoginEventHandler(EventSink_GameLogin);
        }

        public static void EventSink_AccountLogin(AccountLoginEventArgs e)
        {
            string un = e.Username;
            string pw = e.Password;

            e.Accepted = false;
            //Account acct = Accounts.GetAccount(un);
            Console.WriteLine("EventSink_AccountLogin");
            e.Reply = ALRReason.BadPass;
        }

    }
}