using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace JPapp
{
    public partial class WordRemember : Form
    {
        int level=1;//注：level大于0
        Wordreading wordreading;
        int page = 1;
        //-------------------------------------------------
        public WordRemember()
        {
            InitializeComponent();
            ListBox1_init();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)//课文列表中变化则读取变化
        {
            radioButton1.Checked = false;//重置单选按钮（此处必须先重置单选按钮再重置level和page，因为重置单选按钮时会触发事件）
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            level = 1;//重置level
            page = 1;//重置page

            int i = listBox1.SelectedIndex+1;
            wordreading = new Wordreading(i);//创造课文n的类
            wordreading.tmp_level_init();
            wordreading.Read(level,1);//读取相应段的数据
            wordreading.Update(richTextBox1,level,page);//更新文本域中的数据
            button1.Enabled = false;
            button2.Enabled = false;
            label2.Text = "当前页数：1";
        }

        private void ListBox1_init()
        {
            listBox1.SelectionMode = SelectionMode.One;
            listBox1.Items.Add("class 1");
            listBox1.Items.Add("class 2");
            listBox1.Items.Add("class 3");
            listBox1.Items.Add("class 4");
            listBox1.Items.Add("class 5");
            listBox1.Items.Add("class 6");
            listBox1.Items.Add("class 7");
            listBox1.Items.Add("class 8");
            listBox1.Items.Add("class 9");
            listBox1.Items.Add("class 10");
            listBox1.Items.Add("class 11");
            listBox1.Items.Add("class 12");
            listBox1.Items.Add("class 13");
            listBox1.Items.Add("class 14");
            listBox1.Items.Add("class 15");
            listBox1.Items.Add("class 16");
            listBox1.Items.Add("class 17");
            listBox1.Items.Add("class 18");
            listBox1.Items.Add("class 19");
            listBox1.Items.Add("class 20");
            listBox1.Items.Add("class 21");
            listBox1.Items.Add("class 22");
            listBox1.Items.Add("class 23");
            listBox1.Items.Add("class 24");
            listBox1.Items.Add("class 25");
            wordreading = new Wordreading(1);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            page = 1;
            level = 2;
            label2.Text = "当前页数：1";
            wordreading.Update(richTextBox1, level, page);
            button1.Enabled = false;
            if (wordreading.getpageamount(level) > 1)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            page = 1;
            level = 3;
            label2.Text = "当前页数：1";
            wordreading.Update(richTextBox1, level, page);
            button1.Enabled = false;
            if (wordreading.getpageamount(level) > 1)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }
        
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            page = 1;
            level = 4;
            label2.Text = "当前页数：1";
            wordreading.Update(richTextBox1, level, page);
            button1.Enabled = false;
            if (wordreading.getpageamount(level) > 1)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            page = 1;
            level = 5;
            label2.Text = "当前页数：1";
            wordreading.Update(richTextBox1, level, page);
            button1.Enabled = false;
            if (wordreading.getpageamount(level) > 1)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            page--;
            label2.Text = "当前页数:" + page.ToString();
            wordreading.Update(richTextBox1, level, page);
            if (page < 2)
                button1.Enabled = false;
            else
                button1.Enabled = true;
            if (page >= wordreading.getpageamount(level))
                button2.Enabled = false;
            else
                button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            page++;
            label2.Text = "当前页数:" + page.ToString();
            wordreading.Update(richTextBox1, level, page);
            if (page < 2)
                button1.Enabled = false;
            else
                button1.Enabled = true;
            if (page >= wordreading.getpageamount(level))
                button2.Enabled = false;
            else
                button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    public class Wordreading 
    {
        int class_num;//课程编号1-n
        String[] temp;//用于存放从文件中读取的当前数据
        int temp_count;//缓冲区中数据数
        int read_add;//当前读取位置
        int[] tmp_level;//本文件段号起始位置，即##标号所在的行数
        int level_count;//检测到的段号数

        const int level_size = 6;//分段数
        const int buff_size = 20;//存储区大小
        const String file_path = @"..\..\..\src\";//文件路径
        String path = null;
        bool init=false;

        public Wordreading(int Class_num)//初始化
        {
            class_num = Class_num;
            temp = new String[buff_size];
            temp_count = 0;
            read_add = 0;
            tmp_level = new int[level_size];
            level_count = 0;
            path = file_path + "jp" + Class_num.ToString() + ".txt";
            init = true;
        }
        public void tmp_level_init()
        {
            FileStream fs = new FileStream(path,FileMode.Open);
            StreamReader sr = new StreamReader(fs,Encoding.Unicode);
            String tmp = null;
            int count=1;
            tmp = sr.ReadLine();
            while(tmp != null)
            {
                if (tmp.Contains("##"))
                {
                    //int i = tmp.IndexOf("##");
                    //int j = Convert.ToInt32(tmp.Substring(i+1, i + 2));
                    tmp_level[level_count] = count;
                    level_count++;
                }
                count++;
                tmp = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
        }
        public void Read(int level,int line)//读取第level段line行开始的后续内容直至存满缓冲区(包括line行)
            //其中line如果从0开始代表读入tag行（0类），如果从1开始代表仅读入内容行（1类）
        {
            //先重置缓冲区
            for (int n = 0; n < buff_size; n++)
            { temp[n] = ""; }
            temp_count = 0;
            //--------------------------------------
            FileStream fs = new FileStream(path,FileMode.Open);
            StreamReader sr = new StreamReader(fs,Encoding.Unicode);
            String tmp = null;
            int count = 0;
            read_add=tmp_level[level-1]+line;
            tmp = sr.ReadLine();
            while (tmp != null&&count<read_add-1)
            {
                tmp = sr.ReadLine();
                count++;
            }
            count = 0;
            while (tmp != null && count < buff_size)
            {
                temp[count] = tmp;
                temp_count++;
                count++;
                tmp = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
        }
        const int page_size = 10;//默认一页为十行数据
        public void Update(RichTextBox richtextbox,int level,int page)//更新文本域中的数据，level为段号，page为页号
        {
            int amount = tmp_level[level] - tmp_level[level-1] - 1;//该段内容长度
            int item_left = amount % page_size;//剩余条目数
            int page_num = amount / page_size;//完整页数
            int page_item=0;
            if (page <= page_num)
                page_item = page_size;
            else
                page_item = item_left;
                if (tmp_level[level-1] + (page - 1) * page_size >= read_add - 1 && tmp_level[level - 1] + (page - 1) * page_size+page_item < read_add + buff_size - 1)
                //如果数据在缓冲区中，则直接调出数据
                {
                    int start = tmp_level[level - 1] + (page - 1) * page_size - read_add + 1;//此处默认read函数是1类而不是0类的。
                    richtextbox.Clear();
                    for (int i = 0; i < page_item; i++)
                    {
                        richtextbox.AppendText(temp[start + i] + "\n");
                    }
                }
                else
                {
                    Read(level, (page - 1) * page_size + 1);
                    richtextbox.Clear();
                    for (int i = 0; i < page_item; i++)
                    {
                        richtextbox.AppendText(temp[i] + "\n");
                    }
                }
        }

        public int getpageamount(int level)
        {
            int amount = tmp_level[level] - tmp_level[level - 1] - 1;//该段内容长度
            return (amount / page_size+1);//完整页数
        }
    }
}
