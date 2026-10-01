# Streamer.bot Flashbang

A custom C# action for Streamer.bot that synchronises an OBS flashbang animation and sound with an Elgato Key Light.

## Planned behaviour

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

The C# action can read and update the Key Light's settings through
its local HTTP API.

The baseline test turns the light on at 20% brightness and
approximately 4100 K, then reads and logs the settings.

Requests use a five-second timeout, and failures are logged.

The timed flash and OBS integration are not implemented yet.

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
