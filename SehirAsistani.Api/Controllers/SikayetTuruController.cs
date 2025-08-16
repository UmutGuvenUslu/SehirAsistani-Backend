using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SehirAsistani.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SikayetTuruController : ControllerBase
    {
        private readonly ISikayetTuruService _service;

        public SikayetTuruController(ISikayetTuruService service)
        {
            _service = service;
        }

        #region GetAll
        [HttpGet]
        public async Task<List<SikayetTuru>> GetAll()
        {
            try
            {
                return await _service.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAll hata: {ex.Message}");
                return new List<SikayetTuru>();
            }
        }
        #endregion

        #region GetById
        [HttpGet("{id}")]
        public async Task<SikayetTuru?> GetById(int id)
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
        public async Task<SikayetTuru?> Add([FromBody] SikayetTuru sikayetTuru)
        {
            try
            {
                return await _service.AddSikayetTuru(sikayetTuru);
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
        public async Task<SikayetTuru?> Update([FromBody] SikayetTuru sikayetTuru)
        {
            try
            {
                return await _service.UpdateSikayetTuru(sikayetTuru);
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
                return await _service.DeleteSikayetTuru(id);
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

