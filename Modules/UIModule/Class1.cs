using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemModule;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace GameModule
{
    public class GameModule
    {
        Form frm;
        public void SetParam(CommandData data)
        {
            if (data.Name == "NewForm")
            {
                NewForm(data.Params);
            }
        }
        private void NewForm(SortedList<string, string> Params)
        {
            frm = new Form();
            frm.Text = Params["title"];
            frm.Size = new Size(int.Parse(Params["Size"].Split('x')[0]), int.Parse(Params["Size"].Split('x')[1]));
            Application.Run(frm);
        }            
    }
}
