﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Reflection;
using SystemModule;
using System.Diagnostics;

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
                var s = Console.ReadLine();
                int n = 0;

                if (int.TryParse(s, out n))
                {
                    if (getfullgame().Length < n || getfullgame().Length >= 0)
                    {
                        loadgame(Environment.CurrentDirectory + "\\games\\" + getfullgame()[n]);
                    }
                }
            }
            else if (args[0] != null)
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
        /*
         * 
         * Я разрабатываю свой мини-интерпретатор (не спрашивайте зачем), так вот у меня возникла проблема: "Препретатор"(как я его назвал) в самом начале обходит весь файл с кодом и ищет по ключевому слову "function" функции, а затем добавляет их в список в виде структуры (указывается номер строки её начала). Далее, в основном цикле, когда доходит очередь до строки вызова функции, "постпретатор" берёт структуру из списка и добавляет в неё номер строки инициализатора этой функции, затем оно перед символом завершения функции ("}") оно пишет команду вида "goto;8" и затем переводит номер строки выполнения на номер строку начала функции, затем после выполнения функции оно должно перевести номер строки выполнения обратно в основной цикл. Но функция может находится в любой части скрипта, поэтому и был придуман "препретатор". У меня же с одной функцией, всё работает, а вот с двумя уже не хочет: оно почему-то не подставляет команду перевода номера строки в первую функцию, а во вторую подставляет нормально.
         * 
         * 
         * */

         /// <summary>
         /// Структура функции
         /// </summary>
        struct Function
        {
            public int start;
            public int point;
            public string name;
        }

        /// <summary>
        /// Список структур функции
        /// </summary>
        static SortedList<string, Function> funct = new SortedList<string, Function>();

        /// <summary>
        /// Препретатор и постпретатор функция
        /// </summary>
        /// <param name="game">Массив строк всего скрипта</param>
        /// <param name="main">Сам скрипт</param>
        public static void Compilation(string[] game, ref string main, string namefunc = null)
        {
            //funct.Clear();
            for (int i = 0; i < game.Length; i++)
            {
                game = main.Split('\n');
                if (game[i].Split(';')[0] == "function")
                {
                    
                    //создание или "взятие" функции
                    Function fn;
                    if(namefunc != null)
                    {
                        fn = funct[namefunc]; //получем функцию из списка по её имени
                    }
                    else if (!funct.ContainsKey(game[i].Split(';')[1]))
                    {
                        fn = new Function();
                        fn.start = i + 1;
                        string name = game[i].Split(';')[1];
                        fn.name = name;
                        funct.Add(name, fn);
                    }
                    else
                    {
                        fn = funct[game[i].Split(';')[1]];
                    }

                    //Подсчёт строк функции
                    int o = 1;
                    for (; !game[i + o].TrimEnd('\n', '\r', '\t').EndsWith("}"); o++) ;

                    var rrrhhh = ""; //временная переменная с итоговым кодом скрипта
                    var telo = "";//тело функции
                    for (int p = 0; p < ((i + o)); p++)
                    {
                        rrrhhh += game[p] + "\n";  //заполнение до конца функции                      
                    }

                    //Console.WriteLine(rrrhhh);

                    for (int q = i; q < i+o; q++)
                    {
                        telo += game[q] + "\n"; //заполнение тела
                    }
                    telo = telo.Split('\n')[telo.Split('\n').Length - 2];//обрезка } (там 2 должно быть, потому что индекс с 0, а количество с 1)

                    //Console.WriteLine(telo);
                    int lenghtgoto = 0;
                    if (!telo.Contains("goto;")){
                        rrrhhh = rrrhhh.Insert(rrrhhh.Length, String.Format("goto;{0}", fn.point) + "\n}");//если "гото" нет, то добавляем                        
                        lenghtgoto = (String.Format("goto;{0}", fn.point) + "\n}").Length; //вычисляем длинну вставки
                    }
                    else//иначе изменяем
                    {
                        rrrhhh = rrrhhh.Substring(0, rrrhhh.Length - 3);//обрезаем перенос строки и }
                        int y = rrrhhh.Length - 1;//получаем длинну в виде индекса
                        int t = 0;//создает временную переменную
                        for (; rrrhhh[y] == ';'; y--, t++) ;//считаем от конца и до ; (это символ передачи аргументов)
                        //t++;
                        rrrhhh = rrrhhh.Substring(0, rrrhhh.Length - t);//обрезаем по него
                        rrrhhh += ";" + fn.point.ToString() + "\n}";//добавляем новый "гото"
                        //Console.WriteLine(rrrhhh);
                    }
                    rrrhhh = rrrhhh.Insert(rrrhhh.Length, main.Substring(rrrhhh.Length - (lenghtgoto))); //Добавляем всё что после функции
                    //Console.WriteLine(rrrhhh);
                    main = rrrhhh; //изменяем основной скрипт                                    
                }

            }
        }   
        
        private static void RenameVar(ref SortedList<string, string> list, ref string gameS)
        {
            string[] game = gameS.Split('\n');
            for (int i = 0; i < game.Length; i++)
            {
                game = gameS.Split('\n');
                foreach (var item in list)
                {
                    if (gameS.Contains("$" + item.Key))
                        gameS = gameS.Replace("$" + item.Key, item.Value);
                }
            }
            Console.WriteLine(gameS);
        }
        private static void RenameVar(ref SortedList<string, int> list, ref string gameS)
        {
            string[] game = gameS.Split('\n');
            for (int i = 0; i < game.Length; i++)
            {
                game = gameS.Split('\n');
                foreach (var item in list)
                {
                    if (gameS.Contains("$" + item.Key))
                        gameS = gameS.Replace("$" + item.Key, item.Value.ToString());
                }
            }
            Console.WriteLine(gameS);
        }

        public static void loadgame(string path)
        {
            path = path.TrimEnd('\r', '\t');
            string path1 = path.Replace(path.Split('\\')[path.Split('\\').Length - 1], "") + "\\";
            string game = File.ReadAllText(path).Replace("\r", "").Replace("getcurrentdirect:", path1).Replace("currentname:", Environment.UserName).Replace("newline:", Environment.NewLine);
            bool rasm = false;
            bool autoelse = false;
            int lenght = game.Split('\n').Length;
            SortedList<string, string> strings = new SortedList<string, string>();
            SortedList<string, int> ints = new SortedList<string, int>();
            bool loadmodule = false;

            SortedList<string, Assembly> asm = new SortedList<string, Assembly>();
            SortedList<string, Type> t = new SortedList<string, Type>();
            // создаем экземпляр класса Program
            SortedList<string, object> obj = new SortedList<string, object>();
            // получаем метод GetResult
            SortedList<string, MethodInfo> method = new SortedList<string, MethodInfo>();

            //Начало препретатора
            Console.WriteLine("Start Compilation");
            Compilation(game.Split('\n'), ref game);
            lenght = game.Split('\n').Length;
            Console.WriteLine("Stop Compilation");
            //конец препретатора

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
                    case "newmodule":
                        try
                        {
                            string name = game.Split('\n')[i].Split(';')[1];
                            asm[name] = Assembly.LoadFrom(game.Split('\n')[i].Split(';')[2]);
                            t[name] = asm[name].GetType("GameModule.GameModule", true, true);
                            // создаем экземпляр класса Program
                            obj[name] = Activator.CreateInstance(t[name]);
                            // получаем метод GetResult
                            method[name] = t[name].GetMethod("SetParam");
                            // вызываем метод, передаем ему значения для параметров и получаем результат
                            loadmodule = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error " + ex.Message);
                        }
                        break;
                    case "module":
                        if (loadmodule)
                        {
                            string name = game.Split('\n')[i].Split(';')[1];

                            SortedList<string, string> list1 = new SortedList<string, string>();
                            foreach (var item in game.Split('\n')[i].Split(';')[2].Split(':')[1].Split(','))
                            {
  
                                    list1.Add(item.Split('=')[0], item.Split('=')[1]);
                            }

                            SystemModule.CommandData tmpaa = new CommandData(game.Split('\n')[i].Split(';')[2].Split(':')[0], list1);
                            object result = method[name].Invoke(obj[name], new object[] { tmpaa });
                            //Console.WriteLine((result));
                        }
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
                        RenameVar(ref strings, ref game);
                        lenght = game.Split('\n').Length;//пересчёт новой общей длинны скрипта
                        break;
                    case "int":
                        ints.Add(game.Split('\n')[i].Split(';')[1].Split(':')[0], Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split(':')[1]));
                        RenameVar(ref ints, ref game);
                        lenght = game.Split('\n').Length;//пересчёт новой общей длинны скрипта
                        break;
                    case "Applications":
                        if (game.Split('\n')[i].Split(';')[1] == "Start")
                        {
                            Process.Start(game.Split('\n')[i].Split(';')[2], game.Split('\n')[i].Split(';')[3]);
                        }
                        else if (game.Split('\n')[i].Split(';')[1] == "Resize")
                        {                           
                            Console.SetBufferSize(int.Parse(game.Split('\n')[i].Split(';')[2]), int.Parse(game.Split('\n')[i].Split(';')[3]));
                        }
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
                        SortedList<string, string> EtherConnect = new SortedList<string, string>();
                        for (int o = 1; !game.Split('\n')[i + o].EndsWith("}"); o++)
                        {
                            EtherConnect.Add(game.Split('\n')[i + o].Split(';')[0].ToLower(), game.Split('\n')[i + o].Split(';')[1]);
                        }

                        string server = EtherConnect["server"];
                        string login = EtherConnect["login"];
                        string passw = EtherConnect["password"];
                        string socet = EtherConnect["socet"];
                        string gameputserv = EtherConnect["game"];

                        //MessageBox.Show(connecttoserver(server, login, passw, gameputserv, socet));
                        game += connecttoserver(server, login, passw, gameputserv, socet);
                        lenght = game.Split('\n').Length;
                        game = game.Replace("getcurrentdirect:", path1).Replace("currentname:", Environment.UserName).Replace("newline:", Environment.NewLine);
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
                        var rrrhhh = "";
                        for (int p = 0; p < i + 1; p++)
                        {
                            rrrhhh += game.Split('\n')[p] + "\n";
                        }

                        game = (game.Insert(rrrhhh.Length, File.ReadAllText(game.Split('\n')[i].Split(';')[1]) + "\n"));
                        lenght = game.Split('\n').Length;
                        game = game.Replace("getcurrentdirect:", path1).Replace("currentname:", Environment.UserName).Replace("newline:", Environment.NewLine);
                        break;
                    case "playmusic":
                        SoundPlayer player = new System.Media.SoundPlayer();
                        player.SoundLocation = game.Split('\n')[i].Split(';')[1];
                        player.Play();
                        break;
                    case "crash":
                        throw new Exception("Users Crash");                       
                    case "sett":
                        SortedList<string, string> Sett = new SortedList<string, string>();
                        foreach (var item in game.Split('\n')[i].Split(';')[1].Split(','))
                        {
                            Sett.Add(item.Split(':')[0], item.Split(':')[1]);
                        }
                        rasm = Convert.ToBoolean(Sett["razm"]);
                        autoelse = Convert.ToBoolean(Sett["autoelse"]);
                        Console.Title = Sett["title"];
                        Console.BackgroundColor = (ConsoleColor)Convert.ToInt32(Sett["color"]);                        
                        break;
                    case "goto":
                        i = Convert.ToInt32(game.Split('\n')[i].Split(';')[1]) - 2;
                        //goto sled;
                        break;
                    case "func":
                        //func;test
                        Function fc = funct[game.Split('\n')[i].Split(';')[1]]; //получение из аргумента имени функции                      
                        fc.point = i+2; //передача текущего номера строки
                        funct[game.Split('\n')[i].Split(';')[1]] = fc; //замена функции в списке
                        Compilation(game.Split('\n'), ref game, game.Split('\n')[i].Split(';')[1]); //запуск постпретатора
                        //Compilation(game.Split('\n'), ref game);
                        lenght = game.Split('\n').Length;//пересчёт новой общей длинны скрипта
                        i = fc.start;//передача управления другой строке
                        break;
                    case "close":
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        break;
                    case "removestr":
                        //lenght -= Convert.ToInt32(game.Split('\n')[i].Split(';')[1]);
                        game.Split('\n')[Convert.ToInt32(game.Split('\n')[i].Split(';')[1])] = "//";
                        break;
                    case "pause":
                        Console.WriteLine(game.Split('\n')[i].Split(';')[1]);
                        Console.ReadLine();
                        break;
                    case "cryptogam":
                        string cryptopath = "";
                        string cryptotopath = "";
                        cryptotopath = (@game.Split('\n')[i].Split(';')[2]);
                        Crypting.Main.EncryptFile(cryptopath, cryptotopath);
                        break;
                    case "encryptogam":
                        string cryptopath1 = "";
                        string cryptotopath1 = "";
                        cryptotopath1 = (@game.Split('\n')[i].Split(';')[2]);
                        Crypting.Main.DecryptFile(cryptopath1, cryptotopath1);
                        break;
                    case "closegame":
                        Console.WriteLine(game.Split('\n')[i].Split(';')[1]);
                        System.Threading.Thread.Sleep(1500);
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        break;
                    case "messas":
                        int cola = Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split('-')[0]);
                        int colb = Convert.ToInt32(game.Split('\n')[i].Split(';')[1].Split('-')[1]);
                        for (int ic = 0; ic < (colb - cola); ic++)
                        {
                            Console.WriteLine(game.Split('\n')[cola + ic].Split(';')[1]);
                        }
                        break;
                    case "finish":
                        MessageBox.Show(game.Split('\n')[i].Split(';')[1]);
                        Console.WriteLine("Вы прошли игру");
                        Console.ReadLine();
                        Application.Restart();
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        break;
                    case "if":
                        string select = Console.ReadLine();
                        if (!rasm)
                        {
                            select = select.ToLower();
                        }

                        string[] ifs = game.Split('\n')[i].Split(';')[1].Split(',');
                        for (int p = 0; p < ifs.Length; p++)
                        {
                            if (ifs[p].StartsWith("$"))
                            {
                                ifs[p] = ifs[p].Substring(1).Replace(ifs[p].Substring(1), strings[ifs[p].Substring(1)]);
                            }
                        }
                        for (int a = 0; a < ifs.Length; a++)
                        {
                            if (select == ifs[a])
                            {
                                List<string> thens = new List<string>();
                                for (int c = 1; c <= ifs.Length; c++) { thens.Add(game.Split('\n')[i + c]); }

                                for (int b = 0; b < thens.Count; b++)
                                {
                                    if (thens[b].StartsWith("$"))
                                    {
                                        string newstr = thens[b].Split(':')[0].Substring(1).Replace(thens[b].Split(':')[0].Substring(1), strings[thens[b].Split(':')[0].Substring(1)]);

                                        for (int u = 1; u < thens[b].Split(':').Length; u++)
                                        {
                                            newstr += ":" + thens[b].Split(':')[u];
                                        }
                                        thens[b] = newstr;
                                    }

                                    if (thens[b].Split(':')[0] == select)
                                    {
                                        game += String.Format("\nclosegame;\n{1}\ngoto;{0}", (i + 2).ToString(), thens[b].Split(':')[1]);
                                        lenght = game.Split('\n').Length + 1;
                                        i = game.Split('\n').Length - 2;


                                        //i++;
                                        goto sled;

                                    }
                                }
                            }
                            else if (select != ifs[a] && ifs[a] == "else")
                            {
                                List<string> thens = new List<string>();
                                for (int c = 1; c <= ifs.Length; c++) { thens.Add(game.Split('\n')[i + c]); }
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
                                            game += String.Format("\nclosegame;\n{1}\ngoto;{0}", (i + 2).ToString(), thens[b].Split(':')[1]);
                                            lenght = game.Split('\n').Length + 1;
                                            i = game.Split('\n').Length - 2;


                                            //i++;
                                            goto sled;
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