using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SystemModule;

namespace GameModule
{
    public class GameModule
    {
        public void SetParam(CommandData data)
        {
            if (data.Name == "ConnectWeb")
            {
                //GameEngine.mess(data.Params["url"]);
                GameEngine.Vars.ChangeVar("EthernetRequest", GameEngine.replace(ConnectWeb(data.Params["url"]), GameEngine.NotValueVars.One, GameEngine.ValueVars.One));
            }
            if (data.Name == "Start")
            {
                GameEngine.Vars.AddVar("EthernetRequest", "Loaded", true);
            }
        }
        public static string ConnectWeb(string url)
        {
            string tmp = "";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        tmp += line;
                    }
                }
            }
            response.Close();
            return tmp;
        }
    }
}
