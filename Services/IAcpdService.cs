using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAcpdService
    {
        Task<List<MyOfficeAcpd>> GetAllAsync();
        Task<MyOfficeAcpd?> GetByIdAsync(string id);
        Task<(MyOfficeAcpd Entity, string? LogJson)> CreateAsync(MyOfficeAcpd entity);
        Task UpdateAsync(string id, MyOfficeAcpd entity);
        Task DeleteAsync(string id);
    }
}
