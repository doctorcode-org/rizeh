using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Parsnet
{
    public class DataProvider
    {
        private static SqlConnection InitConnection()
        {
            //String CnnString = "Data Source=PARSNET-PC\\SQLEXPRESS;Initial Catalog=Rizeh;Integrated Security=True";
            String CnnString = "Data Source=NonamaServer;Initial Catalog=Rizeh;Integrated Security=True";
            return new SqlConnection(CnnString);
        }

        private static int RunCommand(SqlCommand cmd)
        {
            using (SqlConnection Cnn = InitConnection())
            {
                try
                {
                    Cnn.Open();
                    cmd.Connection = Cnn;
                    SqlParameter returnValue = new SqlParameter("@RETURN_VALUE", -1);
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);
                    cmd.ExecuteNonQuery();
                    return (int)returnValue.Value;
                }
                catch (Exception ex)
                {
                    Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
                    return -1;
                }
            }
        }

        private static int RunCommand(String commandName)
        {
            SqlCommand cmd = new SqlCommand(commandName);
            cmd.CommandType = CommandType.StoredProcedure;
            return RunCommand(cmd);
        }

        public static bool Run(List<SqlParameter> param, string comandName)
        {
            SqlCommand cmd = new SqlCommand(comandName);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter par in param)
            {
                if (par.Value == null)
                    par.Value = DBNull.Value;
                cmd.Parameters.Add(par);
            }
            return (RunCommand(cmd) > 0);
        }

        public static void Run(List<SqlParameter> param, string comandName, out int returnValue)
        {
            SqlCommand cmd = new SqlCommand(comandName);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter par in param)
            {
                if (par.Value == null)
                    par.Value = DBNull.Value;
                cmd.Parameters.Add(par);
            }
            returnValue = RunCommand(cmd);
        }

        public static void Run(string comandName, out int returnValue)
        {
            SqlCommand cmd = new SqlCommand(comandName);
            cmd.CommandType = CommandType.StoredProcedure;
            returnValue = RunCommand(cmd);
        }

        public static bool Run(SqlParameter param, string comandName)
        {
            SqlCommand cmd = new SqlCommand(comandName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(param);
            return (RunCommand(cmd) > 0);
        }

        public static void Run(SqlParameter param, string comandName, out int returnValue)
        {
            SqlCommand cmd = new SqlCommand(comandName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(param);
            returnValue = RunCommand(cmd);
        }


        private static DataTable ExportCommand(SqlCommand cmd)
        {
            DataTable table = new DataTable();

            try
            {
                using (SqlConnection Cnn = InitConnection())
                {
                    Cnn.Open();
                    cmd.Connection = Cnn;
                    var Adapter = new SqlDataAdapter(cmd);
                    Adapter.Fill(table);
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
            }

            return table;

        }

        public static DataTable Export(List<SqlParameter> param, string comandName, out int runResult)
        {
            runResult = -1;
            DataTable table = new DataTable();

            try
            {
                SqlCommand cmd = new SqlCommand(comandName);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter par in param)
                {
                    if (par.Value == null)
                    {
                        par.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(par);
                }

                SqlParameter returnValue = new SqlParameter("@RETURN_VALUE", -1);
                returnValue.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(returnValue);

                using (SqlConnection Cnn = InitConnection())
                {
                    Cnn.Open();
                    cmd.Connection = Cnn;
                    SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
                    Adapter.Fill(table);
                    runResult = (int)returnValue.Value;
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
            }
            

            return table;
        }

        public static DataTable Export(List<SqlParameter> param, string comandName)
        {
            SqlCommand cmd = new SqlCommand(comandName);
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter par in param)
            {
                if (par.Value == null)
                {
                    par.Value = DBNull.Value;
                }
                cmd.Parameters.Add(par);
            }

            return ExportCommand(cmd);
        }

        public static DataTable Export(SqlParameter param, string comandName)
        {
            SqlCommand cmd = new SqlCommand(comandName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(param);
            return ExportCommand(cmd);
        }

        public static DataTable Export(string comandName)
        {
            SqlCommand cmd = new SqlCommand(comandName);
            cmd.CommandType = CommandType.StoredProcedure;
            return ExportCommand(cmd);
        }

        public static DataTable Export(SqlCommand command)
        {
            return ExportCommand(command);
        }

        //___________________________________________________________________________________________________________________________________

        public static object GetValue(string commandName)
        {
            return GetValue(new SqlCommand(commandName));
        }

        public static object GetValue(SqlParameter parameter, string commandName)
        {
            SqlCommand cmd = new SqlCommand(commandName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(parameter);
            return GetValue(cmd);
        }

        public static object GetValue(List<SqlParameter> parameter, string commandName)
        {
            SqlCommand cmd = new SqlCommand(commandName);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (var param in parameter)
            {
                if (param.Value == null)
                {
                    param.Value = DBNull.Value;
                }
                cmd.Parameters.Add(param);
            }

            return GetValue(cmd);

        }

        public static object GetValue(SqlCommand command)
        {
            object result = 0;
            using (SqlConnection Cnn = InitConnection())
            {
                try
                {
                    Cnn.Open();
                    command.Connection = Cnn;
                    result = command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
                }
            }
            return result;
        }


        /// <summary>
        /// Execute multipe SqlCommand with TransAction
        /// </summary>
        /// <param name="commands">List of SqlCommand to execute</param>
        /// <returns>Return the result of TransAction</returns>
        public static bool ExecuteBatch(List<SqlCommand> commands)
        {
            bool result = false;
            SqlTransaction Trans;
            try
            {
                using (SqlConnection Cnn = InitConnection())
                {
                    Cnn.Open();
                    Trans = Cnn.BeginTransaction();

                    try
                    {
                        foreach (var command in commands)
                        {
                            command.Connection = Cnn;
                            command.Transaction = Trans;
                            command.ExecuteNonQuery();
                        }
                        Trans.Commit();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Trans.Rollback();
                        Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
                    }
                }
            }
            catch (Exception exp)
            {
                Log.WriteError(MethodInfo.GetCurrentMethod(), exp);
            }
           
            return result;
        }
    }
}
