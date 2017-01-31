using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Configuration;

namespace DBLayer
{
    public static class DataLoaderProvider
    {
        // Methods
        
        public static DataLoader CreateDataLoader(string ConnectionStringName, DataFileInfo flFileInfo, int ThreadCount, ThreadPriority threadPriority)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            switch (settings.ProviderName.ToUpper())
            {
                case "ORACLE":
                    return OracleDataLoader.CreateDataLoader(settings.ConnectionString, flFileInfo, ThreadCount, threadPriority, 0);

                case "SQLSERVER":
                    return SqlServerDataLoader.CreateDataLoader(settings.ConnectionString, flFileInfo, ThreadCount, threadPriority);
            }
            return null;
        }

        public static DataLoader CreateDataLoader(DbTypes DatabaseType, string ConnectionString, DataFileInfo flFileInfo, int ThreadCount, ThreadPriority threadPriority)
        {
            switch (DatabaseType)
            {
                case DbTypes.Oracle:
                    return OracleDataLoader.CreateDataLoader(ConnectionString, flFileInfo, ThreadCount, threadPriority, 0);

                case DbTypes.SqlServer:
                    return SqlServerDataLoader.CreateDataLoader(ConnectionString, flFileInfo, ThreadCount, threadPriority);
            }
            return null;
        }

        public static DataLoader CreateDataLoader(string ConnectionStringName, DataFileInfo flFileInfo, int ThreadCount, ThreadPriority threadPriority, int CommitAfter)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            switch (settings.ProviderName.ToUpper())
            {
                case "ORACLE":
                    return OracleDataLoader.CreateDataLoader(settings.ConnectionString, flFileInfo, ThreadCount, threadPriority, CommitAfter);

                case "SQLSERVER":
                    return SqlServerDataLoader.CreateDataLoader(settings.ConnectionString, flFileInfo, ThreadCount, threadPriority);
            }
            return null;
        }

        public static DataLoader CreateDataLoader(DbTypes DatabaseType, string ConnectionString, DataFileInfo flFileInfo, int ThreadCount, ThreadPriority threadPriority, int CommitAfter)
        {
            switch (DatabaseType)
            {
                case DbTypes.Oracle:
                    return OracleDataLoader.CreateDataLoader(ConnectionString, flFileInfo, ThreadCount, threadPriority, CommitAfter);

                case DbTypes.SqlServer:
                    return SqlServerDataLoader.CreateDataLoader(ConnectionString, flFileInfo, ThreadCount, threadPriority);
            }
            return null;
        }

    }
}
