using System;
using System.Net.Http;
using System.Text;

public class CPHInline
{
    private static readonly HttpClient httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    public bool Execute()
    {
        string lightIp = "192.168.68.53";
        string lightUrl = $"http://{lightIp}:9123/elgato/lights";

        string settingsJson =
            "{\"numberOfLights\":1,\"lights\":[{\"on\":1,\"brightness\":20,\"temperature\":244}]}";

        try
        {
            CPH.LogInfo("[Flashbang] Setting baseline light settings.");

            using (var content = new StringContent(
                settingsJson,
                Encoding.UTF8,
                "application/json"))
            {
                using (var response = httpClient
                    .PutAsync(lightUrl, content)
                    .GetAwaiter()
                    .GetResult())
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            string currentSettings = httpClient
                .GetStringAsync(lightUrl)
                .GetAwaiter()
                .GetResult();

            CPH.LogInfo($"[Flashbang] Settings after update: {currentSettings}");

            return true;
        }
        catch (Exception error)
        {
            CPH.LogError($"[Flashbang] Baseline test failed: {error.Message}");

            return false;
        }
    }
}