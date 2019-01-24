using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Application.Interfaces;

namespace Cardbooru.Application
{
    public class SystemHttpClient : IBooruHttpClient
    {
        private readonly HttpClient _client;

        public SystemHttpClient()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<byte[]> GetByteArrayAsync(string url, 
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = null;
            try
            {
                response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            }
            catch (ArgumentNullException e)
            {
                return default;
            }
            catch (HttpRequestException e)
            {
                if (e.Message != "Response status code does not indicate success: 404 (Not Found).") throw ;
            }
            catch (Exception e)
            {
                if (e.Message !=
                    "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set."
                )
                    throw;
            }

            if(response == null) return default;
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

    }
}
