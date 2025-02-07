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
        public string hubAddress = "ws://127.0.0.1:5152"; //"ws://192.168.43.107:5152/";
        public async void sendMessage(String message)
        {
            if (ws.State == WebSocketState.None)
            {
                Debug.Log("Connecting to Server");
                await ws.ConnectAsync(new Uri(hubAddress), CancellationToken.None);
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
            Debug.Log($"sent message: {message}");
            await sendTask;
            
        }
        public async void ReceiveMessages()
        {   
            if (ws.State == WebSocketState.None)
            {
                Debug.Log("Connecting to Server");
                await ws.ConnectAsync(new Uri(hubAddress), CancellationToken.None);
                Debug.Log("Connected R");
                RequestId();
            }
            else if (ws.State == WebSocketState.Connecting)
            {
                return;
            }
            Debug.Log("RECEIVE STARTED");
            var receiveTask = Task.Run(async () =>
            {
                var buffer = new byte[1024 * 4];
                while (true)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close) { break; }
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log($"Message received: {message}");
                    string[] msgParts = message.Split('/');
                    if (msgParts.Length >= 2 && id == null && msgParts[1] == "requestId")
                    {
                        Debug.Log("SET ID");
                        id = Int32.Parse(msgParts[0]);
                    } else if (Int32.Parse(msgParts[0]) != id && msgParts[1] != "requestId")
                    {
                        Log(msgParts[1]);
                    }
                }
            }
            );
            await receiveTask;
            Debug.Log("RECEIVE ENDED");

        }

        public async void RequestId()
        {
            var sendTask = Task.Run(async () =>
            {
                var messageBytes = Encoding.UTF8.GetBytes("null/requestId");
                await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            );
            await sendTask;
        }

        public async void Close()
        {
            if (ws.State != WebSocketState.Closed && ws.State != WebSocketState.None)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }

            Debug.Log("socket closed");
        }
    }
}
