using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace JASShortCircuitExample
{
    public class DataAccessLayer : IDataAccessLayer
    {
        public async Task<bool> SqlPostAsync(string procName, string conStr, string jsonBody)
        {
            using (var sqlCon = new SqlConnection(conStr))
            using (var sqlCmd = new SqlCommand(procName, sqlCon))
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;

                var b = sqlCmd.Parameters.Add("@body", SqlDbType.NVarChar, -1);
                b.Direction = ParameterDirection.Input;
                b.Value = jsonBody;

                var id = sqlCmd.Parameters.Add("@id", SqlDbType.BigInt);
                id.Direction = ParameterDirection.Output;
                var r = sqlCmd.Parameters.Add("@response", SqlDbType.NVarChar, -1);
                r.Direction = ParameterDirection.Output;

                await sqlCon.OpenAsync();
                await sqlCmd.ExecuteNonQueryAsync();
                sqlCon.Close();

                var dbInt = (SqlInt64)id.SqlValue;
                var clrBool = dbInt.IsNull ? false : (bool)dbInt.ToSqlBoolean();
                return clrBool;
            }
        }
    }
}
