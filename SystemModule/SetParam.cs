using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemModule
{
    class SetParams
    {
        public SortedList<string, Action> act = new SortedList<string, Action>();
        public void SetParam(CommandData data)
        {
            act[data.Name].DynamicInvoke(data.Params);
        }
        public void SetAct(Action at, string name)
        {
            act.Add(name, at);
        }
    }
}
