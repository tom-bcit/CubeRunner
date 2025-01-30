using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System;
namespace NetworkAPI
{
    public class Messaging
    {
        public delegate void LogHandler(string message);

        public event LogHandler Log;

        public void sendMessage(String message)
        {
            IPAddress mcastAddress;
            int mcastPort;
            Socket mcastSocket = null;
            mcastAddress = IPAddress.Parse("230.0.0.1");
            mcastPort = 11000;
            IPEndPoint endPoint;

            try
            {
                mcastSocket = new Socket(AddressFamily.InterNetwork,
                               SocketType.Dgram,
                               ProtocolType.Udp);

                //Send multicast packets to the listener.
                endPoint = new IPEndPoint(mcastAddress, mcastPort);
                mcastSocket.SendTo(ASCIIEncoding.ASCII.GetBytes(message), endPoint);
                //Debug.Log("Message Sent");

            }
            catch (Exception e)
            {
                Debug.Log("\n" + e.ToString());
            }

            mcastSocket.Close();
        }
        public void ReceiveMessages()
        {
            IPAddress mcastAddress;
            int mcastPort;
            Socket mcastSocket = null;
            MulticastOption mcastOption = null;
            mcastAddress = IPAddress.Parse("230.0.0.1");
            mcastPort = 11000;
            string localIPAddress = GetLocalIPAddress();

            try
            {
                mcastSocket = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Dgram,
                                         ProtocolType.Udp);
                IPAddress localIP = IPAddress.Parse(GetLocalIPAddress());
                EndPoint localEP = (EndPoint)new IPEndPoint(localIP, mcastPort);
                mcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                mcastSocket.Bind(localEP);
                mcastOption = new MulticastOption(mcastAddress, localIP);
                mcastSocket.SetSocketOption(SocketOptionLevel.IP,
                                            SocketOptionName.AddMembership,
                                            mcastOption);

                bool done = false;
                byte[] bytes = new Byte[100];
                IPEndPoint groupEP = new IPEndPoint(mcastAddress, mcastPort);
                EndPoint remoteEP = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

                while (!done)
                {
                    // Clear the buffer before receiving the new message
                    Array.Clear(bytes, 0, bytes.Length);

                    // Receive the message
                    mcastSocket.ReceiveFrom(bytes, ref remoteEP);

                    // Convert the received bytes to a string (ensure it's properly null-terminated)
                    string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length).TrimEnd('\0');

                    // Ensure the message has content before processing
                    if (!string.IsNullOrEmpty(message))
                    {
                        string senderIPAddress = ((IPEndPoint)remoteEP).Address.ToString();
                        
                        IPAddress senderIP = IPAddress.Parse(senderIPAddress);
                        if (senderIP.Equals(localIP))
                        {
                            // Skip processing if the sender is the local machine
                            continue;
                        } else 
                        {
                            Debug.Log($"sender: {senderIPAddress}\tlocal:{localIPAddress}");
                            Log(message);
                        }                        
                    }
                }
                mcastSocket.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        private string GetLocalIPAddress()
        {
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1"; // Default to localhost if no IP found
        }

    }


}
