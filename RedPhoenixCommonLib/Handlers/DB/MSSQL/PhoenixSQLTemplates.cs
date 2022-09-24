using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RedPhoenix.Common.Handlers.DB.MSSQL
{
    /// <summary>
    /// Basic templates for sql requests execution
    /// </summary>
    public static class PhoenixSQLTemplates
    {
        #region Constants
        private const int MAX_PARAMS = 2000;
        #endregion

        #region Properties
        public static string ConnectionString { get; set; }
        #endregion

        #region Methods
        #region Templates
        public static void BaseTemplate(
            Action<SqlCommand> commandHandler)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();

                    commandHandler(command);
                }
            }
            catch (Exception) { throw; }
        }

        public static void DefaultWithReader<T_in, T_out>(
            T_in input, T_out output,
            Action<T_in, SqlCommand> commandSetter,
            Action<SqlDataReader, T_out> dataReader)
        {
            BaseTemplate((SqlCommand command) =>
            {
                commandSetter(input, command);

                using (var reader = command.ExecuteReader())
                    while (reader.HasRows && reader.Read())
                        dataReader(reader, output);
            });
        }

        public static void DefaultWithoutReader<T_in>(
            T_in input,
            Action<T_in, SqlCommand> commandSetter)
        {
            BaseTemplate((SqlCommand command) =>
            {
                commandSetter(input, command);

                command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T_in"></typeparam>
        /// <typeparam name="T_out"></typeparam>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="countParameters"></param>
        /// <param name="commandSetter">Parameters: data list, sqlCommand, startIndex, count</param>
        /// <param name="dataReader"></param>
        private static void WithListAndReader<T_in, T_out>(
            List<T_in> input, T_out output, int countParameters,
            Action<List<T_in>, SqlCommand, int, int> commandSetter,
            Action<SqlDataReader, T_out> dataReader)
        {
            BaseTemplate((SqlCommand command) =>
            {
                int step = MAX_PARAMS / countParameters;

                for (int i = 0; i < input.Count; i += step)
                {
                    commandSetter(input, command, i, GetCountByStep(i, step, input.Count));

                    using (var reader = command.ExecuteReader())
                        while (reader.HasRows && reader.Read())
                            dataReader(reader, output);
                }
            });
        }

        private static void WithList<T_in>(
            List<T_in> input, int countParameters,
            Action<List<T_in>, SqlCommand, int, int> commandSetter)
        {
            BaseTemplate((SqlCommand command) =>
            {
                int step = MAX_PARAMS / countParameters;

                for (int i = 0; i < input.Count; i += step)
                {
                    commandSetter(input, command, i, GetCountByStep(i, step, input.Count));

                    command.ExecuteNonQuery();
                }
            });
        }

        private static void WithoutInputData<T_out>(
            T_out output, string request,
            Action<SqlDataReader, T_out> dataReader)
        {
            BaseTemplate((SqlCommand command) =>
            {
                command.CommandText = request;

                using (var reader = command.ExecuteReader())
                    while (reader.HasRows && reader.Read())
                        dataReader(reader, output);
            });
        }
        #endregion

        #region Other methods
        private static int GetCountByStep(int _currentIndex, int _step, int _commonCount)
        {
            return _currentIndex + _step < _commonCount ? _step : _commonCount - _currentIndex;
        }
        #endregion
        #endregion
    }
}
