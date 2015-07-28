using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace EditorK
{
    public class EditorClient : EditorSocket
    {
        protected override void Connect()
        {
            try
            {
                state = SocketState.Connecting;
                Console.WriteLine("Client begin connect.");

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.BeginConnect(endPoint, new AsyncCallback(OnConnectResult), socket);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void OnConnectResult(IAsyncResult result)
        {
            state = SocketState.Connected;
            socket.EndConnect(result);
            Console.WriteLine("Cient connected.");

            onConnectedCallback();
        }
    }
}
