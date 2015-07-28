using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.IO;
using System.Reflection;


namespace EditorK
{
    public abstract class EditorSocket
    {
        protected enum SocketState
        {
            Init,
            Connecting,
            Connected,
            Closed,
        }

        protected byte[] sendBuffer = new byte[65536];
        protected MemoryStream sendStream;
        protected BinaryWriter sendWriter;

        protected byte[] recvBuffer = new byte[65536];
        protected MemoryStream recvStream;
        protected BinaryReader recvReader;

        protected Socket socket;
        protected SocketState state = SocketState.Init;
        protected IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 19158);

        public delegate void OnConnectedCallback();
        protected OnConnectedCallback onConnectedCallback;

        protected object remoteCallObject;
        protected Type remoteCallType;

        public EditorSocket()
        {
            sendStream = new MemoryStream(sendBuffer);
            sendWriter = new BinaryWriter(sendStream, Encoding.UTF8);
            recvStream = new MemoryStream(recvBuffer);
            recvReader = new BinaryReader(recvStream, Encoding.UTF8);
        }

        public virtual void Init(OnConnectedCallback onConnectedCallback, object remoteCallObject)
        {
            this.onConnectedCallback = onConnectedCallback;
            if (remoteCallObject != null)
            {
                this.remoteCallObject = remoteCallObject;
                this.remoteCallType = remoteCallObject.GetType();
            }
        }

        public void Activate()
        {
            switch (state)
            {
                case SocketState.Init:
                    Connect();
                    break;

                case SocketState.Connecting:
                    break;

                case SocketState.Connected:
                    RecvMessage();
                    break;

                case SocketState.Closed:
                    break;
            }
        }

        protected abstract void Connect();

        private void RecvMessage()
        {
            try
            {
                if (socket.Available <= 0)
                    return;

                int recvSize = socket.Receive(recvBuffer);
                recvStream.Position = 0;
                while (recvStream.Position < recvSize)
                {
                    string funcName;
                    object[] args;
                    FunctionPacker.UnpackAll(recvReader, out funcName, out args);
                    //Console.WriteLine(funcName + "(" + string.Join(",", args) + ")");

                    if (remoteCallObject != null)
                    {
                        MethodInfo methodInfo = remoteCallType.GetMethod(funcName);
                        if (methodInfo == null)
                        {
                            Console.WriteLine("找不到RemoteCall函数:" + funcName);
                        }
                        else
                        {
                            try
                            {
                                methodInfo.Invoke(remoteCallObject, args);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("RemoteCall失败！ 函数名：" + funcName);
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProcessSocketError("recv message failed:", e.ToString());
            }
        }

        public void RemoteCall(string funcName, params object[] args)
        {
            try
            {
                int len = FunctionPacker.PackAll(sendWriter, funcName, args);
                sendWriter.Flush();
                socket.Send(sendBuffer, len, SocketFlags.None);
            }
            catch (Exception e)
            {
                ProcessSocketError("send message failed:", e.ToString());
            }
        }

        private void ProcessSocketError(string errorTitle, string error)
        {
            Console.WriteLine(errorTitle);
            Console.WriteLine(error);

            try
            {
                if (socket.Connected == true)
                    socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Shutdown client failed:");
                Console.WriteLine(e.ToString());
            }
            finally
            {
                socket = null;
                state = SocketState.Closed;
            }
        }

    }
}
