using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
//using UnityEditor.VersionControl;
namespace NetworkAPISocket
{
    public class Messaging
    {
        public Messaging()
        {
            ws = new ClientWebSocket();
        }
        public delegate void LogHandler(string message);

        public event LogHandler Log;
        public ClientWebSocket ws;
        public int? id = null;
        public async void sendMessage(String message)
        {
            //var ws = new ClientWebSocket();
            
            Debug.Log("state: " + ws.State);
            if (ws.State == WebSocketState.None)
            {
                Debug.Log("Connecting to Server");
                await ws.ConnectAsync(new Uri("ws://127.0.0.1:5152/"), CancellationToken.None);
                Debug.Log("Connected S");
                RequestId();
            } else if (ws.State == WebSocketState.Connecting)
            {
                return;
            }
            
            
            var sendTask = Task.Run(async () =>
            {
                var messageBytes = Encoding.UTF8.GetBytes(id + "/" + message);
                await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            );
            Debug.Log("sent message");
            await sendTask;
            //Console.ReadLine();

            //await Task.WhenAny(sendTask);

            //if (ws.State != WebSocketState.Closed)
            //{
            //    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            //}

            //Console.WriteLine("closed");

            //await Task.WhenAll(sendTask);
        }
        public async void ReceiveMessages()
        {
            //var ws = new ClientWebSocket();
            Debug.Log("state: " + ws.State);
            if (ws.State == WebSocketState.None)
            {
                Debug.Log("Connecting to Server");
                await ws.ConnectAsync(new Uri("ws://127.0.0.1:5152/"), CancellationToken.None);
                Debug.Log("Connected R");
                RequestId();
            }
            else if (ws.State == WebSocketState.Connecting)
            {
                return;
            }
            var receiveTask = Task.Run(async () =>
            {
                var buffer = new byte[1024 * 4];
                while (true)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close) { break; }
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log(message);
                    string[] msgParts = message.Split('/');
                    if (msgParts.Length >= 2 && msgParts[0] == "requestId")
                    {
                        id = Int32.Parse(msgParts[1]);
                    } else if (Int32.Parse(msgParts[0]) != id)
                    {
                        Log(msgParts[1]);
                    }
                }
            }
            );
            await receiveTask;

            if (false) // IF MESSAGE IS GOOD
            {
                Log("good message");
            }

        }

        public async void RequestId()
        {
            var sendTask = Task.Run(async () =>
            {
                var messageBytes = Encoding.UTF8.GetBytes("requestId/null");
                await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            );
            await sendTask;
        }
    }
}
