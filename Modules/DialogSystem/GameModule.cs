using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemModule;
using DialogSystem;

namespace GameModule
{
    public class GameModule
    {
        public SortedList<string, Perses> Perses = new SortedList<string, Perses>();
        public void SetParam(CommandData data)
        {
            if (data.Name == "AddPers")
            {
                Perses.Add(data.Params["Name"].ToLower(), new Perses(data.Params["Name"].ToLower()));
            }
            else if (data.Name == "Step")
            {
                Console.WriteLine("Вы пойдете к ");
                foreach (var item in Perses)
                {
                    Console.Write(item.Value.Name + ", ");
                }
                string result = Console.ReadLine();
                Perses[result.ToLower()].NewDialog();
            }
        }
    }
    public class Perses
    {
        public enum Character
        {
            minus=0,
            neutr = 1,
            plus = 2,
        }
        public string Name
        {
            private set;
            get;
        }
        public int Otnos
        {
            private set;
            get;
        }
        public Perses(string name)
        {
            Name = name;
            Otnos = 0;
        }
        private int stadia = 0;
        public void NewDialog()
        {
            if (stadia == 0)
            {
                Console.WriteLine("Вы скажете: " + Environment.NewLine + ManegDialogMy.Helloy().ToString());
                int id1 = Convert.ToInt32(Console.ReadLine());
                Speak(ManegDialogNotMy.Helloy(), ManagederDialog.GetCharacter(id1, ManegDialogMy.Helloy().Type));

                Console.WriteLine("Вы скажете: " + Environment.NewLine + ManegDialogMy.PochemuNeMir().ToString());
                int id2 = Convert.ToInt32(Console.ReadLine());
                Speak(ManegDialogNotMy.PochemeNeMir(), ManagederDialog.GetCharacter(id2, ManegDialogMy.PochemuNeMir().Type));

                Console.WriteLine("Вы скажете: " + Environment.NewLine + ManegDialogMy.MozetMir().ToString());
                int id3 = Convert.ToInt32(Console.ReadLine());
                Speak(ManegDialogNotMy.MozetMir(), ManagederDialog.GetCharacter(id3, ManegDialogMy.MozetMir().Type));
                stadia = 1;
            }
            else
            {
                Console.WriteLine("Дальше диалоговая система не прароботана. Вы можете продожить ее разработку. Исходники в отдельном репозитории на GitHub. А я уже очень устал разрабатывать (поскольку делаю я это один), то очень прошу вас помочь мне с разработкой. Жду ваших крутейших библиотек. Всегда готов помочь по функционалу самого движка");
            }
        }
        public void Speak(Dialog fraz, Character Character)
        {
            
                if (Character == Character.minus)
                {
                    Otnos -= 1;

                    if (fraz.Type == "Helloy")
                    {
                          Console.WriteLine(ManegDialogNotMy.Helloy().Fraz[1]);
                    }

                if (fraz.Type == "MozetMir")
                {
                    Console.WriteLine(ManegDialogNotMy.MozetMir().Fraz[2]);
                }


            }
            if (Character == Character.plus)
                {
                    Otnos += 1;

                    if (fraz.Type == "Helloy")
                    {
                          Console.WriteLine(ManegDialogNotMy.Helloy().Fraz[0]);
                    }
                if (fraz.Type == "MozetMir")
                {
                    Console.WriteLine(ManegDialogNotMy.MozetMir().Fraz[0]);
                }
            }
                if (Character == Character.neutr)
                {
                    if (fraz.Type == "Helloy")
                    {
                          Console.WriteLine(ManegDialogNotMy.Helloy().Fraz[0]);
                    }
                if (fraz.Type == "PochemeNeMir")
                {
                    if (ManagederDialog.ChorosOtnos(Otnos))
                    {
                        Console.WriteLine(ManegDialogNotMy.PochemeNeMir().Fraz[0]);
                    }
                    else
                    {
                        Console.WriteLine(ManegDialogNotMy.PochemeNeMir().Fraz[1]);
                    }
                }
                if (fraz.Type == "MozetMir")
                {
                    Console.WriteLine(ManegDialogNotMy.MozetMir().Fraz[1]);
                }
            }
            Console.WriteLine("Ваши текущие отношения с этим персонажем " + Otnos.ToString()); 
            }
        }
    }
