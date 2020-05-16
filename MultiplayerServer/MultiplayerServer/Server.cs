using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MultiplayerServer
{
    public class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(byte fromClient, Packet packet);

        public static Dictionary<int, PacketHandler> PacketHandlers;

        private static TcpListener _tcpListener;
        private static UdpClient _udpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Log("Starting Server...");
            InitializeServerData();
            
            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            _udpListener = new UdpClient(Port);
            _udpListener.BeginReceive(UDPReceiveCallback, null);

            Log($"Server started on port {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = _tcpListener.EndAcceptTcpClient(result);
            _tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Log($"Incoming connection from {client.Client.RemoteEndPoint}...");
            
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client);
                    return;
                }
            }

            Log($"{client.Client.RemoteEndPoint} failed to connect. Server full!");
        }

        private static void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udpListener.EndReceive(result, ref clientEndPoint);
                _udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (Packet packet = new Packet(data))
                {
                    int clientId = packet.ReadInt();

                    if (clientId == 0)
                    {
                        return;
                    }

                    if (clients[clientId].udp.endPoint == null)
                    {
                        clients[clientId].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
                    {
                        clients[clientId].udp.HandleData(packet);
                    }
                }
            }
            catch (Exception e)
            {
                Log($"Error receiving UDP data: {e}");
            }
        }
        
        public static bool SendUDPData(IPEndPoint clientEndPoint, Packet packet)
        {
            try
            {
                if (clientEndPoint != null)
                {
                    _udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }

                return true;
            }
            catch (Exception e)
            {
                Log($"Error sending data to {clientEndPoint} via UDP: {e}");
                return false;
            }       
        }
        
        private static void InitializeServerData()
        {
            for (byte i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }
            
            PacketHandlers = new Dictionary<int, PacketHandler>
            {
                { (int) ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived },
                { (int) ClientPackets.KnightTexture, ServerHandle.KnightTexture },
                { (int) ClientPackets.FinishedSendingTexBytes, ServerHandle.FinishedSendingTexBytes },
                { (int) ClientPackets.PlayerPosition, ServerHandle.PlayerPosition },
                { (int) ClientPackets.PlayerScale, ServerHandle.PlayerScale },
                { (int) ClientPackets.PlayerAnimation, ServerHandle.PlayerAnimation },
                { (int) ClientPackets.SceneChanged, ServerHandle.SceneChanged },
                { (int) ClientPackets.HealthUpdated, ServerHandle.HealthUpdated },
                { (int) ClientPackets.CharmsUpdated, ServerHandle.CharmsUpdated },
                { (int) ClientPackets.PlayerDisconnected, ServerHandle.PlayerDisconnected },
            };
            
            Log("Initialized Packets.");
        }

        public static void Stop()
        {
            _tcpListener.Stop();
            _udpListener.Close();
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Server] " + message);
    }
}