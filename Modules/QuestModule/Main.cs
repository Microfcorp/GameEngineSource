using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemModule;
using QuestModule;

using QuestManager.QuestFiles;
using QuestManager.QuestFiles.Quest;

namespace GameModule
{
    public class GameModule
    {
        struct Vars
        {
            public const string Request = "QuestRequest";
            public const string Steep = "QuestRequestSteep";
        }

        SortedList<string, Line> Lines = new SortedList<string, Line>();

        /* 0 - это всегда Старт
         *
         * */

        public void SetParam(CommandData data)
        {
            if (data.Name == "AddStart")
            {
                string text = data.Params["Text"];
                Lines.Add("Start", new Line("Start", text));
                GameEngine.Vars.AddVar(Vars.Request, (0).ToString(), true);
            }
            else if (data.Name == "AddLine")
            {
                string text = data.Params["Text"];
                string name = data.Params["Name"];
                string ptext = data.Params["ParentText"];
                string[] ParentLine = (data.Params["ParentLine"].Split('|'));
                Lines.Add(name, new Line(name, text));
                foreach (var item in ParentLine)
                {
                    Lines[item].AddStep(name, ptext);
                }                           
                GameEngine.Vars.AddVar(Vars.Request, (Lines.Count - 1).ToString(), true);
            }
            else if (data.Name == "GetSteep")
            {
                string steep = data.Params["Steep"];
                      
                GameEngine.Vars.AddVar(Vars.Steep, Lines[steep].ToText, true);
                if(Lines[steep].Steep.Count == 0)
                    GameEngine.Vars.AddVar(Vars.Request, "Zero", true);
            }

            else if (data.Name == "LoadFromFile")
            {
                string steep = data.Params["File"];
                var g = File.Open(steep);
                QuestManager.QuestFiles.Quest.QuestManager qm = QuestManager.QuestFiles.Quest.QuestManager.FromFile(g);
                foreach (var item in qm.Quests)
                {
                    string text = item.Text;
                    string name = item.Name;
                    string ptext = item.ParentText;
                    string[] ParentLine = item.ParentChild;
                    Lines.Add(name, new Line(name, text));
                    foreach (var item1 in ParentLine)
                    {
                        if(Lines.Keys.Contains(item1))
                            Lines[item1].AddStep(name, ptext);
                    }
                    GameEngine.Vars.AddVar(Vars.Request, (Lines.Count - 1).ToString(), true);
                }
            }

            else if (data.Name == "Start")
            {
                GameEngine.Vars.AddVar(Vars.Request, "Loaded", true);
            }
        }
    }
}
