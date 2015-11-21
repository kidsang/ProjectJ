using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectK;

namespace ProjectK
{
    /// <summary>
    /// 所有组件的基类
    /// </summary>
    public class GameComp
    {
        /// <summary>
        /// 组件所属的Entity
        /// </summary>
        public SceneEntity Entity { get; set; }

        /// <summary>
        /// 组件名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 组件是否已被初始化
        /// </summary>
        public bool Started { get; private set; }

        public GameComp()
        {
            Type type = GetType();
            Name = type.Name;
        }

        /// <summary>
        /// 被添加到Entity时调用一次，用于初始化组件
        /// </summary>
        virtual public bool Start()
        {
            if (Started)
                return false;

            Started = true;
            return true;
        }

        /// <summary>
        /// 销毁组件，做一些资源释放工作
        /// </summary>
        virtual public void Destroy()
        {

        }

        /// <summary>
        /// 每帧都会调用
        /// </summary>
        virtual public void Update()
        {

        }

        //public override string ToString()
        //{
        //    return "<GameComp>" + Name;
        //}
    }
}
