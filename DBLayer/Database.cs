using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DBLayer
{
    public abstract class Database
    {
        // Fields
        protected object objConnection;
        protected string strConnectionString;

        // Methods
        protected Database()
        {

        }
        public abstract object BeginTransaction();
        public abstract void CloseConnection();
        public abstract void CommitTransaction(object Transaction);
        public abstract DataSet ExecuteDataSet(string strCommandText, CommandType cmdType, params object[] cmdParams);
        public abstract DataTable ExecuteDataTable(string strCommandText, CommandType cmdType, params object[] cmdParams);
        public abstract Params ExecuteStoredProcedure(string strProcedureName, params object[] cmdParams);
        public abstract Params ExecuteStoredProcedureWithTransaction(object trnTransaction, string strProcedureName, params object[] cmdParams);
        public abstract bool ExecuteText(string strCommandText, CompileAs type);
        public abstract void RollBackTransaction(object Transaction);
        public abstract bool TestConnection();

    }
}
