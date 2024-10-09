using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using SeliwareAPI; // Reference Seliware.dll

class Program
{
    private static HttpListener _httpListener;
    private static Process robloxProcess = null;

    static void Main(string[] args)
    {
        // Initialize Seliware
        Seliware.Initialize();

        // Inject Roblox process and monitor it
        InjectAndMonitorRoblox();

        // Setup WebSocket server at port 5588
        StartWebSocketServer();

        // Keep the server alive, but quit when Roblox closes
        MonitorRobloxProcess();
    }

    static void InjectAndMonitorRoblox()
    {
        try
        {
            // Get the Roblox process and inject it
            robloxProcess = Process.GetProcessesByName("RobloxPlayerBeta")[0];
            string injectResult = Seliware.Inject(robloxProcess.Id);
            Console.WriteLine("Injection Result: " + injectResult);

            // Check injection status
            if (Seliware.IsInjected())
            {
                Console.WriteLine("Successfully injected!");
            }
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Roblox process not found.");
            Environment.Exit(1); // Exit if Roblox is not running
        }
    }

    static void StartWebSocketServer()
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add("http://localhost:5588/");
        _httpListener.Start();
        Console.WriteLine("WebSocket server started at ws://localhost:5588");

        // Accept connections in a separate thread
        Thread listenerThread = new Thread(() =>
        {
            while (_httpListener.IsListening)
            {
                var context = _httpListener.GetContext();
                var wsContext = context.AcceptWebSocketAsync(null).Result;
                WebSocket webSocket = wsContext.WebSocket;

                // Handle WebSocket connection
                HandleWebSocketConnection(webSocket);
            }
        });
        listenerThread.Start();
    }

    static async void HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                // Received a script from the WebSocket client
                string script = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Script received: " + script);

                // Execute the script in Roblox using Seliware
                bool executed = Seliware.Execute(script);
                Console.WriteLine("Script executed: " + executed);

                // Send a response back to the client
                string responseMessage = executed ? "Script executed successfully." : "Failed to execute script.";
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    static void MonitorRobloxProcess()
    {
        while (true)
        {
            // Check if Roblox process has exited
            if (robloxProcess.HasExited)
            {
                Console.WriteLine("Roblox has closed. Exiting...");
                Environment.Exit(0); // Gracefully exit the application
            }

            // Sleep for a while to avoid high CPU usage
            Thread.Sleep(1000);
        }
    }
}
