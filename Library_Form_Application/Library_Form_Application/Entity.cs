using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Library_Form_Application
{
    public struct PubDate
    {
        String day;
        String month;
        String year;
        public PubDate(String d, String m, String y)
        {
            day = d;
            month = m;
            year = y;
        }
        public String ToStringDate()
        {
            String ret = "to_date('" + day + "-" + month + "-" + year + "','DD-MM-YYYY')";
            return ret;
        }
    }

    public class Entity
    {
        protected OracleConnection _connection;

        public Entity(OracleConnection conn)
        {
            _connection = conn;
        }
    }
}
