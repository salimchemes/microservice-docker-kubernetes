using System;
using System.Threading.Tasks;

namespace VillaSport.MicroService.Proxies
{
    public interface IRestHttpProxy
    {
        T GetWebRequest<T>(Uri uri);
        Task<T> GetWebRequestAsync<T>(Uri uri);
    }
}