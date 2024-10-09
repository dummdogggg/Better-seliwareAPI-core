
# better-seliwareapi-core

## Overview

This project is designed to make the **SeliwareAPI** more extensible across platforms, enabling compatibility with environments like 32-bit Python, Node.js, and others. It sets up a WebSocket server that allows you to send scripts for execution in Roblox, simplifying integration across various platforms.

## How It Works

- **WebSocket Server**: The project creates a WebSocket server at `ws://localhost:5588`.
- **Injection**: Uses the normal SeliwareAPI injection method to inject into the Roblox process.
- **Script Execution**: You can send scripts to the WebSocket server to be executed in Roblox.

## How to Use

1. **Download the Release**: Get the latest release from the [GitHub Releases](https://github.com/your-repo/releases) section.
2. **Run the Executable**: Extract the ZIP file and run the `SeliwareApp.exe` file. This will start the application in the background and set up the WebSocket server.
3. **Inject and Execute Scripts**: 
   - **Injection**: The application automatically injects into Roblox when it’s running.
   - **Script Execution**: Send the script you want to execute to the WebSocket server running on port `5588`.

## Examples

### Example: Node.js

```javascript
const WebSocket = require('ws');
const { exec } = require('child_process');

// Start the executable from the bin folder
exec('./bin/SeliwareApp.exe');

const ws = new WebSocket('ws://localhost:5588');

ws.on('open', function open() {
  ws.send('print("Hello from Node.js!")');
});

ws.on('message', function incoming(data) {
  console.log(data.toString());  // Logs success/failure message
});
```

### Example: Python

```python
import asyncio
import websockets
import subprocess

# Start the executable from the bin folder
subprocess.Popen(['./bin/SeliwareApp.exe'])

async def send_script():
    uri = "ws://localhost:5588"
    async with websockets.connect(uri) as websocket:
        await websocket.send('print("Hello from Python!")')
        response = await websocket.recv()
        print(f"< {response}")

asyncio.run(send_script())
```

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

--- 

### Contact

Dm dumm_dogg on discord or ping me in the seliware server for help.
