using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace Connection
{
    public class Connection
    {
        private OracleConnection con;
        private String connectionString = "DATA SOURCE=localhost:1521/XE;USER ID=HR; PASSWORD = hr";
        private bool IsInitilized = false;

        public bool Init()
        {
            bool bRet = true;
            try
            {               
                con = new OracleConnection();
                con.ConnectionString = connectionString;
                con.Open();
            }
            catch(Exception ex)
            {
                bRet = false;
                throw new Exception("Connection to database failed. Error : " + ex.Message);              
            }
            IsInitilized = bRet;
            return bRet;
        }


        public bool Login(String username, String password)
        {
            bool bRet = false;
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = string.Format("select name from PSBD_USERS where username = '{0}' and pass = '{1}'", username, password) ;
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    bRet = true;
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Database connection problems:" + ex.Message);
            }
            return bRet;

        }

        public OracleConnection GetConnectionObject ()
        {
            if (!IsInitilized)
                Init();
            return con;
        }
             
    }
}
