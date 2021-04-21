using CoordinatesViewerServer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace CoordinatesViewerServer.Logic
{
    /// <summary>
    /// Координаты указателя мыши (Logic)
    /// </summary>
    public class CoordinatesLogic
    {
        private string ConnectionStringDb = ConfigurationManager.ConnectionStrings["MouseDb"].ConnectionString;

        /// <summary>
        /// Удаление координат мыши из Базы Данных (Logic)
        /// </summary>
        /// <returns></returns>
        public async Task DeleteCoordinatesDb()
        {
            using (var sqlConnection = new SqlConnection(ConnectionStringDb))
            {
                await sqlConnection.OpenAsync();

                SqlCommand command = new SqlCommand("DELETE FROM [Mouse]", sqlConnection);

                await command.ExecuteNonQueryAsync();
            } 
        }

        /// <summary>
        /// Получение всех координат мыши (Logic)
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllCoordinatesDb()
        {
            using (var sqlConnection = new SqlConnection(ConnectionStringDb))
            {
                await sqlConnection.OpenAsync();

                SqlCommand command = new SqlCommand("SELECT * FROM [Mouse]", sqlConnection);

                return await GetCoordinatesData(command);
            }
        }

        /// <summary>
        /// Фильтр отображения координат мыши по дате/времени (Logic)
        /// </summary>
        /// <param name="dateTimeFrom"></param>
        /// <param name="dateTimeTo"></param>
        /// <returns></returns>
        public async Task<List<string>> GetCoordinatesDb(DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            using (var sqlConnection = new SqlConnection(ConnectionStringDb))
            {
                await sqlConnection.OpenAsync();

                SqlCommand command = new SqlCommand("SELECT * FROM [Mouse] " +
                                                "WHERE [date_time] >= @timeFrom " +
                                                "AND [date_time] < @timeTo", sqlConnection);

                command.Parameters.AddWithValue("timeFrom", dateTimeFrom);
                command.Parameters.AddWithValue("timeTo", dateTimeTo);

                return await GetCoordinatesData(command);
            }
        }

        /// <summary>
        /// Фильтр отображения координат мыши по событию (Logic)
        /// </summary>
        /// <param name="eventMouse"></param>
        /// <returns></returns>
        public async Task<List<string>> GetCoordinatesDb(string eventMouse)
        {
            using (var sqlConnection = new SqlConnection(ConnectionStringDb))
            {
                await sqlConnection.OpenAsync();

                SqlCommand command = new SqlCommand("SELECT * FROM [Mouse] WHERE event = @selector", sqlConnection);

                command.Parameters.AddWithValue("selector", eventMouse);

                return await GetCoordinatesData(command);
            }
        }

        /// <summary>
        /// Добавление координат мыши в Базу Данных (Logic)
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public async Task InsertCoordinatesDb(Coordinate coordinate)
        {
            using (var sqlConnection = new SqlConnection(ConnectionStringDb))
            {
                await sqlConnection.OpenAsync();

                SqlCommand cmd = new SqlCommand("INSERT INTO [Mouse] (X, Y, event, date_time) " +
                                                    "VALUES (@xx, @yy, @event, @dateTime)", sqlConnection);

                cmd.Parameters.AddWithValue("xx", coordinate.X);

                cmd.Parameters.AddWithValue("yy", coordinate.Y);

                cmd.Parameters.AddWithValue("event", coordinate.EventMouse);

                cmd.Parameters.AddWithValue("dateTime", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        #region helperMethods

        private async Task<List<string>> GetCoordinatesData(SqlCommand command)
        {
            try
            {
                List<string> coordinatesList = new List<string>();

                using (SqlDataReader sqlReader = await command.ExecuteReaderAsync())
                {
                    while (await sqlReader.ReadAsync())
                    {
                        coordinatesList.Add(GetLogItem(sqlReader));
                    }
                }
                return coordinatesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetLogItem(SqlDataReader sqlReader)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(sqlReader["Id"].ToString().PadRight(10));

            sb.Append(sqlReader["X"].ToString().PadRight(12));

            sb.Append(sqlReader["Y"].ToString().PadRight(12));

            sb.Append(sqlReader["event"].ToString().PadRight(12));

            sb.Append(sqlReader["date_time"].ToString().PadRight(10));

            return sb.ToString();
        }

        #endregion
    }
}
