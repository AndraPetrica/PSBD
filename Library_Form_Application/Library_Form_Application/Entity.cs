using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Library_Form_Application
{
    public class Entity
    {
        protected OracleConnection _connection;

        public Entity(OracleConnection conn)
        {
            _connection = conn;
        }
    }
}
