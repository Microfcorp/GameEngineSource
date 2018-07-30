using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gameSDK
{
    public partial class Form1 : Form
    {
        bool[] paramss = {false,false };
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void вывестиКьдToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Visible = !richTextBox1.Visible;
        }

        private void выводСообщенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "mess;" + toolStripTextBox2.Text + Environment.NewLine;
          
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("sett;razm:{0},title:{1},autoelse:{2},color:{3}",paramss[0].ToString(), toolStripTextBox1.Text, paramss[1].ToString(), toolStripTextBox3.Text) + Environment.NewLine;
        }

        private void даToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paramss[0] = true;
        }

        private void нетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paramss[0] = false;
        }

        private void даToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            paramss[1] = true;
        }

        private void нетToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            paramss[1] = false;
        }

        private void текущаяяПапкаСИгройToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "getcurrentdirect:";
        }

        private void результвтВКоманднуюСтрокуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "consoleprint";
        }

        private void aSCIIАртToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("ascllart;{0},{1}-{2}", toolStripTextBox4.Text, toolStripTextBox5.Text, toolStripTextBox6.Text) + Environment.NewLine;
        }

        private void rjyrwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "finish;" + toolStripTextBox7.Text + Environment.NewLine;
        }

        private void отчиститьКонсольToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "cls;" + Environment.NewLine;
        }

        private void кэшироватьФайлыССервераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("cashe;{0};{1}", toolStripTextBox8.Text, toolStripTextBox9.Text) + Environment.NewLine;
        }

        private void вывестиИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "printimg;" + toolStripTextBox10.Text + Environment.NewLine;
        }

        private void подключаемыейФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "include;" + toolStripTextBox11.Text + Environment.NewLine;
        }

        private void подключаемыйФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "includingas;" + Environment.NewLine;
        }

        private void вывестиТекстToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("messas;{0}-{1}", toolStripTextBox12.Text, toolStripTextBox13.Text) + Environment.NewLine;
        }

        private void подключаемыйТекстToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "messin;" + toolStripTextBox14.Text + Environment.NewLine;
        }

        private void паузаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "pause;" + Environment.NewLine;
        }

        private void числовуюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("int;{0}:{1}", toolStripTextBox15.Text, toolStripTextBox16.Text) + Environment.NewLine;
        }

        private void текстовуюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("int;{0}:{1}", toolStripTextBox17.Text, toolStripTextBox18.Text) + Environment.NewLine;
        }

        private void числовуюToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "messint;" + toolStripTextBox20.Text + Environment.NewLine;
        }

        private void текстовуюToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "messstring;"+ toolStripTextBox20.Text + Environment.NewLine;
        }

        private void числаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("let;{0}{1}{2}:{3}", toolStripTextBox21.Text, toolStripTextBox22.Text, toolStripTextBox23.Text, toolStripTextBox25.Text) + Environment.NewLine;
        }

        private void переменныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("intlet;{0};{1};{2}:{3}", toolStripTextBox26.Text, toolStripTextBox27.Text, toolStripTextBox28.Text, toolStripTextBox24.Text) + Environment.NewLine;
        }

        private void рандомноеЧислоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += String.Format("random;{0}-{1}:{2}", toolStripTextBox29.Text, toolStripTextBox30.Text, toolStripTextBox31.Text) + Environment.NewLine;
        }

        private void перейтиКСтрокеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "goto;" + toolStripTextBox32.Text;
        }

        private void сетевоеСоеденениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}
