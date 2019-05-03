using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestModule
{    
    public struct Steep
    {
        public string Step;
        public string Text;
    }
    public struct SymbolyLink
    {
        string n;
        int o;
    }
    public class Line
    {       

        public string Name
        {
            get;
            private set;
        }
        public string Text
        {
            get;
            private set;
        }

        public List<Steep> Steep = new List<Steep>();

        public Line(string name)
        {
            Name = name;
        }
        public Line(string name, string text)
        {
            Name = name;
            Text = text;
        }
        public void AddStep(string step, string text)
        {
            Steep st = new QuestModule.Steep();
            st.Step = step;
            st.Text = text;
            Steep.Add(st);
        }

        public string ToText
        {
            get
            {
                string tmp = Text;
                tmp += Environment.NewLine;
                Steep.ForEach(temp => tmp += temp.Step + ".) " + temp.Text + Environment.NewLine);
                return tmp;
            }
        }
    }
}
