using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoordinatesViewerServer.Logic;
using CoordinatesViewerServer.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoordinatesViewerServer.Controllers
{
    /// <summary>
    /// Координаты указателя мыши
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CoordinatesController : ControllerBase
    {
        CoordinatesLogic coordinatesLogic;

        /// <summary>
        /// Координаты указателя мыши
        /// </summary>
        public CoordinatesController()
        {
            coordinatesLogic = new CoordinatesLogic();
        }

        /// <summary>
        /// Удаление координат мыши из Базы Данных
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete")]
        public async Task DeleteCoordinatesAsync()
        {
            await coordinatesLogic.DeleteCoordinatesDb();
        }

        /// <summary>
        /// Получение всех координат мыши
        /// </summary>
        /// <returns></returns>
        [HttpGet("allcoordinates")]
        public async Task<List<string>> GetAllCoordinatesAsync()
        {
            return await coordinatesLogic.GetAllCoordinatesDb();
        }

        /// <summary>
        /// Фильтр отображения координат мыши по дате/времени
        /// </summary>
        /// <param name="dateTimeFrom"></param>
        /// <param name="dateTimeTo"></param>
        /// <returns></returns>
        [HttpGet("coordinatestime")]
        public async Task<List<string>> GetCoordinatesAsync(DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            return await coordinatesLogic.GetCoordinatesDb(dateTimeFrom, dateTimeTo);
        }

        /// <summary>
        /// Фильтр отображения координат мыши по событию
        /// </summary>
        /// <param name="eventMouse"></param>
        /// <returns></returns>
        [HttpGet("coordinatesevent")]
        public async Task<List<string>> GetCoordinatesAsync(string eventMouse)
        {
            return await coordinatesLogic.GetCoordinatesDb(eventMouse);
        }

        /// <summary>
        /// Добавление координат мыши в Базу Данных
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        [HttpPost("insert")]
        public async Task InsertCoordinatesAsync(Coordinate coordinate)
        {
            await coordinatesLogic.InsertCoordinatesDb(coordinate);
        }
    }
}
