using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Entities;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface IBelediyeBirimiService
    {
        Task<List<BelediyeBirimi>> GetAll();
        Task<BelediyeBirimi> GetById(int belediyeBirimiId);
        Task<BelediyeBirimi> AddBelediyeBirimi(BelediyeBirimi belediyeBirimi);
        Task<BelediyeBirimi> UpdateBelediyeBirimi(BelediyeBirimi belediyeBirimiId);
        Task<bool> DeleteBelediyeBirimi(int belediyeBirimiId);
        
    }
}
