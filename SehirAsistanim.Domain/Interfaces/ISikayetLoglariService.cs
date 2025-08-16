using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SehirAsistanim.Domain.Entities;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface ISikayetLoglariService
    {
        Task LogAsync(SikayetLog sikayetLog);
        Task DeleteLogsOlderThan(DateTime date);


    }
}
