using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EditorK
{
    public static class FunctionPacker
    {
        enum PackType
        {
            Boolean,
            Int,
            Float,
            Double,
            String,
        }

        public static int PackAll(BinaryWriter writer, string funcName, object[] args)
        {
            int begin = (int)writer.Seek(sizeof(int), SeekOrigin.Begin);
            writer.Write(funcName);

            foreach (var arg in args)
            {
                if (arg is bool)
                {
                    writer.Write((byte)PackType.Boolean);
                    writer.Write((bool)arg);
                }
                else if (arg is string)
                {
                    writer.Write((byte)PackType.String);
                    writer.Write((string)arg);
                }
                else if (arg is int)
                {
                    writer.Write((byte)PackType.Int);
                    writer.Write((int)arg);
                }
                else if (arg is float)
                {
                    writer.Write((byte)PackType.Float);
                    writer.Write((float)arg);
                }
                else if (arg is double)
                {
                    writer.Write((byte)PackType.Double);
                    writer.Write((double)arg);
                }
                else
                {
                    throw new Exception("Pack失败，参数不是支持的类型。 type:" + arg.GetType() + " value:" + arg);
                }
            }

            int len = (int)writer.Seek(0, SeekOrigin.Current) - begin;
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(len);

            return begin + len;
        }

        public static void UnpackAll(BinaryReader reader, out string funcName, out object[] args)
        {
            Stream stream = reader.BaseStream;
            int len = reader.ReadInt32();
            int end = (int)stream.Position + len;

            funcName = reader.ReadString();

            List<object> argList = new List<object>();
            while (stream.Position < end)
            {
                object arg = null;
                PackType type = (PackType)reader.ReadByte();
                switch (type)
                {
                    case PackType.Boolean:
                        arg = reader.ReadBoolean();
                        break;

                    case PackType.String:
                        arg = reader.ReadString();
                        break;

                    case PackType.Int:
                        arg = reader.ReadInt32();
                        break;

                    case PackType.Float:
                        arg = reader.ReadSingle();
                        break;

                    case PackType.Double:
                        arg = reader.ReadDouble();
                        break;
                }
                argList.Add(arg);
            }
            args = argList.ToArray();
        }
    }
}
