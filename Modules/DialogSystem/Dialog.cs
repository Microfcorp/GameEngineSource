using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialogSystem
{
    public class Dialog
    {
        //Priv
        public List<string> Fraz = new List<string> ();
        public string Type
        {
            internal set;
            get;
        }

        public new string ToString()
        {
            string tmp = "";
            for (int i = 0; i < Fraz.Count; i++)
            {
                tmp += i.ToString() + ". " + Fraz[i] + Environment.NewLine;
            }
            return tmp;
        }
    }
    public class ManegDialogMy
    {
        public static Dialog Helloy()
        {
            Dialog d = new Dialog();
            d.Fraz = new List<string> {"Добрый день", "Хайки" };
            d.Type = "Helloy";
            return d;
        }
        public static Dialog PochemuNeMir()
        {
            Dialog d = new Dialog();
            d.Fraz = new List<string> { "Скажите пожалуйста, Почему вы воюете со своими соратниками", "Почему воюете со своими соратниками" };
            d.Type = "PochemeNeMir";
            return d;
        }
        public static Dialog MozetMir()
        {
            Dialog d = new Dialog();
            d.Fraz = new List<string> { "Может быть вы сможете помириться", "Почему вам не помириться", "Вы обязаны помириться" };
            d.Type = "MozetMir";
            return d;
        }
    }
    public class ManegDialogNotMy
    {
        public static Dialog Helloy()
        {
            Dialog d = new Dialog();
            d.Fraz = new List<string> { "Приветствую взаимно", "Возможно вам стоит поучиться хорошим манерам" };
            d.Type = "Helloy";
            return d;
        }
        public static Dialog PochemeNeMir()
        {
            Dialog d = new Dialog();
            d.Fraz = new List<string> { "Они все слишком жадные", "Это вообще не ваше дело" };
            d.Type = "PochemeNeMir";
            return d;
        }
        public static Dialog MozetMir()
        {
            Dialog d = new Dialog();
            d.Fraz = new List<string> { "Они мне задолжали по 10 рублей каждый", "Я не буду мириться", "Не лезте вообще в это дело" };
            d.Type = "MozetMir";
            return d;
        }
    }
    public class ManagederDialog
    {
        public static GameModule.Perses.Character GetCharacter(int id, string Type)
        {
            GameModule.Perses.Character ch = new GameModule.Perses.Character();
            if (Type == "Helloy")
            {
                if(id == 0)
                ch = GameModule.Perses.Character.plus;
                else if(id == 1)
                ch = GameModule.Perses.Character.minus;
            }
            if (Type == "PochemeNeMir")
            {
                    ch = GameModule.Perses.Character.neutr;
            }
            if (Type == "MozetMir")
            {
                if (id == 0)
                    ch = GameModule.Perses.Character.plus;
                else if (id == 1)
                    ch = GameModule.Perses.Character.neutr;
                else if (id == 2)
                    ch = GameModule.Perses.Character.minus;
            }
            return ch;
        }
        public static bool ChorosOtnos(int Otnos)
        {
            if (Otnos > 0) return true;
            else return false;
        }
    }
}
