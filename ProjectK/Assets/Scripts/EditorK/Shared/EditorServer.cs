﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ProjectK.Base;

namespace EditorK
{
    public class EditorServer : EditorSocket
    {
        private Socket server;
        private long lastPingTime;

        public override void Init(OnConnectedCallback onConnectedCallback, object remoteCallObject)
        {
            base.Init(onConnectedCallback, remoteCallObject);

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint);
            server.Listen(10);
        }

        protected override void Connect()
        {
            try
            {
                state = SocketState.Connecting;
                Log.Info("Server begin accept.");
                server.BeginAccept(new AsyncCallback(OnAcceptResult), server);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void OnAcceptResult(IAsyncResult result)
        {
            socket = server.EndAccept(result);
            state = SocketState.Connected;
            lastPingTime = GetElapsedTime();
            Log.Info("Server accepted.");

            onConnectedCallback();
        }

        override protected void OnReceivePing()
        {
            lastPingTime = GetElapsedTime();
            Log.Info("Receive Ping!");
        }

        override public void Activate()
        {
            base.Activate();

            if (state == SocketState.Connected)
            {
                long timeMs = GetElapsedTime();
                if (timeMs - lastPingTime >= EditorSocket.PingTimeout)
                    ProcessSocketError("ConnectionLost", "ConnectionLost!");
            }
        }
    }
}
