using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Data;

namespace DBLayer
{
    public class SqlDatabase : Database
    {
        // Methods
        private SqlDatabase()
        {
        }

        private SqlDatabase(string ConnectionString)
        {
            base.strConnectionString = ConnectionString;
            base.objConnection = new SqlConnection(base.strConnectionString);
        }

        public override object BeginTransaction()
        {
            SqlConnection objConnection = (SqlConnection)base.objConnection;
            if (objConnection == null)
            {
                return null;
            }
            if (objConnection.State != ConnectionState.Open)
            {
                objConnection.Open();
            }
            return objConnection.BeginTransaction();
        }

        public override void CloseConnection()
        {
            if (base.objConnection != null)
            {
                SqlConnection objConnection = (SqlConnection)base.objConnection;
                if (objConnection.State == ConnectionState.Open)
                {
                    objConnection.Close();
                }
            }
        }

        public override void CommitTransaction(object Transaction)
        {
            if (Transaction != null)
            {
                ((SqlTransaction)Transaction).Commit();
            }
        }

        public static Database CreateDatabase(string ConnectionString) =>
            new SqlDatabase(ConnectionString);

        public override DataSet ExecuteDataSet(string strCommandText, CommandType cmdType, params object[] cmdParams)
        {
            SqlConnection objConnection = (SqlConnection)base.objConnection;
            SqlCommand command = new SqlCommand(strCommandText, objConnection)
            {
                CommandType = cmdType
            };
            if (cmdType == CommandType.StoredProcedure)
            {
                try
                {
                    if (objConnection.State != ConnectionState.Open)
                    {
                        objConnection.Open();
                    }
                    SqlCommandBuilder.DeriveParameters(command);
                    objConnection.Close();
                }
                catch (SqlException exception)
                {
                    throw exception;
                }
                for (int i = 1; i < command.Parameters.Count; i++)
                {
                    if (cmdParams.Length <= (i - 1))
                    {
                        throw new Exception("Parameter Count does not match.");
                    }
                    command.Parameters[i].Value = cmdParams[i - 1];
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet;
        }

        public override DataTable ExecuteDataTable(string strCommandText, CommandType cmdType, params object[] cmdParams)
        {
            SqlConnection objConnection = (SqlConnection)base.objConnection;
            SqlCommand command = new SqlCommand(strCommandText, objConnection)
            {
                CommandType = cmdType
            };
            if (cmdType == CommandType.StoredProcedure)
            {
                try
                {
                    if (objConnection.State != ConnectionState.Open)
                    {
                        objConnection.Open();
                    }
                    SqlCommandBuilder.DeriveParameters(command);
                    objConnection.Close();
                }
                catch (SqlException exception)
                {
                    throw exception;
                }
                for (int i = 1; i < command.Parameters.Count; i++)
                {
                    if (cmdParams.Length <= (i - 1))
                    {
                        throw new Exception("Parameter Count does not match.");
                    }
                    command.Parameters[i].Value = cmdParams[i - 1];
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        public override Params ExecuteStoredProcedure(string strProcedureName, params object[] cmdParams)
        {
            Params params2;
            Params @params = new SqlParams();
            SqlConnection objConnection = (SqlConnection)base.objConnection;
            try
            {
                SqlCommand command = new SqlCommand(strProcedureName, objConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                if (objConnection.State != ConnectionState.Open)
                {
                    objConnection.Open();
                }
                SqlCommandBuilder.DeriveParameters(command);
                for (int i = 1; i < command.Parameters.Count; i++)
                {
                    if (command.Parameters[i].Direction == ParameterDirection.Input)
                    {
                        if (cmdParams.Length <= (i - 1))
                        {
                            throw new Exception("Parameter Count does not match.");
                        }
                        command.Parameters[i].Value = cmdParams[i - 1];
                    }
                    else
                    {
                        command.Parameters[i].Value = DBNull.Value;
                    }
                }
                command.ExecuteNonQuery();
                foreach (SqlParameter parameter in command.Parameters)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Output))
                    {
                        @params.Add(parameter);
                    }
                }
                params2 = @params;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.CloseConnection();
            }
            return params2;
        }

        public override Params ExecuteStoredProcedureWithTransaction(object trnTransaction, string strProcedureName, params object[] cmdParams)
        {
            Params params2;
            Params @params = new SqlParams();
            SqlConnection objConnection = (SqlConnection)base.objConnection;
            SqlTransaction transaction = (SqlTransaction)trnTransaction;
            try
            {
                SqlCommand command = new SqlCommand(strProcedureName, objConnection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };
                if (objConnection.State == ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                SqlCommandBuilder.DeriveParameters(command);
                for (int i = 1; i < command.Parameters.Count; i++)
                {
                    if (command.Parameters[i].Direction == ParameterDirection.Input)
                    {
                        if (cmdParams.Length <= (i - 1))
                        {
                            throw new Exception("Parameter Count does not match.");
                        }
                        command.Parameters[i].Value = cmdParams[i - 1];
                    }
                    else
                    {
                        command.Parameters[i].Value = DBNull.Value;
                    }
                }
                command.ExecuteNonQuery();
                foreach (SqlParameter parameter in command.Parameters)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Output))
                    {
                        @params.Add(parameter);
                    }
                }
                params2 = @params;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return params2;
        }

        public override bool ExecuteText(string strCommandText, CompileAs type)
        {
            bool flag;
            SqlConnection objConnection = (SqlConnection)base.objConnection;
            SqlCommand command = new SqlCommand(strCommandText, objConnection)
            {
                CommandType = CommandType.Text
            };
            try
            {
                if (objConnection.State != ConnectionState.Open)
                {
                    objConnection.Open();
                }
                command.ExecuteNonQuery();
                flag = true;
            }
            catch
            {
                flag = false;
            }
            finally
            {
                this.CloseConnection();
            }
            return flag;
        }

        public override void RollBackTransaction(object Transaction)
        {
            if (Transaction != null)
            {
                ((SqlTransaction)Transaction).Rollback();
            }
        }

        public override bool TestConnection()
        {
            bool flag;
            SqlConnection connection = new SqlConnection(base.strConnectionString);
            try
            {
                connection.Open();
                connection.Close();
                flag = true;
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return flag;
        }

        // Nested Types
        private class SqlParams : Params
        {
            // Fields
            private Dictionary<string, int> dctParams = new Dictionary<string, int>();
            private List<SqlParameter> lstParams = new List<SqlParameter>();

            // Methods
            public override void Add(object Param)
            {
                SqlParameter item = (SqlParameter)Param;
                this.lstParams.Add(item);
                this.dctParams.Add(item.ParameterName.ToUpper(), this.lstParams.Count - 1);
            }

            // Properties
            public override object this[int index] =>
                this.lstParams[index].Value;

            public override object this[string ParamName]
            {
                get
                {
                    if (this.dctParams.ContainsKey(ParamName.ToUpper()))
                    {
                        return this.lstParams[this.dctParams[ParamName.ToUpper()]].Value;
                    }
                    return null;
                }
            }
        }
    }
}
