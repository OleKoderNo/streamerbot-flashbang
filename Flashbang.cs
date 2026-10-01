using System;
using System.Net.Http;
using System.Text;

public class CPHInline
{
    private static readonly HttpClient httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    private static readonly object flashLock = new object();

    public bool Execute()
    {
        string lightIp = "192.168.68.53";
        string lightUrl = $"http://{lightIp}:9123/elgato/lights";

        lock (flashLock)
        {
            try
            {
                try
                {
                    SetLight(lightUrl, 100);
                    CPH.LogInfo("[Flashbang] Brightness set to 100%.");

                    CPH.Wait(3000);
                }
                finally
                {
                    SetLight(lightUrl, 20);
                    CPH.LogInfo("[Flashbang] Baseline restored to 20%.");
                }

                return true;
            }
            catch (Exception error)
            {
                CPH.LogError($"[Flashbang] Flash failed: {error.Message}");

                return false;
            }
        }
    }

    private void SetLight(string lightUrl, int brightness)
    {
        string settingsJson =
            "{\"numberOfLights\":1,\"lights\":[{\"on\":1,\"brightness\":"
            + brightness
            + ",\"temperature\":244}]}";

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
    }
}