using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Infrastructure.Services
{
    public class SikayetLoglariService:ISikayetLoglariService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SikayetLoglariService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region SikayetLogla
        public async Task LogAsync(SikayetLog sikayetLog)
        {

            await _unitOfWork.Repository<SikayetLog>().Add(sikayetLog);
            await _unitOfWork.CommitAsync();
        }
        #endregion 


        #region SikayetLoglarınıZamanaGöreSil
        public async Task DeleteLogsOlderThan(DateTime date)
        {
            var oldLogs = _unitOfWork.Repository<SikayetLog>().GetAll().Result.Where(log => log.Tarih < date);
            await _unitOfWork.Repository<SikayetLog>().RemoveRange(oldLogs);
            await _unitOfWork.CommitAsync();
        }

        #endregion 

    }
}
