using HADotNet.CommandCenter.Models.Config;
using System;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Services.Interfaces
{
    public interface IConfigStore
    {
        Task ManipulateConfig(params Action<ConfigRoot>[] changes);
        Task<ConfigRoot> GetConfigAsync();
        Task SaveConfigAsync(ConfigRoot config);
    }
}
