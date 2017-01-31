using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace DBLayer
{
    public static class DbProvider
    {
        // Methods
        public static Database CreateDatabase(string ConnectionStringName)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            switch (settings.ProviderName.ToUpper())
            {
                case "ORACLE":
                    return OracleDatabase.CreateDatabase(settings.ConnectionString);

                case "SQLSERVER":
                    return SqlDatabase.CreateDatabase(settings.ConnectionString);
            }
            return null;
        }

        public static Database CreateDatabase(DbTypes DatabaseType, string ConnectionString)
        {
            switch (DatabaseType)
            {
                case DbTypes.Oracle:
                    return OracleDatabase.CreateDatabase(ConnectionString);

                case DbTypes.SqlServer:
                    return SqlDatabase.CreateDatabase(ConnectionString);
            }
            return null;
        }
    }
}
