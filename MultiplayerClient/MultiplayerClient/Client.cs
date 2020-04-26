using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerClient
{
    public class Client : MonoBehaviour
    {
        public static Client Instance;
        public static int dataBufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 26950;
        public string username = "Default";
        public int myId;
        public TCP tcp;
        public UDP udp;

        public bool isConnected;
        
        private delegate void PacketHandler(Packet packet);

        private static Dictionary<int, PacketHandler> _packetHandlers;
        
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
            {
                Log("Instance already exists, destroying object.");
                Destroy(Instance);
            }
        }

        private void Start()
        {
            tcp = new TCP();
            udp = new UDP();
        }
        
        private void OnApplicationQuit()
        {
            Disconnect();
        }

        /// <summary>Attempts to connect to the server.</summary>
        public void ConnectToServer()
        {
            InitializeClientData();
            
            tcp.Connect();
            
            isConnected = true;
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            /// <summary>Attempts to connect to the server via TCP.</summary>
            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize,
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(Instance.ip, Instance.port, ConnectCallback, socket);
            }

            /// <summary>Initializes the newly connected client's TCP-related info.</summary>
            private void ConnectCallback(IAsyncResult result)
            {
                socket.EndConnect(result);

                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            /// <summary>Sends data to the client via TCP.</summary>
            /// <param name="packet">The packet to send.</param>
            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Log($"Error sending data to server via TCP: {e}.");
                }
            }
            
            /// <summary>Reads incoming data from the stream.</summary>
            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        Log("Byte Length <= 0: Disconnecting!!!!");
                        if (Instance.isConnected)
                        {
                            Instance.Disconnect();
                        }
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data)); // Reset receivedData if all data was handled
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch
                {
                    Disconnect();
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="data">The received data.</param>
            private bool HandleData(byte[] data)
            {
                int packetLength = 0;

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
                {
                    byte[] packetBytes = receivedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetId = packet.ReadInt();
                            _packetHandlers[packetId](packet);
                        }
                    });

                    packetLength = 0;
                    
                    if (receivedData.UnreadLength() >= 4)
                    {
                        packetLength = receivedData.ReadInt();
                        if (packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
            public void Disconnect()
            {
                if (Instance.isConnected)
                {
                    Instance.Disconnect();
                }

                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(Instance.ip), Instance.port);
            }

            /// <summary>Attempts to connect to the server via UDP.</summary>
            /// <param name="localPort">The port number to bind the UDP socket to.</param>
            public void Connect(int localPort)
            {
                socket = new UdpClient(localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                using (Packet packet = new Packet())
                {
                    SendData(packet);
                }
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="packet">The packet to send.</param>
            public void SendData(Packet packet)
            {
                try
                {
                    packet.InsertInt(Instance.myId);
                    if (socket != null)
                    {
                        if (endPoint == null)
                        {
                            endPoint = new IPEndPoint(IPAddress.Parse(Instance.ip), Instance.port);
                        }
                        
                        if (!socket.Client.Connected)
                        {
                            socket.Connect(endPoint);
                            socket.BeginReceive(ReceiveCallback, null);
                        }


                        socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Log($"Error sending data to server via UDP: {e}.");
                }
            }
            
            /// <summary>Receives incoming UDP data.</summary>
            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    byte[] data = socket.EndReceive(result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);
                    
                    if (data.Length < 4)
                    {
                        if (Instance.isConnected)
                        {
                            Instance.Disconnect();
                        }
                        return;
                    }

                    HandleData(data);
                }
                catch (Exception e)
                {
                    Log("Error receiving UDP callback: " + e);
                    Disconnect();
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="data">The received data.</param>
            private void HandleData(byte[] data)
            {
                using (Packet packet = new Packet(data))
                {
                    int packetLength = packet.ReadInt();
                    data = packet.ReadBytes(packetLength);
                }
                
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(data))
                    {
                        int packetId = packet.ReadInt();
                        _packetHandlers[packetId](packet);
                    }
                });
            }

            /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
            public void Disconnect()
            {
                if (Instance.isConnected)
                {
                    Instance.Disconnect();
                }
                
                endPoint = null;
                socket = null;
            }
        }
        
        /// <summary>Initializes all necessary client data.</summary>
        private void InitializeClientData()
        {
            _packetHandlers = new Dictionary<int, PacketHandler>
            {
                { (int) ServerPackets.Welcome, ClientHandle.Welcome },
                { (int) ServerPackets.SpawnPlayer, ClientHandle.SpawnPlayer },
                { (int) ServerPackets.DestroyPlayer, ClientHandle.DestroyPlayer },
                { (int) ServerPackets.PvPEnabled, ClientHandle.PvPEnabled },
                { (int) ServerPackets.PlayerPosition, ClientHandle.PlayerPosition },
                { (int) ServerPackets.PlayerScale, ClientHandle.PlayerScale },
                { (int) ServerPackets.PlayerAnimation, ClientHandle.PlayerAnimation },
                { (int) ServerPackets.CharmsUpdated, ClientHandle.CharmsUpdated },
                { (int) ServerPackets.PlayerDisconnected, ClientHandle.PlayerDisconnected },
            };
            Log("Initialized Packets.");
        }

        /// <summary>Disconnects from the server and stops all network traffic.</summary>
        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                udp.socket.Close();

                Log("You have been disconnected from the server.");
            }
            
            GameManager.Instance.DestroyAllPlayers();
        }

        private static void Log(object message) => Modding.Logger.Log("[Client] (Client) " + message);
    }
}