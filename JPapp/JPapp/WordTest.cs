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
    public partial class WordTest : Form
    {
        //用于写记录的继承父类
        public UserClass us;
        //-------------------
        RandomTest randomtest;
        int testtype = 1;//考查类型
        int classnum = 1;//考察课文号
        const int TestCountLimit = 10;//单次考察上限
        int Testcount = 0;//用于已经考察的题目的计数
        int TheTrueCount = 0;//正确数
        int ans=0;//本次正确答案
        //const int buttonnum = 4;
        int tidcount;
        //-------------------
        public WordTest()
        {
            InitializeComponent();
            init_deal();
            randomtest = new RandomTest();
        }

        public WordTest(UserClass userclass)
        { 
            this.us = userclass;
            InitializeComponent();
            init_deal();
            randomtest = new RandomTest();
        }
        void init_deal()
        {
            //初始化单选按钮
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            //初始化某些按钮
            button6.Enabled = false;
            //设置listbox中的模式
            listBox1.SelectionMode = SelectionMode.One;
            
            //初始化容器
            panel3.Visible = false;
            //初始备用的tid
            tidcount = 0;
        }
        private void button1_Click(object sender, EventArgs e)//在判断题目前必须要把记录存入到缓冲区中
        {
            randomtest.writeSB();//在判断题目前必须要把记录存入到缓冲区中，方便后面保存记录
            const int Bid=1;
            Testcount++;
            if (ans == randomtest.GetSource(Bid))//
            {
                TheTrueCount++;
            }
            RefreshCom();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            randomtest.writeSB();
            const int Bid = 2;
            Testcount++;
            if (ans == randomtest.GetSource(Bid))
            {
                TheTrueCount++;
            }
            RefreshCom();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            randomtest.writeSB();
            const int Bid = 3;
            Testcount++;
            if (ans == randomtest.GetSource(Bid))//
            {
                TheTrueCount++;
            }
            RefreshCom();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            randomtest.writeSB();
            const int Bid = 4;
            Testcount++;
            if (ans == randomtest.GetSource(Bid))//
            {
                TheTrueCount++;
            }
            RefreshCom();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //关闭课文选择框显示
            if (radioButton1.Checked == true)//改变后选中
            {
                //相应变量初始化
                testtype = 1;
                classnum = 1;
                Testcount = 0;
                TheTrueCount = 0;
                ans = 0;
                panel1.Visible = true;
                listBox1.Items.Clear();
                listBox1.Items.Add("class1");
                listBox1.Items.Add("class2");
                listBox1.Items.Add("class3");
                listBox1.Items.Add("class4");
                listBox1.Items.Add("class5");
                listBox1.Items.Add("class6");
                listBox1.Items.Add("class7");
                listBox1.Items.Add("class8");
                listBox1.Items.Add("class9");
                listBox1.Items.Add("class10");
                listBox1.Items.Add("class11");
                listBox1.Items.Add("class12");
                listBox1.Items.Add("class13");
                listBox1.Items.Add("class14");
                listBox1.Items.Add("class15");
                listBox1.Items.Add("class16");
                listBox1.Items.Add("class17");
                listBox1.Items.Add("class18");
                listBox1.Items.Add("class19");
                listBox1.Items.Add("class20");
                listBox1.Items.Add("class21");
                listBox1.Items.Add("class22");
                listBox1.Items.Add("class23");
                listBox1.Items.Add("class24");
                listBox1.Items.Add("class25");
                listBox1.SelectedIndex = 0;
                button5.Enabled = true;
                //button6.Enabled = true;
                button7.Enabled = true;
            }
            else//改变后未选中
            {
                panel1.Visible = false;
                //listBox1.Items.Clear();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                testtype = 2;
                button5.Enabled = true;
                //button6.Enabled = true;
                button7.Enabled = true;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)//不使用random类生成题目
            {
                testtype = -1;//选择记忆生成
                button6.Enabled = false;//禁止手气不错按钮
                Memory memory = new Memory(DateTime.Now);//生成当前时间
                string tidstring = "";
                int limit = memory.getTLimit();
                for (int i = 1; i < limit; i++)//获取所有符合记忆点的记录 i<limit
                {
                    memory.refresh(i);
                    string tmp1 = us.getMtid(i,"1",memory.getUp(),memory.getDown());
                    string tmp2 = us.getMtid(i,"2",memory.getUp(),memory.getDown());
                    tidstring+=(tmp1+tmp2);
                }
                int place=-1;
                place = tidstring.IndexOf("#");
                if (place >= 0)
                {
                    label4.Text = "请从列表中选择记录";
                    listBox1.Items.Clear();
                    panel1.Visible = true;
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
                    panel1.Visible = false;
                    label4.Text = "不存在适合此时记忆的记录！";
                    button5.Enabled = false;//禁止开始按钮
                }

            }
            else
            {
                panel1.Visible = false;
                //listBox1.Items.Clear();
            }
        }

        private void button5_Click(object sender, EventArgs e)//start按钮
            //start后需要把考察模式部分给禁用掉，包括课文选择，还有记得测验结束时记录测验的日期和时间
        {
            bool start_config = false;
            if (testtype == -1)//记忆曲线生成,但不是以-1为参数生成randomtest类的
            {
                string tmp="##";
                tmp=us.Read("Test","tid= '"+tidcount.ToString()+"'",2,"tdatetime-tamount");
                int i=tmp.IndexOf("#");
                string date=tmp.Substring(0,i);
                tmp=tmp.Substring(i+1);
                i=tmp.IndexOf("#");
                int tcount=Convert.ToInt32(tmp.Substring(0,i));
                randomtest.set_config(-1,tcount);
                int j=randomtest.M_readinit(tidcount.ToString(),Convert.ToDateTime(date));
                if (j < 0)
                {
                    label4.Text = "读取记忆文件初始化失败";
                }
                else
                {
                    int k=randomtest.Buff_refresh();
                    if (k < 0) label4.Text = "刷新缓冲区失败";
                    else
                        RefreshCom();
                }
                start_config = true;
            }
            else
            {
                if(testtype>0&&TestCountLimit>0&&classnum>0)
                randomtest.set_config(testtype,TestCountLimit,classnum);
                //int i=randomtest.Buff_refresh();缓冲区刷新只要开始的一次即可
                //int j = randomtest.Sample_refresh();把题目刷新操作放在refresh中
                start_config = true;
                int i = randomtest.Buff_refresh();
                if (i < 0)
                {
                    label2.Text = "数据库操作中发生错误，刷新Buff缓冲区失败";
                }
                else
                {
                    RefreshCom();
                    button6.Enabled = true;
                }
            }
            if(start_config){
            //button6.Enabled = true;
                flg_close = true;
            label2.Visible = true;
            button5.Enabled = false;
            button7.Enabled = true;
            panel2.Visible = true;
            panel1.Enabled = false;
            groupBox1.Enabled = false;
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
            else
            {
                classnum = listBox1.SelectedIndex + 1;
            }
        }

        private void button6_Click(object sender, EventArgs e)//这个实现刷新一遍Buff缓冲区
        {
            int i = randomtest.Buff_refresh();
            if (i != -1)
            {
                RefreshCom();
            }
            else
            {
                label4.Text = "刷新缓冲区的时候发生了错误";
            }
        }
        bool flg_close = false;
        private void button7_Click(object sender, EventArgs e)//退出
        {
            if (Testcount > TestCountLimit-1)
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

        private void RefreshCom()
        {
            if (Testcount > TestCountLimit-1)//测验已经结束
            { 
                //产生别的信息（是否保存记录、重来、退出）
                label2.Visible = false;
                panel2.Visible = false;
                button6.Enabled = false;
                randomtest.all_clear();//清空原有的信息
                label4.Text = "本次小测试已经完成，"+TestCountLimit.ToString()+"道题中，你一共对了"+TheTrueCount.ToString()+"道。\n是否保存记录用于记忆曲线生成？";
                panel3.Visible = true;
            }
            else
            {
                //int i = randomtest.Buff_refresh();
                int j = randomtest.Sample_refresh();
                if (j >= 0)
                {
                    //刷新题目
                    label2.Text = "选择对应如下文字的答案：" + randomtest.GetSample(0);
                    //刷新按钮组
                    button1.Text = randomtest.GetSample(1);
                    button2.Text = randomtest.GetSample(2);
                    button3.Text = randomtest.GetSample(3);
                    button4.Text = randomtest.GetSample(4);
                    //刷新答案
                    ans = randomtest.Get_ans();
                    //刷新提示
                    label4.Text = "当前进度为" + Testcount.ToString() + @"/" +
                        TestCountLimit.ToString();
                    /*+"本题答案为:"+ans+"\n"+"1-4的答案分别为"+
                        randomtest.GetSource(1) + "..." + randomtest.GetSource(2) + "..." + randomtest.GetSource(3) + "..." + randomtest.GetSource(4);*/
                }
                else
                {
                    label4.Text = "初始化题库的时候发生了错误";
                }
            }
        }

        bool success_flag = false;
        private void button8_Click(object sender, EventArgs e)//选择保存结果
        {
            if (!randomtest.getMode())
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
                    int control2 = randomtest.product_log(this_tid, dt);
                    if (control2 < 0)
                    {
                        Console.WriteLine("文件记录产生失败：product_log()");
                        return;
                    }
                    else
                    {
                        success_flag = true;
                        Console.WriteLine("文件已成功产生,时间为"+dt.ToString());
                        //link
                        int c1=us.pWriteLink(this_tid,1);
                        if (c1 < 0) Console.WriteLine("WriteLink出错");
                        //grade
                        int c2=us.pWriteGrade(this_tid,0,(int)(100*TheTrueCount/Testcount));
                        if (c2 < 0) Console.WriteLine("WriteGrade出错");
                        //test
                        int c3=us.pWriteTest(this_tid,dt,testtype.ToString(),Testcount);
                        if (c3 < 0) Console.WriteLine("WriteText出错");
                        if (c1 > 0 && c2 > 0 && c3 > 0)
                        {
                            label4.Text = "结果已经成功保存,时间为"+dt.ToString();
                            panel3.Visible = false;
                        }

                    }
                }
            }
            else//记忆曲线生成情况下使用
            {
                string info=us.Read("Link","tid='"+tidcount.ToString()+"' and visible=1 ",1,"nextlevel");
                int p = info.IndexOf("#");
                if (p >= 0)
                {
                    info = info.Substring(0, p);
                    int level = Convert.ToInt32(info);
                    int c1 = us.pUpdateLink(tidcount.ToString(), level + 1);
                    int c2 = us.pWriteGrade(tidcount.ToString(), level, 100 * TheTrueCount / Testcount);
                    if (c1 > 0 && c2 > 0)
                    {
                        label4.Text = "结果已经成功保存";
                        panel3.Visible = false;
                    }
                    //可以选择把前面的记录调整下
                }
            }
        }
    }
}
