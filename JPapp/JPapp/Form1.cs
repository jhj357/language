using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace JPapp
{
    public partial class Form1 : Form
    {
        public UserClass userinfo;
        public Form1()
        {
            InitializeComponent();
            userinfo = new UserClass();
            进入用户界面ToolStripMenuItem.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WordRemember wordremember = new WordRemember();
            wordremember.Show();
            this.Hide();
            wordremember.FormClosed += delegate(object s, FormClosedEventArgs fe) { this.Show(); };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WordTest wordtest = new WordTest(this.userinfo);
            wordtest.Show();
            this.Hide();
            wordtest.FormClosed += delegate(object s, FormClosedEventArgs fe) { this.Show(); };
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SentenceTest sentence = new SentenceTest(this.userinfo);
            sentence.Show();
            this.Hide();
            sentence.FormClosed += delegate(object s, FormClosedEventArgs fe) { this.Show(); };
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GrammarTest grammartest = new GrammarTest(this.userinfo);
            grammartest.Show();
            this.Hide();
            grammartest.FormClosed += delegate(object s, FormClosedEventArgs fe) { this.Show(); };
        }

        private void button5_Click(object sender, EventArgs e)//用户登陆按钮
        {
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            panel1.Visible = true;
            panel2.Visible = false;
            label6.Text = "请输入登陆信息";
        }

        private void button6_Click(object sender, EventArgs e)//用户注册按钮
        {
            textBox1.Text = "";
            textBox2.Text = "";
            panel1.Visible = false;
            panel2.Visible = true;
            label6.Text = "请输入注册信息";
        }

        private void button7_Click(object sender, EventArgs e)//登陆按钮触发事件
        {
            string username = textBox1.Text.Trim();//用户名
            string password = textBox2.Text.Trim();//用户密码
            if (username.Equals("") || password.Equals(""))
            {
                label6.Text = "信息不能为空";
            }
            else if(username.Length>10||password.Length>15)//按照数据库中设计，用户名是十位，密码最高是15位
            {
                label6.Text = "信息长度不符：用户名最高是10位，密码最高是15位";
            }
            else
            {
                int tmp = -1;
                tmp = userinfo.login(username, password);
                if (tmp == 1)
                {
                    label6.Text = "登录成功";
                    panel3.Visible = false;//取消登陆和注册界面
                    //开启额外功能
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    //可以注销
                    button9.Visible = true;
                    textBox1.Text = "";
                    textBox2.Text = "";
                    进入用户界面ToolStripMenuItem.Enabled = true;

                }
                else if (tmp == 0)
                {
                    label6.Text = "登录失败：用户密码错误，请重新输入";
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
                else if (tmp == -1)
                {
                    label6.Text = "登陆失败：数据库内操作有误。";
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
                else if (tmp == 2)
                {
                    label6.Text = "登陆失败：该用户未注册";
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
                //无其他情况
            }
        }

        private void button8_Click(object sender, EventArgs e)//注册按钮触发事件
        {
            string username = textBox3.Text.Trim();
            string password1 = textBox4.Text.Trim();
            string password2 = textBox5.Text.Trim();
            if(username.Equals("")||password1.Equals("")||password2.Equals(""))
            {
                label6.Text="不能有信息为空";
            }
            else if(username.Length>10||password1.Length>15||password2.Length>15)
            {
                label6.Text="信息长度不符：用户名最高是10位，密码最高是15位";
            }
            else if(!password1.Equals(password2))
            {
                label6.Text="两次输入密码不同";
            }
            else if (!Regex.IsMatch(username, @"^\w+$") || !Regex.IsMatch(password1, @"^\w+$"))
            {
                //正则"^\w+$"
                label6.Text = "含有非法字符";
            }
            else{
                int tmp=-1;
                tmp=userinfo.register(username,password1);
                if(tmp==1)
                {
                    label6.Text="用户注册成功";
                    panel3.Visible = false;//取消登陆和注册界面
                    //开启额外功能
                    button2.Enabled=true;
                    button3.Enabled=true;
                    button4.Enabled=true;
                    //可以注销
                    button9.Visible = true;
                    //清空原来的输入框数据
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    进入用户界面ToolStripMenuItem.Enabled = true;
                }
                else if(tmp==0)
                {
                    label6.Text="已有相同用户存在";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
                else if(tmp==-1)
                {
                    label6.Text="登录失败：数据库操作出现错误";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int i=userinfo.logout();
            if (i == 1)
            {
                button9.Visible = false;//关闭注销
                进入用户界面ToolStripMenuItem.Enabled = false;
                //panel3.Visible = true;//开启登陆
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button10.Visible = true;
                label6.Text = "请输入登陆信息";
            }
            else
            { 
                label6.Text="注销操作发生错误";
                Console.WriteLine("error"); }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //开启额外功能
            /*方便调试使用
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            */
            panel3.Visible = true;
            button10.Visible = false;
        }

        private void 进入用户界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserWindow uw = new UserWindow(this.userinfo);
            uw.Show();
            this.Hide();
            uw.FormClosed += delegate(object s, FormClosedEventArgs fe) { this.Show(); };
        }

        private void 使用说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
            this.Hide();
            help.FormClosed += delegate(object s, FormClosedEventArgs fe) { this.Show(); };
        }

        private void 查看开发信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUs au = new AboutUs();
            au.Show();
            this.Hide();
            au.FormClosed += delegate(object s, FormClosedEventArgs fe) { this.Show(); };
        }
        
    }
}
