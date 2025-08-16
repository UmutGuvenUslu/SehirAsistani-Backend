using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface IDuyguAnaliz
    {
        Task<int> HesaplaDuyguPuani(string metin);
    }
}
