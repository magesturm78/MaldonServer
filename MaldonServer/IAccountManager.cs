using MaldonServer.Network;

namespace MaldonServer
{
    public interface IAccountManager
    {
        void CreateAccount(PlayerSocket ps, string un, string pw, string em);
        void LoginAccount(PlayerSocket ps, string un, string pw);
        void LostPassword(PlayerSocket ps, string accountName);
    }
}
