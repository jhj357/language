using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace JPapp
{
    public class UserClass
    {
        private int init_flag;//类状态标志
        private String UserID;
        private String UserName;
        private String Password;
        const string Connection = @"Data Source=.;Initial Catalog=jpapp;Integrated Security = True";

        public UserClass()
        {
            init_flag = -1;
            UserID = "";
            UserName = "";
            Password = "";
        }

        public int register(String USERNAME,String PASSWORD)//注册用户，返回值控制是否成功（0,1，-1）
        {
            try{
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "select count(uid) num from dbo.jpUser where username='"+USERNAME+"'";
                SqlCommand sc1 = new SqlCommand(sql1,conn);
                SqlDataReader dr1=sc1.ExecuteReader();
                //Console.WriteLine("test point 0.....");//for test
                if (dr1.Read())
                {
                    int i = -1;
                    i = dr1.GetInt32(0);
                    dr1.Close();
                    if(i == 0)//没有相同名称的用户存在
                    {
                        string sql2 = "select count(*) from jpUser";
                        SqlCommand sc2 = new SqlCommand(sql2, conn);
                        //Console.WriteLine("test point 0....."+i);//for test
                        SqlDataReader dr2 = sc2.ExecuteReader();
                        int count = -1;
                        //Console.WriteLine("testpoint 1.....");//for test
                        if (dr2.Read())//生成新uid
                        {
                            count = dr2.GetInt32(0);
                            count += 1;
                            dr2.Close();
                            string tmp = count.ToString();
                            //Console.WriteLine("test point2.....");//for test
                            string sql3 = "insert into jpUser(uid,username,password) values('"+tmp+"','"+USERNAME+"','"+PASSWORD+"')";
                            SqlCommand sc3 = new SqlCommand(sql3, conn);//执行插入
                            //Console.WriteLine("test point3....."+sql3);//for test
                            sc3.ExecuteReader();
                            //Console.WriteLine("test point4.....");//for test
                            //------------------
                            init_flag = 1;
                            UserID = tmp;
                            UserName = USERNAME;
                            Password = PASSWORD;
                        }
                        conn.Close();
                        return 1;
                    }
                    else if (i > 0)
                    {
                        conn.Close();
                        return 0;//已有相同用户存在 
                    }
                    else
                    {
                        conn.Close();
                        return -1; }//其他情况,error
                }
                conn.Close();
                return -1;//error
            }catch(Exception e)
            {Console.WriteLine(e.StackTrace);}

            return -1;//error
        }

        public int login(String USERNAME, String PASSWORD)//用户登录，返回值提示错误信息（-1,0,1,2）
        {
            try
            {
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                //Console.WriteLine("test.....");//for test
                string sql1 = "select password,uid from jpUser where username='" + USERNAME + "'";
                SqlCommand sc1 = new SqlCommand(sql1, conn);
                //Console.WriteLine("test.....");//for test
                SqlDataReader sr1 = sc1.ExecuteReader();
                //Console.WriteLine("test.....");//for test
                if (sr1.Read())
                {
                    string tmp = sr1["password"].ToString();
                    string tmp_uid = sr1["uid"].ToString();
                    if (PASSWORD.Equals(tmp))//完全匹配，修改标志位
                    {
                        init_flag = 1;
                        UserName = USERNAME;
                        Password = PASSWORD;
                        UserID = tmp_uid;
                        conn.Close();
                        return 1;
                    }
                    else//只是存储了用户名，并未登陆成功
                    {
                        init_flag = 0;
                        UserName = USERNAME;
                        conn.Close();
                        return 0;
                    }
                }
                else
                {
                    conn.Close();
                    return 2;//用户尚未注册
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return -1;//数据库开启和连接操作错误引发的异常
        }

        public int logout()//用户注销
        {
            init_flag = -1;
            UserID = "";
            UserName = "";
            Password = "";
            return 1;
        }

        public int pWriteTest(String tid, DateTime tdatetime,String ttype,int tamount=0)//test(--tid,--tdatetime,--ttype)验证在上层
        {
            try {
                if (init_flag<=0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "insert into dbo.Test(tid,tdatetime,ttype,tamount) values('" + tid + "','" +tdatetime.ToString()+ "','" + ttype +"',"+tamount.ToString()+ ")";
                //Console.WriteLine("text:"+sql1);
                SqlCommand sc1 = new SqlCommand(sql1,conn);
                sc1.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.StackTrace); 
            }
            return -1;
        }

        public int pWriteLink(String tid,int nextlevel,int visible=1)//link（--tid--nextlevel）
        {
            try {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                //tid = tid.Trim();
                string sql1 = "insert into dbo.Link(uid,tid,nextlevel,visible) values('"+UserID+"','"+tid+"',"+nextlevel.ToString()+","+visible.ToString()+")";
                //Console.WriteLine("link:"+sql1);
                SqlCommand sc1 = new SqlCommand(sql1,conn);
                sc1.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch(Exception e)
            { Console.WriteLine(e.StackTrace); }
            return -1;
        }

        public int pWriteGrade(String tid,int tlevel,int grade)//grade(tid,tlevel,grade)
        {
            try {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "insert into dbo.Grade(tid,tlevel,grade) values('"+tid+"',"+tlevel.ToString()+","+grade.ToString()+")";
                //Console.WriteLine(sql1);
                SqlCommand sc1 = new SqlCommand(sql1,conn);
                sc1.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return -1;
        }

        public int pUpdateLink(String tid, int nextlevel)
        {
            try
            {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                //string sql1 = "update dbo.Link set nextlevel="+nextlevel.ToString()+" where tid='"+tid+"'" ;
                string sql1 = "update dbo.Link set nextlevel="+nextlevel.ToString()+" where tid='"+tid+"' and uid='"+UserID+"'" ;
                SqlCommand sc1 = new SqlCommand(sql1, conn);
                sc1.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            { Console.WriteLine(e.StackTrace); }
            return -1;
        }

        public int pDelete(String tid)
        {
            try
            {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "update dbo.Link set visible=0 where tid='" + tid + "' and uid='" + UserID + "'";
                //Console.WriteLine(sql1);
                SqlCommand sc1 = new SqlCommand(sql1, conn);
                sc1.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return -1;
        }

        public int pDelete(DateTime datetime)
        {
            try
            {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "update dbo.Link set visible=0 where uid= "+UserID +" and tid in(select tid from Test where datediff(DAY,tdatetime,'"+datetime.ToString()+"')=0)";
                SqlCommand sc1 = new SqlCommand(sql1, conn);
                sc1.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            { Console.WriteLine(e.StackTrace); }
            return -1;
        }

        public int pDelete(DateTime datetime,int days)
        {
            try
            {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "update dbo.Link set visible=0 where uid= " + UserID + " and tid in(select tid from Test where datediff(DAY,tdatetime,'" + datetime.ToString() + "')>" + days.ToString() + ")";
                SqlCommand sc1 = new SqlCommand(sql1, conn);
                sc1.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            { Console.WriteLine(e.StackTrace); }
            return -1;
        }

        public int pUpdatePassword(String newpassword)
        {
            try
            {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "update dbo.jpUser set password=" + newpassword.ToString() + " where uid='" + UserID + "'";
                SqlCommand sc1 = new SqlCommand(sql1, conn);
                sc1.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            { Console.WriteLine(e.StackTrace); }
            return -1;
        }

        public String Read(String Table,String Requirement,int TypeNum=0,String TypeString="")//TypeString 格式为A-B-C这样并列，并且TypeString中必定包含所对应表中的一个候选键才行
        {
            try {
                if (Table.Equals("") || Requirement.Equals("") || TypeNum <= 0)
                {
                    return ""; 
                }
                else
                {
                    string col = TypeString.Replace("-",",");
                    SqlConnection conn = new SqlConnection(Connection);
                    conn.Open();
                    string sql1 = "select "+col+" from "+Table+" where "+Requirement;
                    Console.WriteLine(sql1);
                    SqlCommand sc = new SqlCommand(sql1,conn);
                    SqlDataReader sr1 = sc.ExecuteReader();
                    String Need = "";
                    while(sr1.Read())
                    {
                        for (int i = 0; i < TypeNum; i++)
                        {
                            //Console.WriteLine(Need);
                            Need = Need + sr1.GetSqlValue(i).ToString() + "#";
                        }
                    }
                    sr1.Close();
                    conn.Close();
                    return Need;
                }
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("数据库操作出错，请检查sql语句的正确性，以及确保输入的参数正确");
            }
            return "  ";
        }

        public int getflag()
        {
            return init_flag;
        }

        public int getAmount(String Table)
        {
            try
            {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "select count(*) from "+Table;
                SqlCommand sc1 = new SqlCommand(sql1, conn);
                SqlDataReader sr = sc1.ExecuteReader();
                if (sr.Read())
                {
                    return sr.GetInt32(0);
                }
                conn.Close();
                
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.StackTrace); 
            }
            return -1;
        }

        public String getUserID()
        {
            return UserID;
        }

        public String getUserName()
        {
            return UserName;
        }

        public String getPassword()
        {
            return Password;
        }

        public String getMtid(int checklevel,String ttype,DateTime upDate,DateTime downDate)
            //获取所有以当前日期和当前偏差形成的区域内的nextlevel为给定值的tid组
        {
            try
            {
                if (init_flag<=0)
                    return "";
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "(select tid from Link  where uid='"+UserID+"' and nextlevel="+checklevel.ToString()+")";
                string sql2 = "select tid from Test where ttype='" + ttype + "' and datediff(minute,tdatetime,'" + upDate+"')>0 and datediff(minute,tdatetime,'"+downDate+"')<0 and tid in " ;
                string sql3 = sql2 + sql1;
                //Console.WriteLine(sql3);
                SqlCommand sc = new SqlCommand(sql3,conn);
                SqlDataReader sr = sc.ExecuteReader();
                string tidstring = "";
                while (sr.Read())
                {
                    string tmp = sr.GetSqlValue(0).ToString().Trim();
                    if (!tmp.Equals(""))
                    {
                        tidstring += (tmp+"#");
                    }
                }
                sr.Close();
                conn.Close();
                return tidstring;
            }
            catch (Exception e)
            { Console.WriteLine(e.StackTrace); }
            return "";
        }

        public int updateMtid(int checklevel, DateTime upDate, DateTime downDate)
            //将link记录中的时间段内的所有nextlevel小于checklevel的记录修改为checklevel+1，不管类型，注意此处必须是小于，不包括等于
        {
            try
            {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "(select tid from Link  where uid='" + UserID + "' and nextlevel<" + checklevel.ToString() + ")";
                string sql2 = "(select tid from Test where datediff(minute,tdatetime,'" + upDate + "')>0 and datediff(minute,tdatetime,'" + downDate + "')<0 and tid in"+sql1+")";
                string sql3 = "update Link set nextlevel="+checklevel.ToString()+" where tid in "+sql2;
                SqlCommand sc = new SqlCommand(sql3, conn);
                Console.WriteLine(sql3);
                sc.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            { Console.WriteLine(e.StackTrace); }
            return 0;
        }

        public int giveupStid(DateTime DeadDate)//放弃该日期前所有记录，修改nextlevel为不可达到
        {
            const int Dead = 9;
            try
            {
                if (init_flag <= 0)
                    return -1;
                SqlConnection conn = new SqlConnection(Connection);
                conn.Open();
                string sql1 = "(select tid from Link  where uid='" + UserID + ")";
                string sql2 = "(select tid from Test where datediff(minute,tdatetime,'" + DeadDate + "')<0 and tid in" + sql1 + ")";
                string sql3 = "update Link set nextlevel=" + Dead.ToString() + " where tid in " + sql2;
                SqlCommand sc = new SqlCommand(sql3, conn);
                sc.ExecuteReader();
                conn.Close();
                return 1;
            }
            catch (Exception e)
            { Console.WriteLine(e.StackTrace); }
            return 0;
        }
    }
}
//采集tid的思路：1）检索当前时间下对应的不同的level的时间段tid
//               2）有的tid nextlevel小于当前检测checklevel,将其nextlevel置为单前检查的checklevel
//               
//