using System;
using System.Net.Http;

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

        try
        {
            CPH.LogInfo("[Flashbang] Reading light settings.");

            string response = httpClient
                .GetStringAsync(lightUrl)
                .GetAwaiter()
                .GetResult();

            CPH.LogInfo($"[Flashbang] Light settings: {response}");

            return true;
        }
        catch (Exception error)
        {
            CPH.LogError($"[Flashbang] Could not read light: {error.Message}");

            return false;
        }
    }
}