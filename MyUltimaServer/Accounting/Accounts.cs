using MyUltimaServer.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using MyUltimaServer.Data;
using MyUltimaServer.Server;

namespace MyUltimaServer.Accounting
{
    public class Accounts
    {
        public delegate void AccountLoginEventHandler(object sender, AccountLoginEventArgs args);
        public event AccountLoginEventHandler AccountLoginSuccessEvent;
        public event AccountLoginEventHandler AccountLoginFailEvent;

        private Dictionary<string, Account> m_AccountList = new Dictionary<string, Account>();

        public Accounts()
        {
            AccountLoginSuccessEvent += AccountSuccessLogin;
            AccountLoginFailEvent += AccountLoginFail;
        }
        public void AuthenticateAccount(string username, string password, Client client)
        {
            if (!m_AccountList.ContainsKey(username))
                On_AccountLoginFail(new Account(username, password), client);
            else
            {
                if (m_AccountList[username].Password == password)
                    On_AccountSuccessLogin(m_AccountList[username], client);
                else
                    On_AccountLoginFail(new Account(username, password), client);
            }
        }

        private void AccountSuccessLogin(object sender, AccountLoginEventArgs e)
        {
            e.Client.AssignAccount(e.Account);
            PacketReference.SendGameServerList(e.Client, ServerState.ShardName);
            
            Console.WriteLine("Account has logged in, Username: {0} Password {1}", e.Account.Username, e.Account.Password);
        }
        private void On_AccountSuccessLogin(Account account, Client client)
        {
            if (AccountLoginSuccessEvent != null)
                AccountLoginSuccessEvent(this, new AccountLoginEventArgs(account, client));
        }
        private void AccountLoginFail(object sender, AccountLoginEventArgs e)
        {
            Console.WriteLine("Account Failed to Login, Username: {0} Password {1}", e.Account.Username, e.Account.Password);
            PacketReference.SendClientLoginFailure(e.Client, 0x00);
            
        }
        private void On_AccountLoginFail(Account account, Client client)
        {
            if (AccountLoginFailEvent != null)
                AccountLoginFailEvent(this, new AccountLoginEventArgs(account, client));
        }

        public bool Deserialize(string datapath)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlNodeList xmlnode;
                FileStream stream = new FileStream(datapath, FileMode.Open, FileAccess.Read);
                xmldoc.Load(stream);
                xmlnode = xmldoc.GetElementsByTagName("Account");

                foreach (XmlElement elem in xmlnode)
                {
                    string username = elem["Username"].InnerText;
                    string password = elem["Password"].InnerText;

                    if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                        m_AccountList.Add(username, new Account(username, password));
                }
                return true;
            }
            catch (Exception e)
            {
                m_AccountList = new Dictionary<string, Account>();
                Console.WriteLine(e);
                return false;
            }
        }
    }

    public class AccountLoginEventArgs : EventArgs
    {
        public Account Account { get; }
        public Client Client { get; }
        public AccountLoginEventArgs(Account account, Client client)
        {
            Account = account;
            Client = client;
        }

    }
    

    
}
