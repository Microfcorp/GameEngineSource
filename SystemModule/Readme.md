# Гайд по написанию модуля:

Все модули пишутся на языке C# и представляют сабой библиотеку классов (dll) с основным пространством имён GameModule и основным классном GameModule и основным методом
SetParam, с принимаемым классом CommandData. Обязательно подключение библиотеку SystemModule.

Базовый пример
````C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemModule;

namespace GameModule
{
    public class GameModule
    {
        public void SetParam(CommandData data)
        {
            Console.Write(data.Name) //Выводит имя команды
            Console.Write(data.Params["Age"]) //Выводит значение парамера Age
        }
    }
````
