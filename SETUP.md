# Flashbang setup guide

Set up one Elgato Key Light with an OBS animation and sound,
triggered by a Twitch channel point reward.

## 1. Prepare the requirements

You need:

- Streamer.bot connected to your Twitch broadcaster account.
- OBS Studio.
- One Elgato Key Light configured through Elgato Control Center.
- The computer and light connected to the same local network.
- Flashbang.cs from this project.
- A local animation file containing the flashbang sound.

Provide your own animation file with sound.
Adjust explosionDelayMs in the code to match the explosion
in your chosen animation.

## 2. Find the light's IP address

1. Install Elgato Control Center on iOS or Android.
2. Connect your phone to the same network as the light.
3. Open the light's settings in the app.
4. Note its IP address.
5. On your computer, open this address in a browser:

   http://YOUR_LIGHT_IP:9123/elgato/lights

Replace YOUR_LIGHT_IP with the address you found.
You should see JSON containing the light's settings.

## 3. Connect Streamer.bot to OBS

1. In OBS, open Tools > WebSocket Server Settings.
2. Enable the WebSocket server and apply the settings.
3. In Streamer.bot, open Stream Apps > OBS.
4. Add or edit an OBS connection using the matching port and password.
5. For OBS on the same computer, use host 127.0.0.1.
6. Confirm the connection says Connected.

The code uses the first OBS connection in Streamer.bot.

## 4. Create the OBS effect

Use these exact names, including spaces and capitalisation.

1. Create an OBS scene named Redeem Effects.
2. Inside it, add a Media Source named Flashbang.
3. Select your local animation file.
4. Configure its properties:
   - Loop: off.
   - Restart playback when source becomes active: on.
   - Show nothing when playback ends: on.
   - Close file when inactive: off.

5. Right-click the Flashbang media source and select
   Transform > Fit to Screen.

Keep the media source fitted to the Redeem Effects scene.
Resize the nested scene when adding it to your streaming scenes.

### Add the picture-hiding filter

1. Right-click Flashbang and open Filters.
2. Under Effect Filters, add Color Correction.
3. Name the filter Hide Visual.
4. Set its opacity to 0.
5. Disable the filter using its eye icon.
6. Close Filters and hide the Flashbang source.

The code enables this filter at the explosion to hide the picture
while the sound continues.

### Configure sound

1. Open Advanced Audio Properties in OBS.
2. Make sure Flashbang is unmuted and assigned to your stream's
   audio track.
3. To hear the effect yourself, select Monitor and Output.
4. In Settings > Audio > Advanced, select your headphones as the
   Monitoring Device.

Check a short recording for doubled audio if Desktop Audio also
captures your monitoring device.

## 5. Add the effect to your scenes

For every scene where the effect should appear:

1. In Sources, click + and select Scene.
2. Select the existing Redeem Effects scene.
3. Move it above your other sources.
4. Keep this nested scene source visible.

Repeat this when adding new streaming scenes.

## 6. Configure the C# action

1. Open Flashbang.cs in your editor.
2. Change the lightIp value to your light's IP address.
3. Save the file.
4. In Streamer.bot, create an action named Flashbang - Development.
5. Add Core > C# > Execute C# Code.
6. Paste the complete contents of Flashbang.cs.
7. Click Find Refs, then Compile.
8. Save the code.

When changing the file later, copy the updated code into
Streamer.bot and compile again.

### If compilation reports missing System.dll

1. Open the code editor's References tab.
2. Right-click the list and select Add reference from file.
3. Add this file:

   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.dll

4. Compile and save again.

If that file is unavailable, check:

C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll

## 7. Test the effect

This test turns the light on at 100% brightness, then returns it
to 20%. Both settings use approximately 4100 K.

1. In OBS, select a scene containing Redeem Effects.
2. In Streamer.bot, select your action.
3. Right-click the Triggers panel and add Core > Test.
4. Right-click the new trigger and select Test Trigger.

Expected result:

1. The animation and sound start.
2. Near the explosion, the picture disappears and the light brightens.
3. After approximately three seconds, the light returns to 20%.

The code starts with explosionDelayMs set to 1250.
Adjust this value if your animation's explosion happens earlier
or later, then copy the code into Streamer.bot and compile again.

To inspect results, open the logs folder beside Streamer.bot.exe,
open the newest log file, and search for [Flashbang].

## 8. Connect the channel point reward

1. Select the action in Streamer.bot.
2. Right-click the Triggers panel.
3. Select Twitch > Channel Reward > Reward Redemption.
4. Select your specific flashbang reward and save.
5. Redeem the reward on Twitch to test the complete setup.

Keep the Test trigger for manual testing.

## 9. Customise brightness and colour temperature

Open Flashbang.cs in your editor.

### Change the baseline brightness

Inside Execute(), find the reset inside the finally block:

```csharp
SetLight(lightUrl, 20);
```

Replace 20 with your preferred brightness percentage.
For example, to restore the light to 35%:

```csharp
SetLight(lightUrl, 35);
```

Update the nearby log message to match:

```csharp
CPH.LogInfo("[Flashbang] Baseline restored to 35%.");
```

The baseline is the brightness applied after the flash.
Changing this value does not immediately change the light.

### Change the flash brightness

Find:

```csharp
SetLight(lightUrl, 100);
```

Replace 100 with your preferred flash brightness and update its
nearby log message to match.

Use brightness values from 0 to 100.

### Change the colour temperature

Inside SetLight(), find:

```csharp
+ ",\"temperature\":244}]}";
```

Replace only 244 with the value for your preferred colour temperature.

The API uses mireds rather than Kelvin:

API value = round(1000000 / Kelvin)

| Colour temperature | API value | Appearance      |
| ------------------ | --------- | --------------- |
| 2900 K             | 345       | Warmer          |
| 3500 K             | 286       | Warm            |
| 4100 K             | 244       | Current setting |
| 5000 K             | 200       | Cooler          |
| 6500 K             | 154       | Cool            |
| 7000 K             | 143       | Coolest         |

These values are rounded, so the resulting Kelvin value is approximate.
The original Elgato Key Light supports approximately 2900 to 7000 K.

This setting applies to both the flash and the baseline because
both use the same SetLight() method.

### Apply your changes

1. Save Flashbang.cs.
2. Copy the complete file into Streamer.bot's Execute C# Code editor.
3. Compile and save.
4. Run the Test trigger.
5. Confirm the flash and restored settings match your preferences.

Editing Flashbang.cs alone does not update the code in Streamer.bot.

## During use

Keep OBS and Streamer.bot running and connected, and keep the
light reachable on the network.

The action finishes with the light on at 20% brightness and
approximately 4100 K, even if it was off before the effect.

Cleanup attempts to restore the baseline after an error.
It cannot guarantee restoration if the light loses its connection
or Streamer.bot closes.

If the light's IP address changes, update lightIp and copy the
updated code into Streamer.bot.
