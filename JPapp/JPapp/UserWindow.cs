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
    public partial class UserWindow : Form
    {
        UserClass us;
        int Mode = 0;//用于标明当前所处的模式,初始模式为0
        int menu = 0;//用于选择对应的选项
        int Process = 0;//用于标明执行阶段
        const int ch_limit = 10;
        string[] command_history;
        int ch_count = 0;
        public UserWindow(UserClass userclass)
        {
            InitializeComponent();
            this.us = userclass;
            command_history=new string[ch_limit];
            init();
        }

        void init()
        {
            panel2.Visible = false;
            panel3.Visible = false;
            for (int i = 0; i < ch_limit; i++)
                command_history[i] = "";
        }
        private void button1_Click(object sender, EventArgs e)//修改密码，成功之后会强制退出要求重新登录
        {
            dataGridView1.Rows.Clear();
            panel3.Visible = true;
            panel2.Visible = false;
            Mode = 1;
            richTextBox1.AppendText("选择了密码修改服务！！\n");
            richTextBox1.AppendText("请在命令行输入按以下要求输入命令：\n");
            richTextBox1.AppendText("-----------请输入旧密码-----------\n");
            if (Mode == 1)
                textBox1.PasswordChar = '*';
            else
                textBox1.PasswordChar = '\0';
        }

        private void button2_Click(object sender, EventArgs e)//查看成绩
        {
            dataGridView1.Rows.Clear();
            panel3.Visible = true;
            panel2.Visible = false;
            Mode = 2;
            richTextBox1.AppendText("选择了成绩查看服务！！\n");
            richTextBox1.AppendText("请在命令行按照括号内格式输入命令：\n");
            richTextBox1.AppendText("1.按记录号查看<1+记录号> 例：1+12 \n");
            richTextBox1.AppendText("2.按日期查看<2+日期> 例：2+2014-5-20 \n");
            richTextBox1.AppendText("3.查看最近一段时间<<3+时间单位+偏移量> \n");
            richTextBox1.AppendText("注：时间单位包括：m(分),h(时),d(天)例:3+m+30 查看前后半小时内成绩 \n");
            if (Mode == 1)
                textBox1.PasswordChar = '*';
            else
                textBox1.PasswordChar = '\0';
        }

        private void button3_Click(object sender, EventArgs e)//查看记忆时间点
        {
            dataGridView1.Rows.Clear();
            panel3.Visible = true;
            panel2.Visible = false;
            Mode = 3;
            richTextBox1.AppendText("选择了记忆点查看服务！！\n");
            richTextBox1.AppendText("1.按记录号查看<1+记录号> 例：1+12 \n");
            richTextBox1.AppendText("2.按日期查看<2+日期> 例：2+2014-5-20 \n");
            richTextBox1.AppendText("3.查看最近一段时间内的记忆点<3+时间单位+偏移量> \n");
            richTextBox1.AppendText("注：时间单位包括：m(分),h(时),d(天)例:3+m+30 查看前后半小时内记忆点 \n");
            if (Mode == 1)
                textBox1.PasswordChar = '*';
            else
                textBox1.PasswordChar = '\0';
        }

        private void button4_Click(object sender, EventArgs e)//删除记录
        {
            dataGridView1.Rows.Clear();
            panel3.Visible = true;
            panel2.Visible = false;
            Mode = 4;
            richTextBox1.AppendText("选择了数据删除服务！！\n");
            richTextBox1.AppendText("1.按记录号删除<1+记录号> 例：1+12 \n");
            richTextBox1.AppendText("2.按日期删除<2+日期> 例：2+2014-5-20 \n");
            richTextBox1.AppendText("3.删除历史记录(n天前)<3+天数> 例：3+15 \n");
            if (Mode == 1)
                textBox1.PasswordChar = '*';
            else
                textBox1.PasswordChar = '\0';
        }

        private void button5_Click(object sender, EventArgs e)//退出
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)//清空系统提示
        {
            richTextBox1.Clear();
        }

        private void button7_Click(object sender, EventArgs e)//提交用户命令
        {
            string command = textBox1.Text.Trim();
            if (command.Equals(""))
                richTextBox1.AppendText("命令行不能为空！！");
            else
                command_deal(command);
            textBox1.Clear();
        }

        private void button8_Click(object sender, EventArgs e)//清空命令行
        {
            textBox1.Clear();
        }

        private void command_deal(String command)
        {
            switch (Mode)
            {
                case 0://无效服务
                    break;
                case 1://修改密码
                    if (!Regex.IsMatch(command, @"^\w+$"))
                    {
                        richTextBox1.AppendText("输入含有非法字符因而无效");
                        return;
                    }
                    if (Process == 0)//command为旧密码
                    {
                        if (command.Equals(us.getPassword()))
                        {
                            Process++;
                            richTextBox1.AppendText("原密码验证正确！！请输入新密码。\n");
                        }
                        else
                        { richTextBox1.AppendText("原密码验证错误。请重新输入原密码\n"); }
                    }
                    else if(Process==1)
                    {
                        command_history[ch_count] = command;
                        richTextBox1.AppendText("请输入新密码");
                        ch_count++;
                        Process++;
                    }
                    else if(Process==2)
                    {
                        if (command.Equals(command_history[ch_count - 1]))
                        {
                            richTextBox1.AppendText("两次密码一致！！\n");
                            Process = 0;
                            ch_count = 0;
                            command_history[ch_count]="";
                            //修改数据库密码
                            if (us.pUpdatePassword(command) > 0)
                            {
                                richTextBox1.AppendText("密码修改成功\n");
                                //要求退出重新登录
                            }
                            else richTextBox1.AppendText("密码修改失败\n");
                        }
                        else
                        {
                            richTextBox1.AppendText("两次密码不一致！！请重设\n");
                            textBox1.Clear();
                            //返回第一次输入
                            Process--;
                            ch_count--;
                        }
                    }

                    break;
                case 2://查看成绩
                    bool flg2 = true;
                    try {
                        if (dataGridView1.Visible == true)
                            dataGridView1.Rows.Clear();
                        string tmp2 = command;
                        int p1=tmp2.IndexOf("+");
                        menu = Convert.ToInt32(tmp2.Substring(0,p1));
                        if (menu == 0)
                        { flg2 = false; }
                        else if (menu == 1)
                        {
                            string tmp_2 = tmp2.Substring(p1 + 1);//tid
                            string grade = us.Read("Grade", "tid='" + tmp_2 + "' and tid in(select tid from Link where uid =" + us.getUserID() + ")", 1, "grade").Replace("#", "");
                            if (grade.Equals(""))
                                flg2 = false;
                            else
                            {
                                int index = dataGridView1.Rows.Add();
                                dataGridView1.Rows[index].Cells[0].Value = tmp_2;
                                int count = 1;
                                while(grade.Contains("#"))
                                {
                                    int p2 = grade.IndexOf("#");
                                    string grade1 = grade.Substring(0,p2);
                                    dataGridView1.Rows[index].Cells[count].Value = grade1;
                                    count++;
                                    grade = grade.Substring(p2+1);
                                }
                            }
                        }
                        else if (menu == 2)
                        {
                            string tmp_2 = tmp2.Substring(p1 + 1);
                            DateTime dt = Convert.ToDateTime(tmp_2);
                            string grade = us.Read("Grade g,Test t", "datediff(DAY,t.tdatetime,'" + dt.ToString() + "')=0 and g.tid=t.tid and t.tid in(select tid from Link where uid ='" + us.getUserID() + "')", 2, "g.grade,g.tid");
                            if (grade.Equals(""))
                                flg2 = false;
                            else
                            {
                                int k1 = grade.IndexOf("#");
                                string g1 = grade.Substring(0,k1);
                                grade = grade.Substring(k1+1);
                                int k2 = grade.IndexOf("#");
                                string t1 = grade.Substring(0,k2);
                                grade = grade.Substring(k2+1);
                                int count=2;
                                int index = dataGridView1.Rows.Add();
                                dataGridView1.Rows[index].Cells[0].Value = t1;
                                dataGridView1.Rows[index].Cells[1].Value = g1;
                                while (grade.Contains("#"))
                                {
                                    int k3 = grade.IndexOf("#");
                                    string g3 = grade.Substring(0,k3);
                                    grade = grade.Substring(k3+1);
                                    int k4 = grade.IndexOf("#");
                                    string t2 = grade.Substring(0, k4);
                                    grade = grade.Substring(k4+1);
                                    if (!t1.Equals(t2))//属于不同的tid
                                    {
                                        g1 = g3;
                                        t1 = t2;
                                        count = 2;
                                        index = dataGridView1.Rows.Add();
                                        dataGridView1.Rows[index].Cells[0].Value = t1;
                                        dataGridView1.Rows[index].Cells[1].Value = g1;
                                        break;
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[index].Cells[count].Value = g3;
                                        count++;
                                    }
                                }
                            }
                        }
                        else if (menu == 3)//查看最近一段时间
                        {
                            string tmp_2 = tmp2.Substring(p1 + 1);
                            int p2 = tmp_2.IndexOf("+");
                            string tmp = tmp_2.Substring(0,p2);
                            tmp_2 = tmp_2.Substring(p2+1);
                            int time_offset = Convert.ToInt32(tmp_2);
                            if (tmp.Equals("m"))
                            { //do nothing
                            }
                            else if (tmp.Equals("h"))
                            {
                                time_offset *= 60;
                            }
                            else if (tmp.Equals("d"))
                            {
                                time_offset *= (60 * 24);
                            }
                            else
                            { flg2 = false; break; }
                            DateTime dt = DateTime.Now;
                            Memory mem = new Memory(dt);
                            mem.min_refresh(time_offset);
                            string grade = us.Read("Grade g,Test t", "datediff(DAY,t.tdatetime,'" + mem.getUp().ToString() + "')>0 and datediff(DAY,t.tdatetime,'" + mem.getDown().ToString() + "')<0 and g.tid=t.tid and t.tid in(select tid from Link where uid ='" + us.getUserID() + "')", 2, "g.grade,g.tid");
                            if (grade.Equals(""))
                                flg2 = false;
                            else
                            {
                                int k1 = grade.IndexOf("#");
                                string g1 = grade.Substring(0, k1);
                                grade = grade.Substring(k1 + 1);
                                int k2 = grade.IndexOf("#");
                                string t1 = grade.Substring(0, k2);
                                grade = grade.Substring(k2 + 1);
                                int count = 2;
                                int index = dataGridView1.Rows.Add();
                                dataGridView1.Rows[index].Cells[0].Value = t1;
                                dataGridView1.Rows[index].Cells[1].Value = g1;
                                while (grade.Contains("#"))
                                {
                                    int k3 = grade.IndexOf("#");
                                    string g3 = grade.Substring(0, k3);
                                    grade = grade.Substring(k3 + 1);
                                    int k4 = grade.IndexOf("#");
                                    string t2 = grade.Substring(0, k4);
                                    grade = grade.Substring(k4 + 1);
                                    if (!t1.Equals(t2))//属于不同的tid
                                    {
                                        g1 = g3;
                                        t1 = t2;
                                        count = 2;
                                        index = dataGridView1.Rows.Add();
                                        dataGridView1.Rows[index].Cells[0].Value = t1;
                                        dataGridView1.Rows[index].Cells[1].Value = g1;
                                        break;
                                    }
                                    else
                                    {
                                        dataGridView1.Rows[index].Cells[count].Value = g3;
                                        count++;
                                    }
                                }
                            }
                        }
                        else
                        { flg2 = false; }
                    }
                    catch (Exception e2)
                    {
                        richTextBox1.AppendText("格式错误请重新输入！！\n");
                        flg2 = false;
                    }
                    finally
                    {
                        if (flg2)
                        {
                            richTextBox1.AppendText("成绩查看操作成功。\n");
                            panel2.Visible = true;
                        }
                        else
                            richTextBox1.AppendText("成绩查看操作失败。（或者没有记录）\n");
                    }
                    break;
                case 3://查看记忆点
                    bool flg3 = true;
                    try
                    {
                        if (dataGridView1.Visible == true)
                            dataGridView1.Rows.Clear();
                        string tmp3 = command;
                        int p1 = tmp3.IndexOf("+");
                        menu = Convert.ToInt32(tmp3.Substring(0,p1));
                        if (menu == 0)
                        { flg3 = false; }
                        else if (menu == 1)
                        {
                            string tmp_3 = tmp3.Substring(p1+1);//tmp_3为tid
                            string datetime = us.Read("Test","tid='"+tmp_3+"' and tid in(select tid from Link where uid ="+us.getUserID()+")",1,"tdatetime").Replace("#","");
                            if (datetime.Equals(""))
                                flg3 = false;
                            else
                            {
                                DateTime dt=Convert.ToDateTime(datetime);
                                Memory mem=new Memory(dt);
                                int index=dataGridView1.Rows.Add();
                                dataGridView1.Rows[index].Cells[0].Value = tmp_3;
                                for (int i = 1; i < mem.getTLimit(); i++)
                                {
                                    string date = mem.getMemoryDate(i).ToString();
                                    dataGridView1.Rows[index].Cells[i].Value=date;
                                }
                                //flg3 = true;
                            }
                        }
                        else if (menu == 2)
                        {
                            string tmp_3 = tmp3.Substring(p1+1);//tmp_3为日期
                            DateTime dt=Convert.ToDateTime(tmp_3);
                            string datetime = us.Read("Test","datediff(DAY,tdatetime,'"+dt.ToString()+"')=0 and tid in(select tid from Link where uid ='"+us.getUserID()+"')",2,"tdatetime,tid");
                            //datetime = datetime + " ";//防止最后的截取失败
                            //Console.WriteLine(datetime);
                            if (datetime.Equals(""))
                                flg3 = false;
                            else
                            {
                                int count = 0;
                                while (datetime.Contains("#"))
                                {
                                    int p = datetime.IndexOf("#");
                                    string date = datetime.Substring(0,p);
                                    datetime = datetime.Substring(p+1);
                                    int p_ = datetime.IndexOf("#");
                                    string tid = datetime.Substring(0,p_);
                                    Memory mem = new Memory(Convert.ToDateTime(date));
                                    int index = dataGridView1.Rows.Add();
                                    dataGridView1.Rows[index].Cells[0].Value = tid;
                                    for (int i = 1; i < mem.getTLimit(); i++)
                                    {
                                        string date1 = mem.getMemoryDate(i).ToString();
                                        dataGridView1.Rows[index].Cells[i].Value = date1;
                                    }
                                    count++;
                                    datetime = datetime.Substring(p_+1);
                                }
                            }
                        }
                        else if (menu == 3)
                        {
                            string tmp_3 = tmp3.Substring(p1+1);
                            int p2 = tmp_3.IndexOf("+");
                            string tmp = tmp_3.Substring(0,p2);//时间单位
                            int time_offset = Convert.ToInt32(tmp_3.Substring(p2+1));//时间偏移
                            if (tmp.Equals("m"))
                            { //do nothing
                            }
                            else if (tmp.Equals("h"))
                            {
                                time_offset *= 60;
                            }
                            else if (tmp.Equals("d"))
                            {
                                time_offset *= (60 * 24);
                            }
                            else
                            { flg3 = false; break; }
                            DateTime dt = DateTime.Now;
                            Memory mem = new Memory(dt);
                            mem.min_refresh(time_offset);
                            string datetime = us.Read("Test", "datediff(DAY,tdatetime,'" + mem.getUp().ToString() + "')>0 and datediff(DAY,tdatetime,'"+mem.getDown().ToString()+"')<0 and tid in(select tid from Link where uid ='"+us.getUserID()+"')", 2, "tdatetime,tid");
                            Console.WriteLine(datetime);
                            if (datetime.Equals(""))
                                flg3 = false;
                            else
                            {
                                int count = 0;
                                while (datetime.Contains("#"))
                                {
                                    int p = datetime.IndexOf("#");
                                    string date = datetime.Substring(0, p);
                                    datetime = datetime.Substring(p + 1);
                                    int p_ = datetime.IndexOf("#");
                                    string tid = datetime.Substring(0, p_);
                                    Memory mem1 = new Memory(Convert.ToDateTime(date));
                                    int index = dataGridView1.Rows.Add();
                                    dataGridView1.Rows[index].Cells[0].Value = tid;
                                    for (int i = 1; i < mem1.getTLimit(); i++)
                                    {
                                        string date1 = mem1.getMemoryDate(i).ToString();
                                        dataGridView1.Rows[index].Cells[i].Value = date1;
                                    }
                                    count++;
                                    datetime = datetime.Substring(p_ + 1);
                                }
                            }
                        }
                        else
                        { flg3 = false; }

                    }
                    catch (Exception e3)
                    {
                        richTextBox1.AppendText("格式错误请重新输入！！\n");
                        flg3 = false;
                    }
                    finally 
                    {
                        if (flg3)
                        {
                            richTextBox1.AppendText("记忆查看操作成功。\n");
                            panel2.Visible = true;
                        }
                        else
                            richTextBox1.AppendText("记忆查看操作失败。（没有记录）\n");
                    }
                    break;
                case 4://数据删除
                    bool flg4=true;
                    try
                    {
                        if (dataGridView1.Visible == true)
                            dataGridView1.Rows.Clear();
                        string tmp1 = command;
                        int p1 = tmp1.IndexOf("+");
                        menu = Convert.ToInt32(tmp1.Substring(0,p1));
                        //Console.WriteLine(menu);
                        if (menu == 0)
                        { //无效
                            flg4 = true;
                        }
                        else if (menu == 1)
                        {
                            string tmp2 = tmp1.Substring(p1 + 1);
                            //int p2 = Convert.ToInt32(tmp2);
                            if (us.pDelete(tmp2) > 0)
                                flg4 = true;
                        }
                        else if (menu == 2)
                        {
                            string tmp2 = tmp1.Substring(p1 + 1);
                            DateTime dt = Convert.ToDateTime(tmp2);
                            //Console.WriteLine(dt.ToString());
                            if (us.pDelete(dt) > 0)
                                flg4 = true;
                        }
                        else if (menu == 3)
                        {
                            string tmp2 = tmp1.Substring(p1 + 1);
                            int p2 = Convert.ToInt32(tmp2);
                            DateTime dt = DateTime.Now;
                            if (us.pDelete(dt, p2) > 1)
                                flg4 = true;
                        }
                        else
                            flg4 = false;
                    }
                    catch (Exception e4)
                    {
                        richTextBox1.AppendText("格式错误请重新输入！！\n");
                        flg4 = false;
                    }
                    finally
                    {
                        if (flg4)
                            richTextBox1.AppendText("删除操作成功。\n");
                        else
                            richTextBox1.AppendText("删除操作失败。\n");
                    }
                    break;
                default: break;

            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
}
