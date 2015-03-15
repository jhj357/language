using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JPapp
{
    public partial class GrammarTest : Form
    {
        public UserClass us;
        RandomTest rt;
        //
        const string JPCharacter = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをんがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽ                                    ";
        const int JPCharacterNum = 46+25;
        const int JPpagesize = 12;
        const int JPpageLimit = JPCharacterNum % JPpagesize == 0 ? JPCharacterNum / JPpagesize : JPCharacterNum / JPpagesize+1;//6
        //
        int testtype = 5;
        const int TestCountLimit = 10;
        int Testcount = 0;//已经考察的题目数
        int TheTrueCount = 0;
        int ans = 1;
        int tidcount = 0;
        //
        public GrammarTest()//这个不用
        {
            InitializeComponent();
        }

        public GrammarTest(UserClass userclass)
        {
            InitializeComponent();
            this.us = userclass;
            rt = new RandomTest();
            init();
        }

        void init()
        {
            SetPage(1);
            listBox1.Visible=false;
            panel2.Visible = false;
            textBox1.Visible = false;
            //--------
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            //
            button23.Visible = false;
            //初始要禁用键盘输入
            panel2.Enabled = false;
        }

        
        bool _flg = false;//键盘是否开启
        private void button20_Click(object sender, EventArgs e)//开启和关闭键盘
        {
            if (_flg)
            {
                panel2.Visible = false; ;
                _flg = false;
                button20.Text = "开启键盘";
            }
            else
            {
                panel2.Visible = true;
                _flg = true;
                button20.Text = "关闭键盘";
            }

        }

        private void button21_Click(object sender, EventArgs e)//退格
        {
            int length = textBox1.Text.Length;
            if(length>0)
                textBox1.Text = textBox1.Text.Substring(0,length-1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text+= button1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text += button2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += button3.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text += button4.Text;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text += button5.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text += button6.Text;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text += button7.Text;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Text += button8.Text;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text += button9.Text;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox1.Text += button10.Text;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox1.Text += button11.Text;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Text += button12.Text;
        }
        //
        int JP_page = 1;
        //
        private void button15_Click(object sender, EventArgs e)//最前页
        {
            JP_page = 1;
            SetPage(JP_page);
        }

        private void button13_Click(object sender, EventArgs e)//前翻
        {
            if(JP_page>1){
            JP_page--;
            SetPage(JP_page);
            }
        }

        private void button14_Click(object sender, EventArgs e)//后翻
        {
            if (JP_page <JPpageLimit)
            {
                JP_page++;
                SetPage(JP_page);
            }
        }

        private void button16_Click(object sender, EventArgs e)//最后页
        {
            JP_page = JPpageLimit;
            SetPage(JP_page);
        }

        void SetPage(int page)
        {
                int start = (page - 1)*JPpagesize;
                button1.Text = JPCharacter.Substring(start++, 1).Trim();
                button2.Text = JPCharacter.Substring(start++, 1).Trim();
                button3.Text = JPCharacter.Substring(start++, 1).Trim();
                button4.Text = JPCharacter.Substring(start++, 1).Trim();
                button5.Text = JPCharacter.Substring(start++, 1).Trim();
                button6.Text = JPCharacter.Substring(start++, 1).Trim();
                button7.Text = JPCharacter.Substring(start++, 1).Trim();
                button8.Text = JPCharacter.Substring(start++, 1).Trim();
                button9.Text = JPCharacter.Substring(start++, 1).Trim();
                button10.Text = JPCharacter.Substring(start++, 1).Trim();
                button11.Text = JPCharacter.Substring(start++, 1).Trim();
                button12.Text = JPCharacter.Substring(start++, 1).Trim();
        }


        private void button22_Click(object sender, EventArgs e)//确认键
        {
            if (!textBox1.Text.Trim().Equals(""))
            {
                rt.writeSB();
                Testcount++;
                string answer = rt.GetSample(ans);
                if (answer.Equals(textBox1.Text))
                {
                    TheTrueCount++;
                }
                textBox1.Clear();
                RefreshCom();
            }
            else
                label4.Text = "输入框为空，请输入答案！";
        }

        bool flg_close = false;
        private void button19_Click(object sender, EventArgs e)//close
        {
            if (Testcount > TestCountLimit - 1)
            {
                this.Close();
            }
            else
            {
                if (flg_close)
                {
                    label4.Text = "当前还有未完成的题目，再点一次退出。";
                    flg_close = false;
                }
                else
                    this.Close();
            }
        }

        private void button18_Click(object sender, EventArgs e)//运气不错
        {
            int i = rt.Buff_refresh();
            if (i != -1)
            {
                RefreshCom();
            }
            else
            {
                label4.Text = "刷新缓冲区的时候发生了错误";
            }
        }

        private void button17_Click(object sender, EventArgs e)//start
        {
            bool start_config = false;
            if (testtype == -1)//记忆曲线生成,但不是以-1为参数生成randomtest类的
            {
                string tmp = "##";
                tmp = us.Read("Test", "tid= '" + tidcount.ToString() + "'", 2, "tdatetime-tamount");
                int i = tmp.IndexOf("#");
                string date = tmp.Substring(0, i);
                tmp = tmp.Substring(i + 1);
                i = tmp.IndexOf("#");
                int tcount = Convert.ToInt32(tmp.Substring(0, i));
                rt.set_config(-5, tcount);
                int j = rt.M_readinit(tidcount.ToString(), Convert.ToDateTime(date));
                if (j < 0)
                {
                    label4.Text = "读取记忆文件初始化失败";
                }
                else
                {
                    int k = rt.Buff_refresh();
                    if (k < 0) label4.Text = "刷新缓冲区失败";
                    else
                    {
                        RefreshCom();
                        start_config = true;
                    }
                }
            }
            else
            {
                if (testtype > 0 && TestCountLimit > 0)
                    rt.set_config(testtype, TestCountLimit);
                //int i=randomtest.Buff_refresh();缓冲区刷新只要开始的一次即可
                //int j = randomtest.Sample_refresh();把题目刷新操作放在refresh中
                start_config = true;
                int i = rt.Buff_refresh();
                if (i < 0)
                {
                    label4.Text = "数据库操作中发生错误，刷新Buff缓冲区失败";
                }
                else
                {
                    RefreshCom();
                    button18.Enabled = true;
                }
            }
            if (start_config)
            {
                start_config = true;
                button17.Enabled = false;
                //listBox1.Visible = true;
                groupBox1.Enabled = false;
                panel2.Enabled = true;
                textBox1.Visible = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                testtype = 5;
                TheTrueCount = 0;
                Testcount = 0;
                ans = 0;

                button17.Enabled = true;
                //button18.Enabled = true;
            }
            else
            {
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                testtype = -1;//选择记忆生成
                button18.Enabled = false;//禁止手气不错按钮
                Memory memory = new Memory(DateTime.Now);//生成当前时间
                string tidstring = "";
                int limit = memory.getTLimit();
                for (int i = 1; i < limit; i++)//获取所有符合记忆点的记录 i<limit
                {
                    memory.refresh(i);
                    string tmp1 = us.getMtid(i, "5", memory.getUp(), memory.getDown());
                    tidstring += tmp1;
                }
                int place = -1;
                place = tidstring.IndexOf("#");
                if (place >= 0)
                {
                    label2.Text = "请从列表中选择记录";
                    listBox1.Items.Clear();
                    listBox1.Visible = true;
                    while (place > 0)//添加记录到listbox1中
                    {
                        string item_name = tidstring.Substring(0, place);
                        tidstring = tidstring.Substring(place + 1);
                        listBox1.Items.Add(item_name);
                        place = tidstring.IndexOf("#");
                    }
                    listBox1.SelectedIndex = 0;
                    tidcount = Convert.ToInt32(listBox1.GetItemText(listBox1.SelectedItem));
                }
                else
                {
                    listBox1.Visible = false;
                    label4.Text = "不存在适合此时记忆的记录！";
                    button17.Enabled = false;//禁止开始按钮
                }
            }
            else
            {
                listBox1.Visible = false;
                button18.Enabled = false;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (testtype == -1)
            {
                try
                {
                    tidcount = Convert.ToInt32(listBox1.GetItemText(listBox1.SelectedItem));
                }
                catch (Exception e1)
                {
                    tidcount = 0;
                }
            }
        }


        private void RefreshCom()
        {
            if (Testcount > TestCountLimit - 1)//测验已经结束
            {
                //产生别的信息（是否保存记录、重来、退出）                
                textBox1.Visible = false;
                button18.Enabled = false;
                rt.all_clear();//清空原有的信息
                label4.Text = "本次小测试已经完成，" + TestCountLimit.ToString() + "道题中，你一共对了" + TheTrueCount.ToString() + "道。\n是否保存记录用于记忆曲线生成？";
                button23.Visible = true;
                panel2.Enabled = false;
                label3.Text = "";
                //textBox1.Visible = false;
            }
            else
            {
                //int i = randomtest.Buff_refresh();
                int j = rt.Sample_refresh();
                if (j >= 0)
                {
                    //刷新答案
                    ans = rt.Get_ans();
                    string style = "";
                    switch(ans)
                    {
                        case 0: style = "error"; break;
                        case 1: style = "ます形"; break;
                        case 2: style = "て形"; break;
                        case 3: style = "ない形"; break;
                        case 4: style = "た形"; break;
                        default: break;
                    }
                    //刷新题目
                    label3.Text = "写出对应动词的"+style+"：\n" + rt.GetSample(0);
                    
                    //刷新提示
                    label4.Text = "当前进度为" + Testcount.ToString() + @"/" +
                        TestCountLimit.ToString();
                }
                else
                {
                    label4.Text = "初始化题库的时候发生了错误";
                }
            }
        }

        private void button23_Click(object sender, EventArgs e)//保存题目
        {
            if (!rt.getMode())
            {
                int control1 = -1;
                control1 = us.getAmount("Link");//获取已有tid数
                if (control1 < 0)
                {
                    Console.WriteLine("数据库操作出现错误，统计计数为负值");
                    return;
                }
                else
                {
                    control1++;//获得新tid 
                    String this_tid = control1.ToString();
                    DateTime dt = DateTime.Now;
                    int control2 = rt.product_log(this_tid, dt);
                    if (control2 < 0)
                    {
                        Console.WriteLine("文件记录产生失败：product_log()");
                        return;
                    }
                    else
                    {
                        //success_flag = true;
                        Console.WriteLine("文件已成功产生,时间为" + dt.ToString());
                        //link
                        int c1 = us.pWriteLink(this_tid, 1);
                        if (c1 < 0) Console.WriteLine("WriteLink出错");
                        //grade
                        int c2 = us.pWriteGrade(this_tid, 0, (int)(100 * TheTrueCount / Testcount));
                        if (c2 < 0) Console.WriteLine("WriteGrade出错");
                        //test
                        int c3 = us.pWriteTest(this_tid, dt, testtype.ToString(), Testcount);
                        if (c3 < 0) Console.WriteLine("WriteText出错");
                        if (c1 > 0 && c2 > 0 && c3 > 0)
                        {
                            label4.Text = "结果已经成功保存,时间为" + dt.ToString()+"\n";
                            Memory m = new Memory(dt);
                            label4.Text += "下一次记忆点："+m.getMemoryDate(1).ToString();
                            button23.Visible = false;
                            button17.Enabled = false;
                        }

                    }
                }
            }
            else//记忆曲线生成情况下使用
            {
                if ((100 * TheTrueCount / Testcount) < 60)
                {
                    label4.Text = "成绩太差了哦，你确定你有用心记了吗？\n结果不予保存。重做吧。";
                }
                string info = us.Read("Link", "tid='" + tidcount.ToString() + "' and visible=1 ", 1, "nextlevel");
                int p = info.IndexOf("#");
                if (p >= 0)
                {
                    info = info.Substring(0, p);
                    int level = Convert.ToInt32(info);
                    int c1 = us.pUpdateLink(tidcount.ToString(), level + 1);
                    int c2 = us.pWriteGrade(tidcount.ToString(), level, 100 * TheTrueCount / Testcount);
                    if (c1 > 0 && c2 > 0)
                    {
                        label4.Text = "结果已经成功保存\n";
                        button23.Visible = false;
                        button17.Enabled = false;
                    }
                    //可以选择把前面的记录调整下
                }
            }
        }
        
    }
}
