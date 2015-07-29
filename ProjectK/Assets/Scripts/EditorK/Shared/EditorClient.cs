using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ProjectK.Base;

namespace EditorK
{
    public class EditorClient : EditorSocket
    {
        protected override void Connect()
        {
            try
            {
                state = SocketState.Connecting;
                Log.Info("Client begin connect.");

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.BeginConnect(endPoint, new AsyncCallback(OnConnectResult), socket);
            }
            catch (SocketException e)
            {
                Log.Error(e);
            }
        }

        private void OnConnectResult(IAsyncResult result)
        {
            state = SocketState.Connected;
            socket.EndConnect(result);
            Log.Info("Cient connected.");

            onConnectedCallback();
        }
    }
}
