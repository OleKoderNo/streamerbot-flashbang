# Streamer.bot Flashbang

A custom C# action for Streamer.bot that synchronises an OBS flashbang animation and sound with an Elgato Key Light.

## Quick setup

To install and configure the finished action, follow [SETUP.md](SETUP.md).

This README also records the development process, problems encountered,
and verification of each feature.

## Behaviour

1. Play the flashbang animation and sound in OBS.
2. Hide the animation when the flashbang explodes.
3. Increase the Key Light brightness to 100%.
4. Wait three seconds.
5. Restore the light to 20% brightness and approximately 4100 K.

## Requirements

- Streamer.bot
- OBS Studio
- An Elgato Key Light connected to the same local network as the computer
- Elgato Control Center for initial light setup
- A flashbang animation with sound

## Development setup

1. Create an action in Streamer.bot named `Flashbang - Development`.
2. Add a `Core > C# > Execute C# Code` sub-action.
3. Copy the contents of `Flashbang.cs` into the code editor.
4. Compile and save the code.
5. Add a `Core > Test` trigger.
6. Right-click the trigger and select `Test Trigger`.

Edit the code in VS Code, then copy the updated code into Streamer.bot to compile and test it.

## Current progress

The C# action combines an OBS animation and sound with an Elgato
Key Light flash.

1. Starts the Flashbang media source in Redeem Effects.
2. Waits 1250 milliseconds.
3. Hides the picture using the Hide Visual opacity filter while
   allowing the sound to continue.
4. Sets the light to 100% brightness.
5. Waits three seconds.
6. Restores 20% brightness and resets the OBS source and filter.

Both light settings use approximately 4100 K.

The combined sequence has been manually tested successfully.
The channel point reward is connected and has been tested through
a real Twitch redemption.

## Find the light's IP address

The recommended method is to use Elgato Control Center on iOS or
Android.

1. Install Elgato Control Center from the App Store or Google Play.
2. Connect your phone to the same local network as the Key Light.
3. Open Control Center and locate your light.
4. Open the light's settings and note its IP address.

During this setup, the IP address was found using the mobile app
because it was not displayed in the desktop app.

If a mobile device is unavailable, Elgato also suggests using a
network scanner, such as Wireless Network Watcher.

### Verify connectivity

On your computer, open this address in a browser:

`http://YOUR_LIGHT_IP:9123/elgato/lights`

Replace `YOUR_LIGHT_IP` with your light's IP address.

A successful request returns JSON containing the light's settings.
This request only reads the settings; it does not change them.

## Development notes

### Step 1: Verify C# execution

Created a development action with an Execute C# Code sub-action.
Confirmed that running the Test trigger writes the expected log message.

### Step 2: Check the light's local API

Before implementing HTTP requests in C#, test this address in a browser:

`http://YOUR_LIGHT_IP:9123/elgato/lights`

Replace `YOUR_LIGHT_IP` with the light's local IP address.

- `9123` is the API port.
- `/elgato/lights` is the endpoint for the light's settings.
- A GET request reads the settings without changing them.
- The expected response is JSON containing the light's power state,
  brightness, and colour temperature value.

Status: Verified.

The browser returned settings for one light:

- Power: off (`on: 0`).
- Brightness: 20%.
- Temperature: 244, approximately 4100 K.

This confirms that the computer can reach the light's local API.
Reading the settings works even when the light's illumination is off.

### Step 3: Read light settings from C#

Added an HTTP GET request to the light's `/elgato/lights` endpoint.

The action reuses an HttpClient with a five-second timeout and logs
the response. If the request fails, it logs an error and returns false.

This step only reads the settings. It does not change the light.

Status: Verified.

Compiled successfully and ran the action using a Core > Test trigger.
The HTTP GET request returned settings for one light:
power off, brightness 20%, and temperature 244 (approximately 4100 K).

### Compilation issue: Missing System.dll reference

The first compilation failed with error CS0012 because System.dll
was missing from the code action's references.

Fixed in Streamer.bot:

1. Open the Execute C# Code sub-action.
2. Select the References tab.
3. Right-click the references list.
4. Select Add reference from file.
5. Add `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.dll`.
6. Compile again and save.

If that path is unavailable, check:
`C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll`.

### Viewing test results

1. Open the folder containing Streamer.bot.exe.
2. Open the logs folder.
3. Open the most recently modified log file.
4. Search for `[Flashbang]`.

### Step 4: Apply baseline light settings

Added an HTTP PUT request to set the light to:

- Power: on.
- Brightness: 20%.
- Temperature: 244, approximately 4100 K.

The action checks the HTTP response for success, then sends a GET
request and logs the returned settings for verification.

Status: Verified.

The code compiled successfully. Running the Test trigger turned the
light on at 20% brightness and approximately 4100 K. The follow-up
GET request confirmed the expected settings.

### Step 5: Add the timed flash

Moved the light update request into a reusable SetLight method.

The action now:

1. Turns the light on at 100% brightness and approximately 4100 K.
2. Waits 3000 milliseconds after the request succeeds.
3. Restores 20% brightness and approximately 4100 K.

A finally block attempts restoration even if the flash request fails.
Restoration depends on the light remaining reachable.

A lock prevents overlapping executions of this code's flash sequence.
Network delays can make the physical flash slightly longer than
three seconds.

Status: Verified.

The code compiled successfully. Running the Test trigger increased
the brightness to 100% for approximately three seconds, then
restored it to 20%.

### Step 6: Integrate OBS animation and sound

Added OBS control using:

- Scene: Redeem Effects.
- Media source: Flashbang.
- Color Correction filter: Hide Visual, with opacity set to 0.

The action starts the media, waits 1250 milliseconds, enables the
opacity filter, and triggers the three-second light flash.
The filter hides the picture while the media audio continues.

Cleanup attempts to restore the light, hide the media source,
and disable the opacity filter for the next run.

The action stops before starting if OBS is disconnected.
The explosion delay may need adjustment for playback and network
latency.

Status: Verified.

The code compiled successfully. A manual test confirmed that the
animation and sound played, the picture disappeared, and the light
flashed before returning to 20% brightness.

The sequence worked with an explosion delay of 1250 milliseconds.

### Step 7: Connect the channel point reward

1. Select the Flashbang - Development action in Streamer.bot.
2. Right-click inside the Triggers panel.
3. Select Twitch > Channel Reward > Reward Redemption.
4. Select the specific flashbang reward and save.

Keep the Core > Test trigger for manual testing.

OBS and Streamer.bot must be running, with Streamer.bot connected
to OBS and the Twitch broadcaster account.

Status: Verified.

Redeemed the reward through my own Twitch channel and observed
the full animation, sound, and light sequence. One redemption
triggered one sequence, and the light returned to 20% brightness.
