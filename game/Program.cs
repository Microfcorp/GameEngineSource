using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

namespace game
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.Title = "Запуск игры";
                Console.WriteLine("Выберите номер игры из списка:");
                for (int i = 0; i < getfullgame().Length; i++)
                {
                    Console.WriteLine("{0}. {1}", i, getfullgame()[i]);
                }
                loadgame(Environment.CurrentDirectory + "\\games\\" + getfullgame()[Convert.ToInt32(Console.ReadLine())]);
            }
            else if(args[0] != null)
            {
                loadgame(args[0]);
            }
        }

        public static string[] getfullgame()
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\games\\");
            string[] path = Directory.GetFiles(Environment.CurrentDirectory + "\\games\\", "*.gam");
            string[] tmp = new string[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                tmp[i] += path[i].Split('\\')[path[i].Split('\\').Length - 1];
            }

            return tmp;
        }

        public static void printimage(string path)
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

        static int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };

        public static void ConsoleWritePixel(Color cValue)
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


        public static void ConsoleWriteImage(Bitmap source)
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

        public static string connecttoserver(string server, string login, string passw, string path, string socet)
        {            
            string tmp = "";
            WebRequest request = WebRequest.Create("http://" + server + "/" + socet + "?login=" + login + "&passw=" + passw + "&path=" + path);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                       tmp += line + "\n";
                    }
                }
            }
            response.Close();
            return tmp;
        }

        public static void downloadcashe(string path, string filename)
        {
            string remoteUri = "http://" + path;
            string fileName = filename, myStringWebResource = null;
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri + fileName;
            //Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(remoteUri, fileName);
            //Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
            //Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t" + Application.StartupPath);
        }

        public static void loadgame(string path)
        {
            string path1 = path.Replace(path.Split('\\')[path.Split('\\').Length - 1], "");
            string game = File.ReadAllText(path).Replace("\r","").Replace("getcurrentdirect:", path1);
            bool rasm = false;
            bool autoelse = false;
            int lenght = game.Split('\n').Length;
            SortedList<string, string> strings = new SortedList<string, string>();
            SortedList<string, int> ints = new SortedList<string, int>();

            for (int i = 0; i < lenght; i++)
            {
                sled:
                switch (game.Split('\n')[i].Split(';')[0])
                {
                    case "mess":
                        Console.WriteLine(game.Split('\n')[i].Split(';')[1]);
                        break;
                    case "cls":
                        Console.Clear();
                        break;
                    case "cashe":
                        string[] files = game.Split('\n')[i].Split(';')[2].Split(',');
                        foreach (var item in files)
                        {
                            downloadcashe(game.Split('\n')[i].Split(';')[1] + "/" + item, path1 + "\\" + item);
                        }
                        break;
                    case "printimg":
                        Bitmap bmpSrc = new Bitmap(@game.Split('\n')[i].Split(';')[1], true);
                        ConsoleWriteImage(bmpSrc);
                        break;
                    case "setbackphoto":
                        printimage(game.Split('\n')[i].Split(';')[1]);
                        break;
                    case "string":
                        strings.Add(game.Split('\n')[i].Split(';')[1].Split(':')[0], game.Split('\n')[i].Split(';')[1].Split(':')[1]);
                        break;
                    case "messstring":
                        Console.WriteLine(strings[game.Split('\n')[i].Split(';')[1]]);
                        break;
                    case "messint":
                        Console.WriteLine(ints[game.Split('\n')[i].Split(';')[1]].ToString());
                        break;
                    case "int":
                        ints.Add(game.Split('\n')[i].Split(';')[1].Split(':')[0], Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split(':')[1]));
                        break;
                    case "random":
                        Random rnd = new Random();
                        int value = rnd.Next(Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split('-')[0]), Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split('-')[1].Split(':')[0]));
                        string outs2 = game.Split('\n')[i].Split(';')[1].Split(':')[1];
                        if (outs2 == "consoleprint") { Console.WriteLine(value); }
                        else
                        {
                            ints[outs2] = value;
                        }
                    break;
                    case "let":
                        string outs = game.Split('\n')[i].Split(';')[1].Split(':')[1];
                        string viraz = game.Split('\n')[i].Split(';')[1].Split(':')[0];
                        int itog = 0;
                        if (viraz.Contains("+")) { itog = Convert.ToInt32(viraz.Split('+')[0]) + Convert.ToInt32(viraz.Split('+')[1]); }
                        if (viraz.Contains("-")) { itog = Convert.ToInt32(viraz.Split('-')[0]) - Convert.ToInt32(viraz.Split('-')[1]); }
                        if (viraz.Contains("*")) { itog = Convert.ToInt32(viraz.Split('*')[0]) * Convert.ToInt32(viraz.Split('*')[1]); }
                        if (viraz.Contains("/")) { itog = Convert.ToInt32(viraz.Split('/')[0]) / Convert.ToInt32(viraz.Split('/')[1]); }
                        if (outs == "consoleprint") { Console.WriteLine(itog); }
                        else
                        {
                            ints[outs] = itog;
                        }
                    break;
                    case "intlet":
                        string outs1 = game.Split('\n')[i].Split(';')[3].Split(':')[1];
                        string viraz1 = game.Split('\n')[i].Split(';')[1] + game.Split('\n')[i].Split(';')[2] + game.Split('\n')[i].Split(';')[3];
                        int itog1 = 0;
                        if (viraz1.Contains("+")) { itog1 = ints[viraz1.Split('+')[0]] + ints[viraz1.Split('+')[1].Split(':')[0]]; }
                        if (viraz1.Contains("-")) { itog1 = ints[viraz1.Split('-')[0]] - ints[viraz1.Split('-')[1].Split(':')[0]]; }
                        if (viraz1.Contains("*")) { itog1 = ints[viraz1.Split('*')[0]] * ints[viraz1.Split('*')[1].Split(':')[0]]; }
                        if (viraz1.Contains("/")) { itog1 = ints[viraz1.Split('/')[0]] / ints[viraz1.Split('/')[1].Split(':')[0]]; }
                        if (outs1 == "consoleprint") { Console.WriteLine(itog1); }
                        else
                        {
                            ints[outs1] = itog1;
                        }
                    break;
                    case "Ethernetconnection":
                        string server = game.Split('\n')[i + 1].Split(';')[1];
                        string login = game.Split('\n')[i + 2].Split(';')[1];
                        string passw = game.Split('\n')[i + 3].Split(';')[1];
                        string socet = game.Split('\n')[i + 4].Split(';')[1];
                        string gameputserv = game.Split('\n')[i + 5].Split(';')[1];

                        //MessageBox.Show(connecttoserver(server, login, passw, gameputserv, socet));
                        game += connecttoserver(server, login, passw, gameputserv, socet);                       
                        lenght = game.Split('\n').Length;
                        game = game.Replace("getcurrentdirect:", path1);
                    break;
                    case "ascllart":
                        var tmp = File.ReadAllText(game.Split('\n')[i].Split(';')[1].Split(',')[0]).Remove('\r', ' ');
                        string[] temp = tmp.Split('\n');
                        
                        int cola1 = Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split(',')[1].Split('-')[0]);
                        int colb1 = Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split(',')[1].Split('-')[1]);
                        for (int ic1 = 0; ic1 < (colb1 - cola1); ic1++)
                        {
                            Console.WriteLine(temp[cola1 + ic1]);
                        }
                    break;
                    case "include":                    
                        game += File.ReadAllText(game.Split('\n')[i].Split(';')[1]);                       
                        lenght = game.Split('\n').Length;
                        game = game.Replace("getcurrentdirect:", path1);
                    break;
                    case "playmusic":
                        SoundPlayer player = new System.Media.SoundPlayer();
                        player.SoundLocation = game.Split('\n')[i].Split(';')[1];
                        player.Play();
                    break;
                    case "sett":
                        rasm = Convert.ToBoolean(game.Split('\n')[i].Split(';')[1].Split(',')[0].Split(':')[1]);
                        autoelse = Convert.ToBoolean(game.Split('\n')[i].Split(';')[1].Split(',')[2].Split(':')[1]);
                        Console.Title = game.Split('\n')[i].Split(';')[1].Split(',')[1].Split(':')[1];
                        Console.BackgroundColor = (ConsoleColor)Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split(',')[3].Split(':')[1]);           
                    break;
                    case "goto":
                        i = Convert.ToInt32(game.Split('\n')[i].Split(';')[1]) - 2;
                    break;
                    case "close":
                        Application.Exit();
                    break;
                    case "pause":
                        Console.ReadLine();
                    break;
                    case "finish":
                        MessageBox.Show(game.Split('\n')[i].Split(';')[1]);
                        Console.WriteLine("Вы прошли игру");
                        Console.ReadLine();
                        Application.Restart();
                        break;
                    case "if":
                        string select = Console.ReadLine();                       
                        if (!rasm)
                        {
                            select = select.ToLower();
                        }
                       
                        string[] ifs = game.Split('\n')[i].Split(';')[1].Split(',');
                        for (int a = 0; a < ifs.Length; a++)
                        {
                            if(select == ifs[a])
                            {                                
                                List<string> thens = new List<string>();
                                for (int c = 1; c <= ifs.Length; c++){thens.Add(game.Split('\n')[i + c]);}
                                for (int b = 0; b < thens.Count; b++)
                                {
                                    /*string thb = thens[b].Split(':')[0];
                                    if (!rasm)
                                    {
                                        thb = thb.ToLower();
                                    }*/

                                    if (thens[b].Split(':')[0] == select)
                                    {
                                        if (thens[b].Split(':')[1].Split(';')[0] == "closegame")
                                        {
                                            //Console.WriteLine("CLOSEEEEEEEEEEE");                                       
                                            Console.WriteLine(thens[b].Split(':')[1].Split(';')[1]);
                                            System.Threading.Thread.Sleep(1500);
                                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                                            //break;
                                            //
                                        }
                                        else if (thens[b].Split(':')[1].Split(';')[0] == "messstring")
                                        {
                                            Console.WriteLine(strings[thens[b].Split(':')[1].Split(';')[1]]);
                                        }
                                        else if (thens[b].Split(':')[1].Split(';')[0] == "mess")
                                        {
                                            Console.WriteLine(thens[b].Split(':')[1].Split(';')[1]);
                                        }
                                        else if (thens[b].Split(':')[1].Split(';')[0] == "printimg")
                                        {
                                            string put = "";
                                            for (int i0 = 1; i0 < thens[b].Split(':').Length; i0++)
                                            {
                                                put += ":" + thens[b].Split(':')[i0];
                                            }
                                            Bitmap bmpSrc1 = new Bitmap(@put.Split(';')[1], true);
                                            ConsoleWriteImage(bmpSrc1);
                                        }
                                        else if (thens[b].Split(':')[1].Split(';')[0] == "setbackphoto")
                                        {
                                            string put = "";
                                            for (int i0 = 1; i0 < thens[b].Split(':').Length; i0++)
                                            {
                                                put += ":" + thens[b].Split(':')[i0];
                                            }
                                            printimage(put.Split(';')[1]);
                                        }
                                        else if (thens[b].Split(':')[1].Split(';')[0] == "playmusic")
                                        {
                                            string put = "";
                                            for (int i0 = 1; i0 < thens[b].Split(':').Length; i0++)
                                            {
                                                put += ":" + thens[b].Split(':')[i0];
                                            }
                                            SoundPlayer player1 = new System.Media.SoundPlayer();
                                            player1.SoundLocation = put.Split(';')[1];
                                            player1.Play();
                                        }
                                        else if (thens[b].Split(':')[1].Split(';')[0] == "ascllart")
                                        {                                       
                                            string put = "";
                                            for (int i0 = 1; i0 < thens[b].Split(':').Length; i0++)
                                            {
                                                put += ":" + thens[b].Split(':')[i0];
                                            } 
                                            var tmp2 = File.ReadAllText(put.Split(';')[1].Split(',')[0]).Remove('\r', ' ');
                                            string[] temp2 = tmp2.Split('\n');

                                            int cola12 = Convert.ToInt32(thens[b].Split(':')[2].Split(',')[1].Split('-')[0]);
                                            int colb12 = Convert.ToInt32(thens[b].Split(':')[2].Split(',')[1].Split('-')[1]);
                                            for (int ic1 = 0; ic1 < (colb12 - cola12); ic1++)
                                            {
                                                Console.WriteLine(temp2[cola12 + ic1]);
                                            }
                                        }
                                        else if (thens[b].Split(':')[1].Split(';')[0] == "goto")
                                        {
                                            i = Convert.ToInt32(thens[b].Split(':')[1].Split(';')[1]) - 2;
                                        }
                                        else if (thens[b].Split(':')[1].Split(';')[0] == "messas")
                                        {
                                            int cola = Convert.ToInt32(thens[b].Split(':')[1].Split(';')[1].Split('-')[0]);
                                            int colb = Convert.ToInt32(thens[b].Split(':')[1].Split(';')[1].Split('-')[1]);
                                            for (int ic = 0; ic < (colb - cola); ic++)
                                            {
                                                Console.WriteLine(game.Split('\n')[cola + ic].Split(';')[1]);
                                            }
                                        }
                                        //Console.WriteLine(thens[b].Split(':')[1].Split(';')[1]);
                                        i++;
                                        goto sled;
                                    }                                    
                                    //throw new Exception();
                                }                               
                            }
                            else if (select != ifs[a] && ifs[a] == "else")
                            {
                                List<string> thens = new List<string>();
                                for (int c = 1; c <= ifs.Length; c++){thens.Add(game.Split('\n')[i + c]);}
                                for (int b = 0; b < thens.Count; b++)
                                {
                                    if (thens[b].Split(':')[0] == "else")
                                    {
                                        if (autoelse)
                                        {
                                            Console.WriteLine("Не правильный ответ");
                                            System.Threading.Thread.Sleep(1500);
                                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                                        }
                                        else
                                        {
                                            if (thens[b].Split(':')[1].Split(';')[0] == "closegame")
                                            {
                                                Console.WriteLine(thens[b].Split(':')[1].Split(';')[1]);
                                                System.Threading.Thread.Sleep(1500);
                                                System.Diagnostics.Process.GetCurrentProcess().Kill();
                                            }
                                            else if (thens[b].Split(':')[1].Split(';')[0] == "mess")
                                            {
                                                Console.WriteLine(thens[b].Split(':')[1].Split(';')[1]);
                                            }
                                            else if (thens[b].Split(':')[1].Split(';')[0] == "goto")
                                            {
                                                b = Convert.ToInt32(thens[b].Split(':')[1].Split(';')[1]);
                                            }
                                            else if (thens[b].Split(':')[1].Split(';')[0] == "printimg")
                                            {
                                                string put = "";
                                                for (int i0 = 1; i0 < thens[b].Split(':').Length; i0++)
                                                {
                                                    put += ":" + thens[b].Split(':')[i0];
                                                }
                                                Bitmap bmpSrc1 = new Bitmap(@put.Split(';')[1], true);
                                                ConsoleWriteImage(bmpSrc1);                                             
                                            }
                                            else if (thens[b].Split(':')[1].Split(';')[0] == "setbackphoto")
                                            {
                                                string put = "";
                                                for (int i0 = 1; i0 < thens[b].Split(':').Length; i0++)
                                                {
                                                    put += ":" + thens[b].Split(':')[i0];
                                                }
                                                printimage(put.Split(';')[1]);
                                            }
                                            else if (thens[b].Split(':')[1].Split(';')[0] == "playmusic")
                                            {
                                                string put = "";
                                                for (int i0 = 1; i0 < thens[b].Split(':').Length; i0++)
                                                {
                                                    put += ":" + thens[b].Split(':')[i0];
                                                }
                                                SoundPlayer player1 = new System.Media.SoundPlayer();
                                                player1.SoundLocation = put.Split(';')[1];
                                                player1.Play();
                                            }
                                            else if (thens[b].Split(':')[1].Split(';')[0] == "ascllart")
                                            {
                                                string put = "";
                                                for (int i0 = 1; i0 < thens[b].Split(':').Length; i0++)
                                                {
                                                    put += ":" + thens[b].Split(':')[i0];
                                                }
                                                var tmp2 = File.ReadAllText(put.Split(';')[1].Split(',')[0]).Remove('\r', ' ');
                                                string[] temp2 = tmp2.Split('\n');

                                                int cola12 = Convert.ToInt32(thens[b].Split(':')[2].Split(',')[1].Split('-')[0]);
                                                int colb12 = Convert.ToInt32(thens[b].Split(':')[2].Split(',')[1].Split('-')[1]);
                                                for (int ic1 = 0; ic1 < (colb12 - cola12); ic1++)
                                                {
                                                    Console.WriteLine(temp2[cola12 + ic1]);
                                                }
                                            }
                                            else if (thens[b].Split(':')[1].Split(';')[0] == "messas")
                                            {
                                                int cola = Convert.ToInt32(thens[b].Split(':')[1].Split(';')[1].Split('-')[0]);
                                                int colb = Convert.ToInt32(thens[b].Split(':')[1].Split(';')[1].Split('-')[1]);
                                                for (int ic = 0; ic < (colb - cola); ic++)
                                                {
                                                    Console.WriteLine(game.Split('\n')[cola + ic].Split(';')[1]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                        }
                        break;
                }
                }
            
         }
            }
        }
