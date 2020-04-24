using MyUltimaServer.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyUltimaServer.Accounting
{
    public class Account
    {
        private readonly string m_Username;
        private readonly string m_Password;

        public string Username { get { return m_Username; } }
        public string Password { get { return m_Password; } }

        public Account(string username, string password)
        {
            m_Username = username;
            m_Password = password;
        }
    }
}
