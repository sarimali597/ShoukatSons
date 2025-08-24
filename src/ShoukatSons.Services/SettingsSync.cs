using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ShoukatSons.Core.Helpers;

namespace ShoukatSons.Services
{
    public static class SettingsSync
    {
        private static string GetEndpoint()
        {
            return ConfigurationManager.AppSettings["CloudSyncEndpoint"] ?? "";
        }

        public static async Task PushAsync(string filePath)
        {
            try
            {
                var endpoint = GetEndpoint();
                if (string.IsNullOrWhiteSpace(endpoint)) return;
                if (!File.Exists(filePath)) return;

                using var client = new HttpClient();
                using var content = new MultipartFormDataContent();
                await using var fs = File.OpenRead(filePath);
                var fileContent = new StreamContent(fs);
                content.Add(fileContent, "file", Path.GetFileName(filePath));
                var resp = await client.PostAsync(endpoint, content);
                resp.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
    }
}