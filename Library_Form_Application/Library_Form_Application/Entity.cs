using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Library_Form_Application
{
    public struct CustomDate
    {
        String day;
        String month;
        String year;

        public CustomDate(String d, String m, String y)
        {
            day = d;
            month = m;
            year = y;
        }

        public CustomDate(String stringDate)
        {
            String[] lines = stringDate.Split("-./".ToCharArray());
            day = lines[0];
            month = lines[1];
            year = lines[2];
        }

        public String DateToString()
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
