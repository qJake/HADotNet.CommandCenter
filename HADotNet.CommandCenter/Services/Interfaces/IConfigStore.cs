using HADotNet.CommandCenter.Models.Config;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Services.Interfaces
{
    public interface IConfigStore
    {
        Task<ConfigRoot> GetConfigAsync();
        Task SaveConfigAsync(ConfigRoot config);
    }
}
