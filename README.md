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

The initial logging test works. Running the development action writes this message to Streamer.bot's log:

`[Flashbang] Development action started.`

Light control and OBS integration are not implemented yet.
