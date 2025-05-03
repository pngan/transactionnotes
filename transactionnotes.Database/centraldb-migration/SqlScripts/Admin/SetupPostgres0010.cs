using System.Data;
using DbUp.Engine;

namespace centraldb_migration.SqlScripts.Admin
{
    public class SetupPostgres0010 : IScript
    {
        public string ProvideScript(Func<IDbCommand> commandFactory)
        {
            using var command = commandFactory();
            var username = Worker.MigrationuserUsername;
            var password = Worker.MigrationuserPassword;
            command.CommandText = $"CREATE ROLE schema_owner NOLOGIN;" +
                                  $"CREATE SCHEMA app AUTHORIZATION schema_owner;" +
                                  $"CREATE USER {username} LOGIN PASSWORD '{password}';" +
                                  $"GRANT schema_owner TO {username};";
            command.ExecuteNonQuery();

            return string.Empty;
        }
    }
}
