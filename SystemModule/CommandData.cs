using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemModule
{
    public class CommandData
    {
        public string Name = "";
        public SortedList<string, string> Params;
        public CommandData(string Name, SortedList<string,string> Params)
        {
            this.Name = Name;
            this.Params = Params;
        }
    }
}
