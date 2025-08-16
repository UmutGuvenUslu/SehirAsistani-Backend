using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Entities;

namespace SehirAsistanim.Domain.Interfaces
{
    public interface IRolService
    {
        Task<IActionResult> AddRol([FromBody] RolTuruRequest request);
    }
}
