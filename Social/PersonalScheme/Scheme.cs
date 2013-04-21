using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Social.PersonalScheme
{
    class Scheme
    {
        public object SchemeParams;
        public string Name { get { if (string.IsNullOrEmpty(_name)) _name = CreateName(); return _name; } }


        private string CreateName()
        {
            throw new NotImplementedException();
        }

        string _name;
        string _secondName;
        string _lastName;
        string _login;
        string _passwd;
        string _email;
    }
}
