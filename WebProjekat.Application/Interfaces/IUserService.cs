using System.Threading.Tasks;
using WebProjekat.Application.DTOs;

namespace WebProjekat.Application.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO> AuthenticateAsync(LoginDTO dto);
    }
}