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

        string scene = "Redeem Effects";
        string source = "Flashbang";
        string filter = "Hide Visual";

        int explosionDelayMs = 1250;

        lock (flashLock)
        {
            if (!CPH.ObsIsConnected())
            {
                CPH.LogError("[Flashbang] OBS is not connected.");
                return false;
            }

            try
            {
                try
                {
                    // Prepare and start the animation.
                    CPH.ObsSetSourceVisibility(scene, source, false);
                    CPH.ObsSetFilterState(scene, source, filter, 1);
                    CPH.ObsSetSourceVisibility(scene, source, true);
                    CPH.ObsMediaRestart(scene, source);

                    CPH.LogInfo("[Flashbang] Animation started.");

                    CPH.Wait(explosionDelayMs);

                    // Hide the picture while allowing its audio to continue.
                    CPH.ObsSetFilterState(scene, source, filter, 0);

                    SetLight(lightUrl, 100);
                    CPH.LogInfo("[Flashbang] Brightness set to 100%.");

                    CPH.Wait(3000);
                }
                finally
                {
                    try
                    {
                        SetLight(lightUrl, 20);
                        CPH.LogInfo("[Flashbang] Baseline restored to 20%.");
                    }
                    finally
                    {
                        CPH.ObsSetSourceVisibility(scene, source, false);
                        CPH.ObsSetFilterState(scene, source, filter, 1);
                    }
                }

                return true;
            }
            catch (Exception error)
            {
                CPH.LogError($"[Flashbang] Sequence failed: {error.Message}");
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