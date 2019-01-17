﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Application.Interfaces;

namespace Cardbooru.Application
{
    public class SystemHttpClient : IBooruHttpClient
    {
        private HttpClient _client;

        public SystemHttpClient()
        {
            _client = new HttpClient();
        }
        public Task<string> GetStringAsync(string url)
        {
            return _client.GetStringAsync(url);
        }

        public async Task<byte[]> GetByteArrayAsync(string url, 
            CancellationToken cancellationToken = default)
        {
            byte[] result = { };
            try
            {
                result = await _client.GetByteArrayAsync(url);
            }
            catch (ArgumentNullException e)
            {
                return result;
            }
            catch (Exception e)
            {
                if (e.Message !=
                    "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set."
                )
                    throw;
            }


            return result;
        }

    }
}
