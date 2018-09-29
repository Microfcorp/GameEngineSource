using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemModule;
using System.IO;

namespace GameModule
{
    public class GameModule
    {
        public void SetParam(CommandData data)
        {
            if(data.Name == "ReadFile")
            {
                ReadFile(data.Params["path"]);
            }
            else if (data.Name == "WriteFile")
            {
                WriteFile(data.Params["path"], data.Params["text"]);
            }
            else if (data.Name == "ReadAndWrire")
            {
                ReadAndWrite(data.Params["path"], data.Params["text"], data.Params["split"]);
            }
            else if (data.Name == "ReadAndFind")
            {
                ReadAndFind(data.Params["path"], data.Params["find"]);
            }
        }

        public void ReadFile(string path)
        {
            Console.WriteLine(File.ReadAllText(path));
        }
        public void WriteFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }
        public void ReadAndWrite(string path, string text, string split)
        {
            string str = "";
            if (File.Exists(path))
            {
               str = File.ReadAllText(path);
            }
            File.WriteAllText(path, str + split + text);
        }
        public void ReadAndFind(string path, string text)
        {
            string str = File.ReadAllText(path);
            foreach (var item in str.Split('\n'))
            {
                if(item.Split('=')[0] == text)
                {
                    GameEngine.mess(item.Split('=')[1]);
                }
            }
        }
    }
}
