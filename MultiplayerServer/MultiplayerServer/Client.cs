using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace MultiplayerServer
{
    public class Client
    {
        public static int dataBufferSize = 4096;
        
        public int id;
        public Player player;
        public TCP tcp;
        public UDP udp;

        public Client(int clientID)
        {
            id = clientID;
            tcp = new TCP(id);
            udp = new UDP(id);
        }
        
        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int id)
            {
                this.id = id;
            }

            /// <summary>Initializes the newly connected client's TCP-related info.</summary>
            /// <param name="socket">The TcpClient instance of the newly connected client.</param>
            public void Connect(TcpClient socket)
            {
                this.socket = socket;
                this.socket.ReceiveBufferSize = dataBufferSize;
                this.socket.SendBufferSize = dataBufferSize;

                stream = this.socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                string welcomeMessage = $"Welcome to the server! Your player ID is : {id}.";
                
                ServerSend.Welcome(id, welcomeMessage);
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
                    Log($"Error sending data to player {id} via TCP: {e}.");
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
                        Log("Byte length is less than 0, disconnecting...");
                        Server.clients[id].Disconnect();
                        
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data));
                    
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    Log(e);
                    
                    Server.clients[id].Disconnect();
                }
            }
            
            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="data">The recieved data.</param>
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
                            Server.PacketHandlers[packetId](id, packet);
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

            /// <summary>Closes and cleans up the TCP connection.</summary>
            public void Disconnect()
            {
                Log("Disconnecting TCP (Server)");
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public IPEndPoint endPoint;

            private int id;

            public UDP(int id)
            {
                this.id = id;
            }

            /// <summary>Initializes the newly connected client's UDP-related info.</summary>
            /// <param name="endPoint">The IPEndPoint instance of the newly connected client.</param>
            public void Connect(IPEndPoint endPoint)
            {
                this.endPoint = endPoint;
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="packet">The packet to send.</param>
            public void SendData(Packet packet)
            {
                Log("Server.SendUDPData(endPoint, packet)");
                Server.SendUDPData(endPoint, packet);
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="packetData">The packet containing the recieved data.</param>
            public void HandleData(Packet packetData)
            {
                int packetLength = packetData.ReadInt();
                byte[] packetBytes = packetData.ReadBytes(packetLength);
                
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.PacketHandlers[packetId](id, packet);
                    }
                });
            }

            /// <summary>Cleans up the UDP connection.</summary>
            public void Disconnect()
            {
                Log("Disconnecting UDP (Server)");
                endPoint = null;
            }
        }

        /// <summary>Sends the client into the game and informs other clients of the new player.</summary>
        /// <param name="username">The username of the new player.</param>
        /// <param name="position">The spawn position of the new player.</param>
        /// <param name="scale">The spawn scale of the new player.</param>
        public void SendIntoGame(string username, Vector3 position, Vector3 scale)
        {
            player = NetworkManager.Instance.InstantiatePlayer(position, scale);
            player.Initialize(id, username);

            foreach (Client client in Server.clients.Values)
            {
                if (client.player != null)
                {
                    if (client.id != id)
                    {
                        // Send existing player to the new client
                        Log($"Send existing player {client.player.username} to the new client {id}");
                        ServerSend.SpawnPlayer(id, client.player);
                        
                        // Send the new player to the existing client
                        Log($"Send the new player {player.username} to the existing client {client.id}");
                        ServerSend.SpawnPlayer(client.id, player);
                    }
                }
            }
        }

        /// <summary>Disconnects the client and stops all network traffic.</summary>
        private void Disconnect()
        {
            Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected from the server.");

            ThreadManager.ExecuteOnMainThread(() =>
            {
                UnityEngine.Object.Destroy(player.gameObject);
                player = null;
            });

            tcp.Disconnect();
            udp.Disconnect();
            
            ServerSend.PlayerDisconnected(id);
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Client] (Server) " + message);
    }
}