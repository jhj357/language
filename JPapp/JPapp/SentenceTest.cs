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
    public partial class SentenceTest : Form
    {
        public UserClass us;
        RandomTest rt;
        int testtype = 3;
        const int TestCountLimit = 10;
        int Testcount = 0;//已经考察的题目数
        int TheTrueCount = 0;
        int ans = 0;
        int tidcount = 0;

        public SentenceTest()
        {
            InitializeComponent();
            init();
            rt = new RandomTest();
        }

        public SentenceTest(UserClass userclass)
        {
            InitializeComponent();
            init();
            this.us = userclass;
            rt = new RandomTest();
        }

        void init()
        {
            panel1.Visible = false;
            panel2.Visible = false;
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            //关闭手气不错
            button6.Enabled = false;
            button7.Enabled = true;
            button8.Visible = false;

            label3.Text = "系统已经就绪";
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            rt.writeSB();//在判断题目前必须要把记录存入到缓冲区中，方便后面保存记录
            const int Bid = 1;
            Testcount++;
            if (ans == rt.GetSource(Bid))//
            {
                TheTrueCount++;
            }
            RefreshCom();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rt.writeSB();
            const int Bid = 2;
            Testcount++;
            if (ans == rt.GetSource(Bid))
            {
                TheTrueCount++;
            }
            RefreshCom();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rt.writeSB();
            const int Bid = 3;
            Testcount++;
            if (ans == rt.GetSource(Bid))
            {
                TheTrueCount++;
            }
            RefreshCom();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            rt.writeSB();
            const int Bid = 4;
            Testcount++;
            if (ans == rt.GetSource(Bid))
            {
                TheTrueCount++;
            }
            RefreshCom();
        }

        private void button5_Click(object sender, EventArgs e)//start
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
                rt.set_config(-3, tcount);
                int j = rt.M_readinit(tidcount.ToString(), Convert.ToDateTime(date));
                if (j < 0)
                {
                    label3.Text = "读取记忆文件初始化失败";
                }
                else
                {
                    int k = rt.Buff_refresh();
                    if (k < 0) label3.Text = "刷新缓冲区失败";
                    else
                        RefreshCom();
                }
                start_config = true;
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
                    label3.Text = "数据库操作中发生错误，刷新Buff缓冲区失败";
                }
                else
                {
                    RefreshCom();
                    button6.Enabled = true;
                }
            }
            if (start_config)
            {
                button5.Enabled = false;
                button7.Enabled = true;
                panel1.Visible = true;
                panel2.Enabled = false;
                groupBox1.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)//手气不错
        {
            int i = rt.Buff_refresh();
            if (i != -1)
            {
                RefreshCom();
            }
            else
            {
                label3.Text = "刷新缓冲区的时候发生了错误";
            }
        }

        bool flg_close = false;
        private void button7_Click(object sender, EventArgs e)
        {
            if (Testcount > TestCountLimit - 1)
            {
                this.Close();
            }
            else
            {
                if (flg_close)
                {
                    label3.Text = "当前还有未完成的题目，再点一次退出。";
                    flg_close = false;
                }
                else
                    this.Close();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)//普通生成
        {
            if (radioButton1.Checked == true)
            {
                testtype = 3;
                Testcount = 0;
                TheTrueCount = 0;
                ans = 0;
                
                button5.Enabled = true;
                button6.Enabled = true;
                //button7.Enabled = true;
                panel2.Visible = false;
            }
            else
            { }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)//记忆生成
        {
            if (radioButton2.Checked == true)//不使用random类生成题目
            {
                testtype = -1;//选择记忆生成
                button6.Enabled = false;//禁止手气不错按钮
                Memory memory = new Memory(DateTime.Now);//生成当前时间
                string tidstring = "";
                int limit = memory.getTLimit();
                for (int i = 1; i < limit; i++)//获取所有符合记忆点的记录 i<limit
                {
                    memory.refresh(i);
                    string tmp1 = us.getMtid(i, "3", memory.getUp(), memory.getDown());
                    string tmp2 = us.getMtid(i, "4", memory.getUp(), memory.getDown());
                    tidstring += (tmp1 + tmp2);
                }
                int place = -1;
                place = tidstring.IndexOf("#");
                if (place >= 0)
                {
                    label3.Text = "请从列表中选择记录";
                    listBox1.Items.Clear();
                    panel2.Visible = true;
                    while (place > 0)//添加记录到listbox1中
                    {
                        string item_name = tidstring.Substring(0, place);
                        tidstring = tidstring.Substring(place + 1);
                        listBox1.Items.Add(item_name);
                        place = tidstring.IndexOf("#");
                    }
                    listBox1.SelectedIndex = 0;
                }
                else
                {
                    panel2.Visible = false;
                    label3.Text = "不存在适合此时记忆的记录！";
                    button5.Enabled = false;//禁止开始按钮
                }

            }
            else
            {
                panel2.Visible = false;
                button6.Enabled = false;
                //listBox1.Items.Clear();
            }
        }

        private void RefreshCom()
        {
            if (Testcount > TestCountLimit - 1)//测验已经结束
            {
                //产生别的信息（是否保存记录、重来、退出）
                //label2.Visible = false;
                panel1.Visible = false;
                button6.Enabled = false;
                rt.all_clear();//清空原有的信息
                label3.Text = "本次小测试已经完成，" + TestCountLimit.ToString() + "道题中，你一共对了" + TheTrueCount.ToString() + "道。\n是否保存记录用于记忆曲线生成？";
                button8.Visible = true;
            }
            else
            {
                //int i = randomtest.Buff_refresh();
                int j = rt.Sample_refresh();
                if (j >= 0)
                {
                    //刷新题目
                    string stmp = rt.GetSample(0);
                    if (stmp.Length > 16)
                        stmp = stmp.Substring(0, 16) +"\n"+stmp.Substring(16);
                    if (stmp.Length > 33)
                        stmp = stmp.Substring(0, 33) + "\n" + stmp.Substring(33);
                    label4.Text = "选择问号处的答案：\n" + stmp;
                    //刷新按钮组
                    button1.Text = rt.GetSample(1);
                    button2.Text = rt.GetSample(2);
                    button3.Text = rt.GetSample(3);
                    button4.Text = rt.GetSample(4);
                    //刷新答案
                    ans = rt.Get_ans();
                    //刷新提示
                    label3.Text = "当前进度为" + Testcount.ToString() + @"/" +
                        TestCountLimit.ToString();
                    /*+"本题答案为:"+ans+"\n"+"1-4的答案分别为"+
                        randomtest.GetSource(1) + "..." + randomtest.GetSource(2) + "..." + randomtest.GetSource(3) + "..." + randomtest.GetSource(4);*/
                }
                else
                {
                    label3.Text = "初始化题库的时候发生了错误";
                }
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
        //bool success_flag = false;
        private void button8_Click(object sender, EventArgs e)
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
                            label3.Text = "结果已经成功保存,时间为" + dt.ToString();
                            button8.Visible = false;
                            button5.Enabled = false;
                        }

                    }
                }
            }
            else//记忆曲线生成情况下使用
            {
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
                        label3.Text = "结果已经成功保存";
                        button8.Visible = false;
                        button5.Enabled = false;
                    }
                    //可以选择把前面的记录调整下
                }
            }
        }
    }
}
