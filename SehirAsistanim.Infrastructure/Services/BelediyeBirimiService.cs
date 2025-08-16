using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Infrastructure.Services
{
    public class BelediyeBirimiService : IBelediyeBirimiService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BelediyeBirimiService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region GetAll
        public async Task<List<BelediyeBirimi>> GetAll()
        {
            var result = await _unitOfWork.Repository<BelediyeBirimi>().GetAll();
            return result.ToList();
        }
        #endregion

        #region GetById
        public async Task<BelediyeBirimi> GetById(int belediyeBirimiId)
        {
            return await _unitOfWork.Repository<BelediyeBirimi>().GetById(belediyeBirimiId);
        }
        #endregion

        #region AddBelediyeBirimi
        public async Task<BelediyeBirimi> AddBelediyeBirimi(BelediyeBirimi belediyeBirimi)
        {
            await _unitOfWork.Repository<BelediyeBirimi>().Add(belediyeBirimi);
            await _unitOfWork.CommitAsync();
            return belediyeBirimi;
        }
        #endregion

        #region UpdateBelediyeBirimi
        public async Task<BelediyeBirimi> UpdateBelediyeBirimi(BelediyeBirimi belediyeBirimi)
        {
            _unitOfWork.Repository<BelediyeBirimi>().Update(belediyeBirimi);
            await _unitOfWork.CommitAsync();
            return belediyeBirimi;
        }
        #endregion

        #region DeleteBelediyeBirimi
        public async Task<bool> DeleteBelediyeBirimi(int belediyeBirimiId)
        {
            var belediyeBirimi = await _unitOfWork.Repository<BelediyeBirimi>().GetById(belediyeBirimiId);
            if (belediyeBirimi == null)
                return false;

            _unitOfWork.Repository<BelediyeBirimi>().Delete(belediyeBirimi.Id);
            await _unitOfWork.CommitAsync();
            return true;
        }
        #endregion

        #region SoftDelete
        //public async Task<bool> SoftDeleteBelediyeBirimi(int belediyeBirimiId)
        //{
        //    var belediyeBirimi = await _unitOfWork.Repository<BelediyeBirimi>().GetById(belediyeBirimiId);
        //    if (belediyeBirimi == null)
        //        return false;

        //    belediyeBirimi.SilindiMi = true;

        //    _unitOfWork.Repository<BelediyeBirimi>().Update(belediyeBirimi.Id);
        //    await _unitOfWork.Commit();
        //    return true;
        //}
        #endregion
    }
}
