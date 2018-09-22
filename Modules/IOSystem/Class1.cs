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
            string str = File.ReadAllText(path);
            File.WriteAllText(path, str + split + text);
        }
    }
}
