using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Class
{
    internal class Connection : IDisposable
    {
        #region Propriedades

        private string MySqlConnectionString
        {
            get
            {
                var mysqluser = "root";
                var mysqlserver = "localhost";
                var pwd = "";
                var database = "redesocial";

                var mysqlConnectionString = new MySqlConnectionStringBuilder()
                {
                    Server = mysqlserver,
                    UserID = mysqluser,
                    Password = pwd,
                    Database = database

                }.ToString();

                return mysqlConnectionString;
            }
        }

        public MySqlConnection MySqlConnection
        {
            get
            {
                if (_mysqlConnection == null)
                {
                    _mysqlConnection = new MySqlConnection(MySqlConnectionString);
                    _mysqlConnection.Open();
                }
                return _mysqlConnection;
            }
        }

        private MySqlConnection _mysqlConnection = null;

        public void Dispose()
        {
            _mysqlConnection?.Close();
            _mysqlConnection?.Dispose();
        }

        #endregion Propriedades
    }


}