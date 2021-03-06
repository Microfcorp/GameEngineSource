﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace SystemModule
{
    public class CommandData
    {
        public string Name
        {
            get;
            private set;
        }
            
        public SortedList<string, string> Params
        {
            get;
            private set;
        }

        public CommandData(string Name, SortedList<string,string> Params)
        {
            this.Name = Name;
            this.Params = Params;
            this.Params.Values.ToList().ForEach(tmp => tmp = tmp.TrimEnd(' '));
        }
    }
    public static class GameEngine
    {
        public static void mess(string text)
        {
            Console.WriteLine(text);
        }
        public static void cls(string text)
        {
            Console.Clear();
        }
        public static void printimg(string url)
        {
            Bitmap bmpSrc = new Bitmap(@url, true);
            ConsoleWriteImage(bmpSrc);
        }
        private static void printimage(string path)
        {
            Image Picture = Image.FromFile(@path);
            Console.SetBufferSize((Picture.Width * 0x2), (Picture.Height * 0x2));
            FrameDimension Dimension = new FrameDimension(Picture.FrameDimensionsList[0x0]);
            int FrameCount = Picture.GetFrameCount(Dimension);
            int Left = Console.WindowLeft, Top = Console.WindowTop;
            char[] Chars = { '#', '#', '@', '%', '=', '+', '*', ':', '-', '.', ' ' };
            Picture.SelectActiveFrame(Dimension, 0x0);
            for (int i = 0x0; i < Picture.Height; i++)
            {
                for (int x = 0x0; x < Picture.Width; x++)
                {
                    Color Color = ((Bitmap)Picture).GetPixel(x, i);
                    int Gray = (Color.R + Color.G + Color.B) / 0x3;
                    int Index = (Gray * (Chars.Length - 0x1)) / 0xFF;
                    Console.Write(Chars[Index]);
                }
                Console.Write('\n');
            }
            Console.SetCursorPosition(Left, Top);
        }

        private static int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };

        private static void ConsoleWritePixel(Color cValue)
        {
            Color[] cTable = cColors.Select(x => Color.FromArgb(x)).ToArray();
            char[] rList = new char[] { (char)9617, (char)9618, (char)9619, (char)9608 }; // 1/4, 2/4, 3/4, 4/4
            int[] bestHit = new int[] { 0, 0, 4, int.MaxValue }; //ForeColor, BackColor, Symbol, Score

            for (int rChar = rList.Length; rChar > 0; rChar--)
            {
                for (int cFore = 0; cFore < cTable.Length; cFore++)
                {
                    for (int cBack = 0; cBack < cTable.Length; cBack++)
                    {
                        int R = (cTable[cFore].R * rChar + cTable[cBack].R * (rList.Length - rChar)) / rList.Length;
                        int G = (cTable[cFore].G * rChar + cTable[cBack].G * (rList.Length - rChar)) / rList.Length;
                        int B = (cTable[cFore].B * rChar + cTable[cBack].B * (rList.Length - rChar)) / rList.Length;
                        int iScore = (cValue.R - R) * (cValue.R - R) + (cValue.G - G) * (cValue.G - G) + (cValue.B - B) * (cValue.B - B);
                        if (!(rChar > 1 && rChar < 4 && iScore > 50000)) // rule out too weird combinations
                        {
                            if (iScore < bestHit[3])
                            {
                                bestHit[3] = iScore; //Score
                                bestHit[0] = cFore;  //ForeColor
                                bestHit[1] = cBack;  //BackColor
                                bestHit[2] = rChar;  //Symbol
                            }
                        }
                    }
                }
            }
            Console.ForegroundColor = (ConsoleColor)bestHit[0];
            Console.BackgroundColor = (ConsoleColor)bestHit[1];
            Console.Write(rList[bestHit[2] - 1]);
        }
        private static void ConsoleWriteImage(Bitmap source)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
            Size dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));
            Bitmap bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
            for (int i = 0; i < dSize.Height; i++)
            {
                for (int j = 0; j < dSize.Width; j++)
                {
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2, i));
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2 + 1, i));
                }
                System.Console.WriteLine();
            }
            Console.ResetColor();
        }
        public static void setbackphoto(string url)
        {
            printimage(url);
        }
        public static void ascllart(string url, int start, int stop)
        {
            var tmp = File.ReadAllText(url.Remove('\r', ' '));
            string[] temp = tmp.Split('\n');

            int cola1 = Convert.ToInt32(start);
            int colb1 = Convert.ToInt32(stop);
            for (int ic1 = 0; ic1 < (colb1 - cola1); ic1++)
            {
                Console.WriteLine(temp[cola1 + ic1]);
            }
        }
        public static void playmusic(string url)
        {
            SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = url;
            player.Play();
        }
        public static void sett(string title, ConsoleColor consolecolor)
        {
            Console.Title = title;
            Console.BackgroundColor = consolecolor;
        }
        public static void close()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        public static void pause()
        {
            Console.ReadLine();
        }
        public static void closegame(string mess)
        {
            Console.WriteLine(mess);
            System.Threading.Thread.Sleep(1500);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        public static void finish(string mess)
        {
            MessageBox.Show(mess);
            Console.WriteLine("Вы прошли игру");
            Console.ReadLine();
            Application.Restart();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        public static string replace(string In, string From, string To)
        {
            return In.Replace(From,To);
        }
        public static class Applications
        {
            public static void Start(string name, string args)
            {
                Process.Start(name, args);
            }
            public static void Resize(int X, int Y)
            {
                Console.SetBufferSize(X, Y);
            }
        }
        public static class NotValueVars
        {
            public static string One
            {
                get
                {
                    return ";";
                }
            }
        }
        public static class ValueVars
        {
            public static string One
            {
                get
                {
                    return "|";
                }
            }
        }
        public static int Random(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max);
        }
        public static void Crash(string help)
        {
            throw new Exception(help);
        }
        public static class Vars
        {
            public class KeyAndValue
            {
                public string Key
                {
                    get;
                    private set;
                }
                public string Value
                {
                    get;
                    private set;
                }
                public int ValueToInt
                {
                    get
                    {
                        int res = 0;
                        bool ok = int.TryParse(Value, out res);
                        if (ok)
                            return res;
                        else
                            return 0xFF;
                    }
                }

                public TypeVars Type
                {
                    get;
                    private set;
                }

                public enum TypeVars
                {
                    String,
                    Int,
                }
                    
                internal KeyAndValue(string key, string value)
                {
                    Key = key;
                    Value = value;

                    int res = 0;
                    bool ok = int.TryParse(Value, out res);
                    if (ok)
                        Type = TypeVars.Int;
                    else
                        Type = TypeVars.String;
                }
            }
            public static KeyAndValue[] GetVars()
            {
                string tmp = "";
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp"))
                {
                    tmp = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp");
                }
                else
                {
                    Crash("Ошибка, переменнные еще не объявлены");                   
                }                
                string[] vars = tmp.Split(';');
                List<KeyAndValue> ret = new List<KeyAndValue>();
                foreach (var item in vars)
                {
                    if(item.Split('=').Length > 1)
                        ret.Add(new KeyAndValue(item.Split('=')[0], item.Split('=')[1]));
                }
                return ret.ToArray();
            }
            public static KeyAndValue GetVar(string name)
            {
                string tmp = "";
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp"))
                {
                    tmp = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp");
                }
                else
                {
                    Crash("Ошибка, переменнные еще не объявлены");

                }
                string[] vars = tmp.Split(';');
                KeyAndValue ret = new KeyAndValue(null, null);
                foreach (var item in vars)
                {
                    if (item.Split('=')[0] == name)
                    {
                        if (item.Split('=').Length > 1)
                            ret = (new KeyAndValue(item.Split('=')[0], item.Split('=')[1]));
                    }
                }
                return ret;
            }
            public static bool IsVars(string name)
            {
                string tmp = "";
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp"))
                {
                    tmp = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp");
                }
                else
                {
                    Crash("Ошибка, переменнные еще не объявлены");

                }
                string[] vars = tmp.Split(';');
                foreach (var item in vars)
                {
                    if (item.Split('=')[0] == name)
                        return true;
                }
                return false;
            }
            public static KeyAndValue AddVar(string name, string value, bool AddAndChange = false)
            {
                if (AddAndChange)
                {
                    if (IsVars(name))
                    {
                        ChangeVar(name, value);
                    }
                    else
                    {
                        AddVar(name, value);
                    }
                }
                else
                {
                    AddVar(name, value);
                }
                return new KeyAndValue(name, value);
            }
            public static KeyAndValue AddVar(string name, string value)
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp"))
                {
                    string tmp = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp");
                    tmp += name + "=" + value + ";";
                    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create) + "\\ConsoleGameEngine.tmp", tmp);
                }
                else
                {
                    Crash("Ошибка, переменнные еще не объявлены");
                }
                return new KeyAndValue(name, value);
            }
            public static KeyAndValue ChangeVar(string name, string value)
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp"))
                {
                    string tmp = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ConsoleGameEngine.tmp");
                    string tmp1 = "";

                    foreach (var item in tmp.Split(';'))
                    {
                        if (item.Split('=').Length > 1)
                        {
                            if (item.Split('=')[0] == name)
                            {
                                tmp1 += item.Split('=')[0] + "=" + value + ";";
                            }
                            else
                            {
                                tmp1 += item.Split('=')[0] + "=" + item.Split('=')[1] + ";";
                            }
                        }
                    }
                    
                    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create) + "\\ConsoleGameEngine.tmp", tmp1);
                }
                else
                {
                    Crash("Ошибка, переменнные еще не объявлены");
                }
                return new KeyAndValue(name, value);
            }
        }
    }
}
