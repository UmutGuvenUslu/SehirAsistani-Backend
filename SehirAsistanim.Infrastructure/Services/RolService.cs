using Microsoft.AspNetCore.Mvc;
using SehirAsistanim.Domain.Entities;
using SehirAsistanim.Domain.Interfaces;

public class RolService : IRolService
{
    private readonly IUnitOfWork _unitOfWork;

    public RolService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> AddRol(RolTuruRequest request)
    {
        var yeniRol = new Rol
        {
            Tur = request.Rol
        };

        _unitOfWork.Repository<Rol>().Add(yeniRol);
        await _unitOfWork.CommitAsync();

        return new OkObjectResult(new { message = "Rol eklendi", rol = yeniRol.Tur });
    }
}
