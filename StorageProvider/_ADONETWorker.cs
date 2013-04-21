using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace ParseKit.Data.db
{
    /// <summary>
    /// DESCRIPTION: No any exceptions handled
    /// </summary>
    public class ADONETWorker : IDisposable
    {
        string _connectionString;
        bool _keepConnection;
        string _catalog;
        SqlConnection connection;// new SqlConnection(connectionString)

        /// <summary>
        /// Local connection
        /// </summary>
        public ADONETWorker(string catalog, bool keepConnection)
        {
            this._connectionString = string.Format("Data Source=(local);Initial Catalog={0};Integrated Security=true", catalog);
            this._keepConnection = keepConnection;
            this._catalog = catalog;
        }
        /*
        public ADONETWorker(string connectionString, bool keepConnection)
        {
            this._connectionString = connectionString;
            this._keepConnection = keepConnection;
        }
        */
        
        ///<summary>
        ///Exec query string. 
        ///(Param names must be without '@' symbol)
        ///</summary>
        /// <param name="arg">each param must have format like key: [STR_value] value: [STR_value]</param>
        private List<KeyValuePair<string, object>[]> ExecuteQuery(string query, List<KeyValuePair<string, string>> arg = null, CommandBehavior comBehavior = CommandBehavior.Default)
        {
            UpdateConnection();

            SqlCommand cm = new SqlCommand(query, connection);
            if (arg != null)
            {
                foreach (var pair in arg)
                {
                    cm.Parameters.AddWithValue("@" + pair.Key, pair.Value);
                }
            }
            
            SqlDataReader reader = cm.ExecuteReader(comBehavior);

            List<KeyValuePair<string, object>[]> results = new List<KeyValuePair<string, object>[]>();
            while (reader.Read())
            {
                KeyValuePair<string, object>[] resultLine = new KeyValuePair<string, object>[reader.FieldCount];

                for (int i = 0; i < reader.FieldCount; i++)
			    {
                    resultLine[i] = new KeyValuePair<string,object>(reader.GetName(i), reader[i]);
			    }
                results.Add(resultLine);
            }
            reader.Close();
            
            if (!_keepConnection)
                connection.Close();

            return results;
        }

        public List<string> SelectColumn(string table, string columnName, string[] orderColumns = null, string whereParam = null)
        {
            List<KeyValuePair<string, object>[]> results = Select(new string[] { columnName }, table, orderColumns, whereParam);

            return results.ConvertAll<string>(x => { return x[0].Value.ToString(); });
        }

        public List<KeyValuePair<string, object>[]> SelectAll(string table, string[] orderColumns = null, string whereParam = null)
        {
            if (string.IsNullOrEmpty(table))
                throw new ArgumentException();

            string where = whereParam != null ? "WHERE " + whereParam : null;
            string orderBy = string.Empty;
            if (orderColumns != null && orderColumns.Length > 0)
            {
                orderBy = "ORDER BY";
                for (int i = 0; i < orderColumns.Length; i++)
                {
                    orderBy += " " + orderColumns[i];
                }
            }

            string query = string.Format(
            "SELECT * from dbo.{0} "
                + "{1} "
                + "{2}",  table, where, orderBy);

            return ExecuteQuery(query, null);
        }

        public List<KeyValuePair<string, object>[]> Select(string[] columnNames, string table, string[] orderColumns = null, string whereParam = null)
        {
            if (string.IsNullOrEmpty(table) || columnNames == null || columnNames.Length == 0)
	            throw new ArgumentException();

            string columnsQuery = columnNames[0];
            for (int i = 1; i < columnNames.Length; i++)
			{
			    columnsQuery += ", " + columnNames[i];
			}

            string where = whereParam !=null ? "WHERE " + whereParam : null;
            string orderBy = string.Empty;

            if (orderColumns != null && orderColumns.Length > 0)
            {
                orderBy = "ORDER BY";
                for (int i = 0; i < orderColumns.Length; i++)
                {
                    orderBy += " " + orderColumns[i];
                }
            }

            string query = string.Format(
            "SELECT {0} from dbo.{1} "
                + "{2} "
                + "{3}", columnsQuery, table, where, orderBy);

            return ExecuteQuery(query, null);
        }

        /// <summary>
        /// Set first columns in table values from 'values' variable
        /// </summary>
        /// <param name="table">table name</param>
        /// <param name="values">values for insert to first values.Lenght number of columns</param>
        /// <param name="where">WHERE param string (with no 'WHERE' keyword)</param>
        public void Update(string table, string[] values, string where)
        {
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(where) || values == null || values.Length == 0)
                throw new ArgumentException();

            List<string> columns = GetColumnsNames(table);

            if (columns.Count < values.Length)
                throw new ArgumentException("values lenght too big");

            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < values.Length; i++)
            {
                args.Add(new KeyValuePair<string, string>(columns[i], values[i]));
            }

            Update(table, args, where);
        }

        /// <summary>
        /// UPDATE table with args, and row matched WHERE expression 
        /// </summary>
        /// <param name="table">table name</param>
        /// <param name="args">'SET' params with format: 'agrs[n].Key=agrs[n].Value'</param>
        /// <param name="where">WHERE param string (with no 'WHERE' keyword)</param>
        public void Update(string table, List<KeyValuePair<string, string>> args, string where)
        {
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(where) || args == null || args.Count == 0)
	            throw new ArgumentException();

            string set = string.Format("{0}=@{0}", args[0].Key);
            foreach (var arg in args)
	        {
		        set += string.Format(", {0}=@{0}", arg.Key);
	        }

            string query = string.Format("UPDATE {0} " +
                                         "SET {1} " +
                                         "WHERE {2} ", table, set, where);

            ExecuteQuery(query, args);
        }

        /// <summary>
        ///  DELETE FROM {0} {1}, table, whereParam
        ///  [Reset id value to 0 on next record]
        /// </summary>
        /// <param name="table"></param>
        /// <param name="whereParam">where expression without 'WHERE' word</param>
        public void DeleteTable(string table, string whereParam = null)
        {
            if (string.IsNullOrEmpty(table))
                throw new ArgumentException();

            string reset = null;

            if (!string.IsNullOrEmpty(whereParam))
            {
                whereParam = "WHERE " + whereParam;
            }
            else
            {
                reset = string.Format("DBCC CHECKIDENT ({0}, RESEED, -1)", table);
            }

            string query = string.Format("DELETE FROM {0} " +
                                         "{1}" + 
                                         "{2}", table, whereParam, reset);

            ExecuteQuery(query);
        }

        /// <summary>
        /// Delete specified table from current database if exist
        /// </summary>
        /// <param name="table"></param>
        public void DropTable(string table)
        {
            if (string.IsNullOrEmpty(table))
                throw new ArgumentException();

            string query = string.Format("IF OBJECT_ID(N'{0}..{1}', N'U') IS NOT NULL " +
                                        "DROP TABLE {1};", _catalog, table);

            ExecuteQuery(query);
        }

        /// <summary>
        /// create table if not exist, with id column as key
        /// </summary>
        /// <param name="name">table name</param>
        /// <param name="args">columns names and types (using 3 types: int, double, text)</param>
        /// <returns>true if table created of false if any exception genered</returns>
        public bool TryCreateTable(string name, List<KeyValuePair<string, SqlCType>> args)
        {
            if (string.IsNullOrEmpty(name) || args == null || args.Count == 0)
                return false;

            string columns = string.Format("[{0}] {1} NULL", args[0].Key, args[0].Value);

            for (int i = 1; i < args.Count; i++)
            {
                columns += string.Format(", [{0}] {1} NULL", args[i].Key, args[i].Value);
            }

            string query = string.Format("SET ANSI_PADDING ON " +
                                        "GO " +

                                        "CREATE TABLE [dbo].[{0}]" +
                                        "(" +
	                                            "[id] [int] IDENTITY(1,1) NOT NULL," +
	                                            "{1}," +
                                                "CONSTRAINT [PK_Main] PRIMARY KEY CLUSTERED" +
                                            "([id] ASC)" + 
                                            "WITH " + 
                                                "(PAD_INDEX  = OFF," +
                                                "STATISTICS_NORECOMPUTE  = OFF, " + 
                                                "IGNORE_DUP_KEY = OFF, " + 
                                                "ALLOW_ROW_LOCKS  = ON, " + 
                                                "ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]" +
                                        ") " + 
                                        "ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] " +
                                        "GO" +

                                        "ET ANSI_PADDING OFF "+
                                        "GO", name, columns);

            try
            {       
                ExecuteQuery(query);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Insert(string[] values, string table)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException();

            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();

            args.Add(new KeyValuePair<string, string>("val0", values[0]));
            string valuesQuery = "@val0";
            for (int i = 1; i < values.Length; i++)
            {
                args.Add(new KeyValuePair<string, string>("val" + i, values[i]));
                valuesQuery += ", @val" + i;
            }

            string query = string.Format("INSERT INTO dbo.{0} "
                + "VALUES ({1})", table, valuesQuery);

            ExecuteQuery(query, args);
        }

        public void Insert(List<KeyValuePair<string, string>> args, string table)
        {
            if (args == null || args.Count == 0)
	            throw new ArgumentException();

            string columns = args[0].Key; 
            string values = "@" + args[0].Key;

            for (int i = 1; i < args.Count; i++)
            {
                columns += ", " + args[i].Key;
                values += ", @" + args[i].Key;
            }

            string query = string.Format("INSERT INTO dbo.{0} ({1})"
                + " VALUES ({2})", table, columns, values);

            ExecuteQuery(query, args);
        }

        public List<string> GetTablesList()
        {
            string query = string.Format("SELECT * " +
                                        "FROM sys.Tables " +
                                        "GO");

            List<string> tables = new List<string>();
            ExecuteQuery(query).ForEach(x => { tables.Add(x[0].Value.ToString()); });

            return tables;
        }

        public List<string> GetColumnsNames(string table)
        {
            if (string.IsNullOrEmpty(table))
                throw new ArgumentException();

            string query = "SELECT name " +
                            "FROM sys.columns " +
                            "WHERE object_id = OBJECT_ID(N'[dbo].[Main]')";

            List<string> columns = new List<string>();
            ExecuteQuery(query).ForEach(x => { columns.Add(x[0].Value.ToString()); });

            return columns;
        }

        public void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void UpdateConnection()
        {
            if (connection == null)
                connection = new SqlConnection(_connectionString);

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                return;
            }

            if (connection.State == ConnectionState.Broken)
            {
                connection.Close();
                connection.Open();
                return;
            }
        }

        #region Члены IDisposable

        public void Dispose()
        {
            CloseConnection();
        }

        #endregion
    }

    public class SqlCType
    {
        string _typeStr;
        private SqlCType(string type)
        {
            _typeStr = type;
        }

        public static SqlCType @String
        {
            get { return new SqlCType("varchar(MAX)"); }
        }
        public static SqlCType @Int
        {
            get { return new SqlCType("int"); }
        }
        public static SqlCType @Double
        {
            get { return new SqlCType("decimal(18, 5)"); }
        }

        public override string ToString()
        {
            return _typeStr;
        }
    }
}
