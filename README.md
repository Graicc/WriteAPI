# WriteAPI

A tool that provides an API for external programs to modify the location of the spectator camera in Echo VR.

## How It Works

WriteAPI works by directly writing values to Echo's memory using [memory.dll](https://github.com/erfg12/memory.dll/). The offsets for the multi-level pointers used are stored in the `config.json` file.

## Usage

This program does nothing on its own, but instead provides a way for other programs to easily modify the in game camera.

For camera writing to work, Echo must be in "Free Cam" mode, activated by pressing `c` while in the 2d spectator view.

The camera transform consists of a vector for position and a quaternion for rotation.
The position `(0,0,0)` is the middle of the arena, with +Y being up and +Z being towards the orange goal.
The rotation `(0,0,0,1)` points towards the orange goal, in the +Z direction.

In order to update the camera transform, send a POST request to `127.0.0.1:6723` with a json body following the following format:

```
{
	"position": {
		"X":0.0,
		"Y":5.68,
		"Z":29
	},
	"rotation": {
		"X":0.0,
		"Y":-0.98,
		"Z":0.18,
		"W":0.0
	}
}
```

In order to get the camera transform, send a GET request to `127.0.0.1:6723`. The response will be json in the same format as the above.

## Development

Run with `--noupdateconfig` to use a modified local config without fetching one from the internet.

## Sample Project

The repository includes a sample project that makes use of the API to move the camera. It is a .NET Core WPF project that interpolates the camera over a user defined path.

## Disclaimer

The use of this software may violate the End User License Agreement for Echo VR, available [here](http://www.readyatdawn.com/eula-echo-vr/). Per the `LICENSE`, I am not responsible for any consequences of the use of this software.
