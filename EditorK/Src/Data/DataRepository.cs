﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK.Base;

namespace EditorK
{
    public class DataRepository<T> where T : class, new()
    {
        private List<HistoryData> history = new List<HistoryData>();
        private int nextIndex = 0;
        public T Data { get; private set; }
        public bool Recording { get; set; }

        public string CurrentEvt { get; private set; }
        public InfoMap CurrentInfos { get; private set; }

        public void New(T data, string evt, InfoMap infos)
        {
            Clear(0);
            nextIndex = 1;
            HistoryData current = new HistoryData(data, evt, infos);
            history.Add(current);
            Data = Clone(current.Data);

            CurrentEvt = evt;
            CurrentInfos = infos;
            EventManager.Instance.FireEvent(evt, infos);
        }

        public void Modify(string evt, InfoMap infos)
        {
            if (Recording)
            {
                Clear(nextIndex);
                nextIndex += 1;
                HistoryData current = new HistoryData(Data, evt, infos);
                history.Add(current);
                Data = Clone(current.Data);
            }

            CurrentEvt = evt;
            CurrentInfos = infos;
            EventManager.Instance.FireEvent(evt, infos);
        }

        public void Undo()
        {
            if (nextIndex <= 1)
                return;

            nextIndex -= 1;
            HistoryData last = history[nextIndex];
            HistoryData current = history[nextIndex - 1];
            Data = Clone(current.Data);

            CurrentEvt = last.Event;
            CurrentInfos = last.Infos;
            EventManager.Instance.FireEvent(last.Event, last.Infos);
        }

        public void Redo()
        {
            if (nextIndex >= history.Count)
                return;

            HistoryData current = history[nextIndex];
            Data = Clone(current.Data);
            nextIndex += 1;

            CurrentEvt = current.Event;
            CurrentInfos = current.Infos;
            EventManager.Instance.FireEvent(current.Event, current.Infos);
        }

        private void Clear(int toIndex)
        {
            history.RemoveRange(toIndex, history.Count - toIndex);
            nextIndex = toIndex;
        }

        private T Clone(T data)
        {
            return SimpleJson.DeserializeObject<T>(SimpleJson.SerializeObject(data));
        }

        class HistoryData
        {
            public HistoryData(T data, string evt, InfoMap infos)
            {
                Data = data;
                Event = evt;
                Infos = infos;
            }

            public T Data;
            public string Event;
            public InfoMap Infos;
        }
    }
}
