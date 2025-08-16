using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistani.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BelediyeBirimiController : ControllerBase
    {
        private readonly IBelediyeBirimiService _service;

        public BelediyeBirimiController(IBelediyeBirimiService service)
        {
            _service = service;
        }

        #region GetAll
        [HttpGet]
        public async Task<List<BelediyeBirimi>> GetAll()
        {
            try
            {
                return await _service.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAll hata: {ex.Message}");
                return new List<BelediyeBirimi>();
            }
        }
        #endregion

        #region GetById
        [HttpGet("{id}")]
        public async Task<BelediyeBirimi?> GetById(int id)
        {
            try
            {
                return await _service.GetById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetById hata: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Add
        [HttpPost]
        public async Task<BelediyeBirimi?> Add([FromBody] BelediyeBirimi belediyeBirimi)
        {
            try
            {
                return await _service.AddBelediyeBirimi(belediyeBirimi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add hata: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Update
        [HttpPut]
        public async Task<BelediyeBirimi?> Update([FromBody] BelediyeBirimi belediyeBirimi)
        {
            try
            {
                return await _service.UpdateBelediyeBirimi(belediyeBirimi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update hata: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            try
            {
                return await _service.DeleteBelediyeBirimi(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete hata: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
