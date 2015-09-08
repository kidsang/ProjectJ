using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorK
{
    public interface IDetailPanel
    {
        void OnShow(object[] args);
        void OnHide();
    }
}
