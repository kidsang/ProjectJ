﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EditorK
{
    public class InfoMap : Dictionary<string, object>
    {
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[InfoMap]{");
            foreach (var pair in this)
            {
                builder.Append(pair.Key);
                builder.Append(":");
                builder.Append(pair.Value);
                builder.Append(",");
            }
            builder.Append("}");
            return builder.ToString();
        }
    }

    public static class EditorUtils
    {
        public static Color SelectedColor = new Color(0, 0.6f, 1);

        public static InfoMap GetEventInfos(object[] args)
        {
            return args[0] as InfoMap;
        }

        public static void SetFlag(ref int flags, int flag, bool value)
        {
            if (value)
                flags |= flag;
            else
                flags &= ~flag;
        }

        public static bool HasFlag(int flags, int flag)
        {
            return (flags & flag) != 0;
        }
    }
}
