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
            Table,
            Null,
        }

        public static int PackAll(BinaryWriter writer, string funcName, object[] args = null)
        {
            int begin = (int)writer.Seek(sizeof(int), SeekOrigin.Begin);
            writer.Write(funcName);

            if (args != null)
            {
                foreach (var arg in args)
                    PackOne(writer, arg);
            }

            int len = (int)writer.Seek(0, SeekOrigin.Current) - begin;
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(len);

            return begin + len;
        }

        private static void PackOne(BinaryWriter writer, object arg)
        {
            if (arg == null)
            {
                writer.Write((byte)PackType.Null);
            }
            else if (arg is bool)
            {
                writer.Write((byte)PackType.Boolean);
                writer.Write((bool)arg);
            }
            else if (arg is string)
            {
                writer.Write((byte)PackType.String);
                writer.Write((string)arg);
            }
            else if ((arg is int) || (arg is Enum))
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
            else if (arg is InfoMap)
            {
                InfoMap table = arg as InfoMap;
                writer.Write((byte)PackType.Table);
                writer.Write(table.Count);
                foreach (var pair in table)
                {
                    PackOne(writer, pair.Key);
                    PackOne(writer, pair.Value);
                }
            }
            else
            {
                throw new Exception("Pack失败，参数不是支持的类型。 type:" + arg.GetType() + " value:" + arg);
            }
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
                object arg = UnpackOne(reader);
                argList.Add(arg);
            }
            args = argList.ToArray();
        }

        private static object UnpackOne(BinaryReader reader)
        {
            object arg = null;
            PackType type = (PackType)reader.ReadByte();
            switch (type)
            {
                case PackType.Null:
                    break;

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

                case PackType.Table:
                    InfoMap table = new InfoMap();
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; ++i)
                    {
                        string key = (string)UnpackOne(reader);
                        object val = UnpackOne(reader);
                        table[key] = val;
                    }
                    arg = table;
                    break;
            }
            return arg;
        }
    }
}
