using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace JPapp
{
    class RandomTest//用于题目生成的类
    {
        bool init_flag;
        String[] Samples;//用于存放答案，其中0为题目，1-4为选项
        int[] Source;//用于表明选项来源，为一个四位数据，其中0、1标明其行号，2、3标明其课文号
        int ans;//用于存放答案的编号(题号)
        const int Sample_size = 5;//答案存储区大小
        const int Source_size = 4;//选项存储区大小

        int TestCount;//测试次数
        const int TestCountLimit=30;//一次测试最大上限为30
        int TestType;//测试种类
        int ClassNum;//如果选择了课内随机，需要使用确定的课文号
        // 题目类型 1--课内随机 2--教材内随机 （单词）
        //          3--课内随机 4--教材内随机 （语法）
        //          5--教材内随机 （动词形态）
        //
        String[] Buff;//题目缓冲区
        //缓冲条目格式：题号（4位）#问题#答案1#答案2#
        const int Buff_size = 40;
        int Buff_count;//缓冲区中真正的条目数
        int Bpointer;//缓冲区访问指针

        //生成的题会从缓冲区中生成，但是缓冲区中生成的题目规格数上限是一定的，对于不同的生成量，按照
        //分批的原则进行生成，并刷新缓冲区（主要是针对教材内随机）
        const string Path = @"..\..\..\src_c\";//源文件路径
        const int class_max = 25;


        public RandomTest()
        {
            init_flag = false;
            Samples = new String[Sample_size];
            Source = new int[Source_size];
            ans = 0;
            for (int i = 0; i < Sample_size; i++)
                Samples[i] = "";
            for (int j = 0; j < Source_size; j++)
                Source[j] = 0;
            Buff = new String[Buff_size];
            for (int k = 0; k < Buff_size; k++)
                Buff[k] = "";
            Buff_count = 0;
            Bpointer = 0;

            TestCount = 0;
            TestType = 0;
            ClassNum = 0;
            //以下为记忆曲线部分初始化
            TestMode = false;//普通模式,真为记忆模式
            SourceBuff=new int[SBLimit];
            for (int i = 0; i < SBLimit; i++)
                SourceBuff[i] = 0;
            SBcount = SBLimit;
            SBpointer = 0;
        }

        public RandomTest(int testtype,int testcount,int classnum=0)
        { 
            //对于单词测验：Sample 中0号位为问题，后面1-4为选项，ans 为答案对应的编号，Source 为四个条目的编号
            //对于语法测验：sample 中0号为为问题，后面1-4为选项，ans 为答案对应的编号，source 为四个答案的编号
            //对于单词的形态测验：0号为单词原型，后面1-4为4种形态，Source为四个答案的编号，该类测试为全教材测试
            //
            //
            Samples=new String[Sample_size];
            Source=new int[Source_size];
            ans = 0;
            for (int i = 0; i < Sample_size; i++)
                Samples[i] = "";
            for (int j = 0; j < Source_size; j++)
                Source[j] = 0;
            if (testtype < 0)
            {
                TestMode = true;
                TestType = -testtype;
            }
            else
            {
                TestMode = false;
                TestType = testtype;
            }
            TestCount = (testcount>TestCountLimit)?TestCountLimit:testcount;
            ClassNum = classnum;

            Buff=new String[Buff_size];
            for (int k = 0; k < Buff_size;k++)
                Buff[k] = "";
            Buff_count = 0;
            Bpointer = 0;

            init_flag = true;
            //以下为记忆曲线部分用的变量的初始化
            
            if (testcount > 0)
            {
                SBcount = NumLimit[testtype] * testcount;
                SourceBuff = new int[SBcount];
                for (int i = 0; i < SBcount; i++)
                    SourceBuff[i] = 0;
                SBpointer = 0;
            }
        }

        public int Buff_refresh()//刷新缓冲，检测生成类型--检测资源量--
        {
            try
            {
                //Console.WriteLine(TestType+"-----"+TestCount);
                if (!init_flag)
                    return -1;
                if (TestMode)//记忆模式下的生成，与课内生成完全不同，其题号数据来自SBuff
                {
                    if (TestType == 1 || TestType == 2)//单词考察
                    { 
                        int[] match=new int[Buff_size];//以match中存储的数据顺序读内容
                        for (int i = 0; i < Buff_size; i++)
                        {
                            match[i] = i;
                        }
                        Buff_count = (SBcount - SBpointer >= Buff_size) ? Buff_size : SBcount - SBpointer;
                        for (int m = 0; m < Buff_count; m++)
                        {
                            int k = SourceBuff[SBpointer + match[m]] > 0 ? SourceBuff[SBpointer + match[m]] : -SourceBuff[SBpointer + match[m]];
                            //Console.WriteLine(k);
                            int place = 0;
                            for (int n = m + 1; n < Buff_count; n++)
                            {
                                if (k > (SourceBuff[SBpointer + match[n]] > 0 ? SourceBuff[SBpointer + match[n]] : -SourceBuff[SBpointer + match[n]]))
                                {
                                    place = n;
                                    k = SourceBuff[SBpointer + match[n]] > 0 ? SourceBuff[SBpointer + match[n]] : -SourceBuff[SBpointer + match[n]];
                                }
                            }
                            //Console.WriteLine(k+"----"+place);
                            if (place != 0)
                            {
                                int tmp = match[m];//交换match中的数据
                                match[m] = match[place];
                                match[place] = tmp;
                            }
                        }//排序后可以避免多次文件读写，只要开一次文件就能保证该文件中需要的内容全部被读出
                        int count = 0;
                        int handle_id = SourceBuff[match[0] + SBpointer] > 0 ? SourceBuff[match[0] + SBpointer] : -SourceBuff[match[0] + SBpointer];
                        //Console.WriteLine("handle_id:"+handle_id+" match[0]:"+match[0]+"SBpointer:"+SBpointer);
                        //SBpointer += Buff_count;//把指针向后移动不能在此处移动
                        //刷新缓冲区后要重置Bpointer
                        Bpointer = 0;
                        while (count < Buff_count)
                        {

                            int class_id = handle_id / 100;
                            int line_id = handle_id % 100;
                            string realpath = Path + "test" + class_id.ToString() + ".txt";
                            //Console.WriteLine(realpath);
                            //Console.WriteLine(handle_id);
                            FileStream fs = new FileStream(realpath, FileMode.Open);
                            StreamReader sr = new StreamReader(fs, Encoding.Unicode);
                            String stmp1 = "";
                            int tcount = 0;
                            stmp1 = sr.ReadLine();
                            while (stmp1 != null)
                            {
                                //Console.WriteLine(stmp1);
                                if (stmp1.Contains("##1"))
                                { tcount = -1; }
                                if (stmp1.Contains("##2"))
                                { break; }
                                tcount++;
                                if (tcount == line_id)//找到对应行
                                {
                                    //读入缓冲
                                    Buff[match[count]] = stmp1;
                                    count++;
                                    if (count >= Buff_count)
                                    {
                                        SBpointer += Buff_count;
                                        sr.Close();
                                        fs.Close();
                                        return 1;
                                    }
                                    //检测下一行
                                    handle_id = SourceBuff[match[count] + SBpointer] > 0 ? SourceBuff[match[count] + SBpointer] : -SourceBuff[match[count] + SBpointer];
                                    int next_class_id = handle_id / 100;
                                    int next_line_id = handle_id % 100;
                                    //Console.WriteLine(class_id+"----"+line_id+"----"+next_class_id+"----"+next_line_id);
                                    if (class_id == next_class_id)
                                    {
                                        if (next_line_id == tcount)//该行被抽到多次
                                        {
                                            while (next_line_id == tcount && next_class_id == class_id)
                                            {
                                                Buff[match[count]] = stmp1;
                                                count++;
                                                if (count >= Buff_count)
                                                {
                                                    SBpointer += Buff_count;
                                                    sr.Close();
                                                    fs.Close();
                                                    return 1;
                                                }
                                                handle_id = SourceBuff[match[count] + SBpointer] > 0 ? SourceBuff[match[count] + SBpointer] : -SourceBuff[match[count] + SBpointer];
                                                next_class_id = handle_id / 100;
                                                next_line_id = handle_id % 100;
                                            }
                                            if (class_id != next_class_id)
                                                break;
                                        }
                                        if (next_line_id > tcount)
                                        {
                                            line_id = next_line_id;
                                        }
                                        else//不可能的情况
                                        {
                                            //Console.WriteLine("此处发生错误");
                                            sr.Close();
                                            fs.Close();
                                            return -1; }
                                    }
                                    else
                                    { break; }
                                }
                                stmp1 = sr.ReadLine();
                            }
                            sr.Close();
                            fs.Close();
                        }
                        SBpointer += Buff_count;//把指针向后移动
                    }
                    if (TestType == 3 || TestType == 4)//语法考察记忆生成
                    {
                        int[] match = new int[Buff_size];//以match中存储的数据顺序读内容
                        int [] copy = new int[Buff_size];//用作标记
                        for (int i = 0; i < Buff_size; i++)
                        {
                            match[i] = i;
                            copy[i] = SourceBuff[SBpointer + i] > 0 ? SourceBuff[SBpointer + i] : -SourceBuff[SBpointer + i]; ;
                        }
                        //Console.WriteLine("开始");
                        //for (int i = 0; i < Buff_size; i++)
                        //    Console.WriteLine(match[i]+"---"+copy[i]);
                        //Console.WriteLine("结束");
                        Buff_count = (SBcount - SBpointer >= Buff_size) ? Buff_size : SBcount - SBpointer;
                        for (int m = 0; m < Buff_count; m++)
                        {
                            int k = copy[m];
                            int place = m;
                            for (int n = m+1; n < Buff_count; n++)
                            {
                                if (k > copy[n])
                                {
                                    place = n;
                                    k = copy[n];
                                }
                            }
                            if (place != m)
                            {
                                int tmp1 = copy[m];
                                int tmp2 = match[m];
                                copy[m] = copy[place];
                                match[m] = match[place];
                                copy[place] = tmp1;
                                match[place] = tmp2;
                            }
                        }//排序完成
                        //Console.WriteLine("开始");
                        //for (int i = 0; i < Buff_size; i++)
                        //    Console.WriteLine(match[i]);
                        //Console.WriteLine("结束");
                        int count = 0;
                        int handle_id = SourceBuff[match[0] + SBpointer] > 0 ? SourceBuff[match[0] + SBpointer] : -SourceBuff[match[0] + SBpointer];
                        
                        //此处与上面不一样的地方在于只从一个文件中读取数据
                        Bpointer = 0;
                        while (count < Buff_count)
                        {
                            string realpath = Path + "sentencetest.txt";
                            //Console.WriteLine(realpath);
                            FileStream fs = new FileStream(realpath, FileMode.Open);
                            StreamReader sr = new StreamReader(fs, Encoding.Unicode);
                            String stmp1 = "";
                            int tcount = 0;
                            stmp1 = sr.ReadLine();
                            while (stmp1 != null)
                            {
                                //Console.WriteLine(handle_id);
                                if (stmp1.Contains("##16"))
                                { break; }
                                if (stmp1.Contains("##"))
                                { stmp1 = sr.ReadLine();
                                continue;
                                }
                                tcount++;
                                if (tcount == handle_id)//找到对应行
                                {
                                    //读入缓冲
                                    Buff[match[count]] = stmp1;
                                    //Console.WriteLine("stmp1:"+stmp1+" count:"+count+"match[count]"+match[count]);
                                    count++;
                                    if (count >= Buff_count)
                                    {
                                        SBpointer += Buff_count;
                                        sr.Close();
                                        fs.Close();
                                        return 1;
                                    }
                                    //检测下一行
                                    handle_id = SourceBuff[match[count] + SBpointer] > 0 ? SourceBuff[match[count] + SBpointer] : -SourceBuff[match[count] + SBpointer];
                                    //Console.WriteLine(handle_id);
                                    if (tcount == handle_id)
                                    {
                                        while (handle_id == tcount)
                                        {
                                            Buff[match[count]] = stmp1;
                                            //Console.WriteLine("stmp1:" + stmp1 + " count:" + count + "match[count]" + match[count]);
                                            count++;
                                            if (count >= Buff_count)
                                            {
                                                SBpointer += Buff_count;
                                                sr.Close();
                                                fs.Close();
                                                return 1;
                                            }
                                            handle_id = SourceBuff[match[count] + SBpointer] > 0 ? SourceBuff[match[count] + SBpointer] : -SourceBuff[match[count] + SBpointer];
                                            //Console.WriteLine(handle_id);
                                        }
                                    }
                                }
                                stmp1 = sr.ReadLine();
                            }
                            sr.Close();
                            fs.Close();
                        }
                        SBpointer += Buff_count;//把指针向后移动
                        //
                        //Console.WriteLine("开始");
                        //for (int i = 0; i < Buff_count; i++)
                        //   Console.WriteLine(i+":"+Buff[i]);
                        //Console.WriteLine("结束");
                        //
                    }
                    if (TestType == 5)//动词形态
                    {
                        int[] match = new int[Buff_size];//以match中存储的数据顺序读内容
                        int[] copy = new int[Buff_size];//用作标记
                        for (int i = 0; i < Buff_size; i++)
                        {
                            match[i] = i;
                            copy[i] = SourceBuff[SBpointer + i] > 0 ? SourceBuff[SBpointer + i] : -SourceBuff[SBpointer + i]; ;
                        }
                        //Console.WriteLine("开始");
                        //for (int i = 0; i < Buff_size; i++)
                        //    Console.WriteLine(match[i]+"---"+copy[i]);
                        //Console.WriteLine("结束");
                        Buff_count = (SBcount - SBpointer >= Buff_size) ? Buff_size : SBcount - SBpointer;
                        for (int m = 0; m < Buff_count; m++)
                        {
                            int k = copy[m];
                            int place = m;
                            for (int n = m + 1; n < Buff_count; n++)
                            {
                                if (k > copy[n])
                                {
                                    place = n;
                                    k = copy[n];
                                }
                            }
                            if (place != m)
                            {
                                int tmp1 = copy[m];
                                int tmp2 = match[m];
                                copy[m] = copy[place];
                                match[m] = match[place];
                                copy[place] = tmp1;
                                match[place] = tmp2;
                            }
                        }//排序完成
                        //Console.WriteLine("开始");
                        //for (int i = 0; i < Buff_size; i++)
                        //    Console.WriteLine(match[i]);
                        //Console.WriteLine("结束");
                        int count = 0;
                        int handle_id = SourceBuff[match[0] + SBpointer] > 0 ? SourceBuff[match[0] + SBpointer] : -SourceBuff[match[0] + SBpointer];

                        //此处与上面不一样的地方在于只从一个文件中读取数据
                        Bpointer = 0;
                        while (count < Buff_count)
                        {
                            string realpath = Path + "wordmod.txt";
                            //Console.WriteLine(realpath);
                            FileStream fs = new FileStream(realpath, FileMode.Open);
                            StreamReader sr = new StreamReader(fs, Encoding.Unicode);
                            String stmp1 = "";
                            int tcount = 0;
                            stmp1 = sr.ReadLine();
                            while (stmp1 != null)
                            {
                                //Console.WriteLine(handle_id);
                                if (stmp1.Contains("##1"))
                                {
                                    tcount = -1;
                                }
                                if (stmp1.Contains("##2"))
                                { break; }
                                tcount++;
                                if (tcount == handle_id)//找到对应行
                                {
                                    //读入缓冲
                                    Buff[match[count]] = stmp1;
                                    //Console.WriteLine("stmp1:"+stmp1+" count:"+count+"match[count]"+match[count]);
                                    count++;
                                    if (count >= Buff_count)
                                    {
                                        SBpointer += Buff_count;
                                        sr.Close();
                                        fs.Close();
                                        return 1;
                                    }
                                    //检测下一行
                                    handle_id = SourceBuff[match[count] + SBpointer] > 0 ? SourceBuff[match[count] + SBpointer] : -SourceBuff[match[count] + SBpointer];
                                    //Console.WriteLine(handle_id);
                                    if (tcount == handle_id)
                                    {
                                        while (handle_id == tcount)
                                        {
                                            Buff[match[count]] = stmp1;
                                            //Console.WriteLine("stmp1:" + stmp1 + " count:" + count + "match[count]" + match[count]);
                                            count++;
                                            if (count >= Buff_count)
                                            {
                                                SBpointer += Buff_count;
                                                sr.Close();
                                                fs.Close();
                                                return 1;
                                            }
                                            handle_id = SourceBuff[match[count] + SBpointer] > 0 ? SourceBuff[match[count] + SBpointer] : -SourceBuff[match[count] + SBpointer];
                                            //Console.WriteLine(handle_id);
                                        }
                                    }
                                }
                                stmp1 = sr.ReadLine();
                            }
                            sr.Close();
                            fs.Close();
                        }
                        SBpointer += Buff_count;//把指针向后移动
                    }
                    else
                    { return -1; }
                }
                else
                {
                    if (TestType == 1)//课内随机生成（单词）
                    {
                        //对于课内随机生成的情况，读取所有对应内容至缓冲区
                        //生成策略为完全随机，即题目随机，答案随机，在一次测验中可能考察相同的知识点多次
                        if (ClassNum < 1 || ClassNum > class_max)
                            return -1;
                        string realpath = Path + "test" + ClassNum.ToString() + ".txt";
                        //Console.WriteLine(realpath);
                        FileStream fs = new FileStream(realpath, FileMode.Open);
                        StreamReader sr = new StreamReader(fs, Encoding.Unicode);
                        //Console.WriteLine("break point for test........");
                        String tmpstr = "";
                        int count = 0;
                        tmpstr = sr.ReadLine();
                        while (tmpstr != null)
                        {
                            if (tmpstr.Contains("##1"))
                                count = -1;
                            if (tmpstr.Contains("##2"))//单词部分结束
                            {break; }
                            count++;
                            if (count > 0 && count <= Buff_size)//单词条目数目小于等于缓冲区数目
                            {
                                int i = 100 * ClassNum + count;
                                Buff[count - 1] = i.ToString() + "#" + tmpstr;
                            }
                            if (count > Buff_size)//如果单词的条目超过40，这种情况极少，将后来的单词随机覆盖前面的条目
                            {
                                int i = 100 * ClassNum + count;
                                Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
                                int j = ra.Next(1, Buff_size + 1);
                                Buff[j - 1] = i.ToString() + "#" + tmpstr;
                            }
                            tmpstr = sr.ReadLine();
                        }
                        Buff_count = (count > 40) ? Buff_size : count;//缓冲区中存有的题数
                        sr.Close();
                        fs.Close();
                    }
                    else if (TestType == 2)//教材随机生成（单词） 
                    {
                        //对于教材随机，首先随机出课文号一共n个，然后分类统计，计算不同课文下需要抽查的数目，再随机出课文内
                        //条目，导入内存，存在反复读写文件的问题
                        //题目生成也为随机生成
                        if (TestCount <= 0)
                        { return -1; }
                        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
                        int[] class_nums = new int[Buff_size];
                        int[] class_count = new int[class_max];
                        for (int i = 0; i < class_max; i++)
                            class_count[i] = 0;
                        for (int i = 0; i < Buff_size; i++)
                        {
                            class_nums[i] = ra.Next(1, class_max + 1);
                            class_count[class_nums[i] - 1]++;
                        }

                        int count = 0;
                        int real_count = Buff_size;
                        for (int i = 0; i < class_max; i++)
                        {
                            if (class_count[i] > 0)
                            {
                                int tmp = class_count[i];
                                string realpath = Path + "test" + (i + 1).ToString() + ".txt";
                                //Console.WriteLine(realpath);
                                FileStream fs = new FileStream(realpath, FileMode.Open);
                                StreamReader sr = new StreamReader(fs, Encoding.Unicode);
                                int ecount = 0;//课文内条目数
                                string etmp = "";
                                etmp = sr.ReadLine();
                                while (etmp != null)
                                {
                                    if (etmp.Contains("##1"))
                                    { ecount = -1; }
                                    if (etmp.Contains("##2"))
                                        break;
                                    ecount++;
                                    etmp = sr.ReadLine();
                                }
                                sr.Close();
                                fs.Close();
                                Random ran = new Random(unchecked((int)DateTime.Now.Ticks));
                                int[] find = new int[tmp];
                                for (int j = 0; j < tmp; j++)
                                {
                                    find[j] = ran.Next(1, ecount + 1);
                                }
                                const int cir_limit = 10;//最大允许10次生成失误
                                int cir_count = 0;
                                bool flg = false;
                                while (cir_count < cir_limit)//只要有重复数据就不断排序
                                {
                                    cir_count++;
                                    flg = false;
                                    for (int m = 0; m < tmp; m++)
                                    {
                                        bool flag = false;
                                        int k = find[m];
                                        int place = m;
                                        for (int n = m + 1; n < tmp; n++)
                                        {
                                            if (find[n] < k)
                                            {
                                                k = find[n];
                                                place = n;
                                            }
                                            if (find[n] == k)//如果随机出的数据中有同样的数据，对后一个数据进行重新生成
                                            {
                                                find[n] = ran.Next(1, ecount + 1);
                                                flg = true;//重新排序
                                                flag = true;//中断当前排序
                                                break;
                                            }
                                        }
                                        if (flag)
                                            break;
                                        if (place != m)
                                        {
                                            int swap = -1;
                                            swap = find[m];
                                            find[m] = find[place];
                                            find[place] = swap;
                                        }
                                    }
                                    if (!flg)//flg为false代表排序成功
                                        break;
                                }
                                if (cir_count >= cir_limit && flg == true)//放弃本课的抽查:该课文抽查的量偏多了
                                {
                                    real_count -= class_count[i];
                                    class_count[i] = 0;
                                }
                                else//将抽查的内容读入缓冲区
                                {
                                    FileStream fs1 = new FileStream(realpath, FileMode.Open);
                                    StreamReader sr1 = new StreamReader(fs1, Encoding.Unicode);
                                    string stmp1 = "";
                                    int stmp2 = 0;
                                    int place = 0;
                                    stmp1 = sr1.ReadLine();
                                    while (stmp1 != null && place < tmp)
                                    {
                                        if (stmp1.Contains("##1"))
                                        { stmp2 = -1; }
                                        if (stmp1.Contains("##2"))
                                            break;
                                        stmp2++;
                                        if (stmp2 == find[place])
                                        {
                                            Buff[count] = (100 * (i + 1) + stmp2).ToString() + "#" + stmp1;
                                            count++;
                                            place++;
                                        }
                                        stmp1 = sr1.ReadLine();
                                    }
                                    sr1.Close();
                                    fs1.Close();
                                }
                            }
                            if (count >= real_count)
                                break;

                        }
                        Buff_count = count;
                    }
                    else if (TestType == 3||TestType==4)//随机生成（语法）（超出部分随机替换）
                    {

                        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
                        string realpath = Path + "sentencetest.txt";
                        //Console.WriteLine(realpath);
                        FileStream fs = new FileStream(realpath, FileMode.Open);
                        StreamReader sr = new StreamReader(fs, Encoding.Unicode);
                        //Console.WriteLine("break point for test........");
                        String tmpstr = "";
                        int count = 0;
                        tmpstr = sr.ReadLine();
                        while (tmpstr != null)
                        {
                            //Console.WriteLine(count);
                            if (tmpstr.Contains("##1#"))
                                count = -1;
                            if (tmpstr.Contains("##16"))
                            {
                                break;
                            }
                            if (tmpstr.Contains("##"))//跳过含有##行的内容
                            {
                                tmpstr = sr.ReadLine();
                                continue; }
                            
                            count++;
                            if (count > 0 && count <= Buff_size)//条目数目小于等于缓冲区数目 (行号#内容)
                            {
                                Buff[count - 1] = count.ToString() + "#" + tmpstr;
                                //Console.WriteLine(Buff[count - 1]);
                            }
                            if (count > Buff_size)//如果单词的条目超过40，这种情况极少，将后来的单词随机覆盖前面的条目
                            {
                                int j = ra.Next(1, Buff_size + 1);
                                Buff[j - 1] = count.ToString() + "#" + tmpstr;
                                //Console.WriteLine(count+"行替换了"+j+"位置的内容");
                            }
                            tmpstr = sr.ReadLine();
                        }
                        Buff_count = (count > 40) ? Buff_size : count;//缓冲区中存有的题数
                        sr.Close();
                        fs.Close();
                        //
                        //Console.WriteLine("开始");
                        //for (int i = 0; i < Buff_count; i++)
                        //    Console.WriteLine(i + ":" + Buff[i]);
                        //Console.WriteLine("结束");
                        //

                    }
                    else if (TestType == 5)//动词活用生成（超出部分随机替换)
                    {
                        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
                        string realpath = Path + "wordmod.txt";
                        FileStream fs = new FileStream(realpath, FileMode.Open);
                        StreamReader sr = new StreamReader(fs, Encoding.Unicode);
                        String tmpstr = "";
                        int count = 0;
                        tmpstr = sr.ReadLine();
                        while (tmpstr != null)
                        {
                            if (tmpstr.Contains("##1"))
                                count = -1;
                            if (tmpstr.Contains("##2"))
                                break;
                            count++;//计算行数
                            if (count > 0 && count <= Buff_size)//条目数目小于等于缓冲区数目 (行号#内容)
                            {
                                Buff[count - 1] = count.ToString() + "#" + tmpstr;
                                //Console.WriteLine(Buff[count - 1]);
                            }
                            if (count > Buff_size)//后来的内容随机覆盖前面的条目
                            {
                                int j = ra.Next(1, Buff_size + 1);
                                Buff[j - 1] = count.ToString() + "#" + tmpstr;
                                //Console.WriteLine(count+"行替换了"+j+"位置的内容");
                            }
                            tmpstr = sr.ReadLine();
                        }
                        Buff_count = (count > 40) ? Buff_size : count;
                        sr.Close();
                        fs.Close();
                        
                    }
                    else { return -1; }//不可能的情况
                }
            }
            catch (Exception e)
            { 
                //Console.WriteLine(e.StackTrace);
                return -1;
            }
            return 0;
        }

        public int Sample_refresh()//L267
            //刷新题目，从缓冲区中抽取一个题作为考查点，抽三个另外的作为错误答案到Sample和Source
        {
            try
            {
                if (!init_flag)
                    return -1;//未初始化
                if (Buff_count < 2)
                    return -2;//题量过少
                if (TestMode)
                {
                    if (TestType == 1 || TestType == 2)
                    {
                        int deta = NumLimit[TestType];
                        int SBstart=SBpointer-Buff_count+Bpointer;
                        //Console.WriteLine("SBpointer:"+SBpointer+"Bpointer"+Bpointer+"SBstart"+SBstart);
                        if (SBstart < 0) return -1;
                        if (deta + Bpointer <= Buff_count)
                        {
                            for (int i = 0; i < deta; i++)
                            {
                                string answer = Buff[Bpointer + i];
                                int p = answer.IndexOf("#");
                                Samples[i+1] = answer.Substring(p+1).Replace("#"," ");
                                if (SourceBuff[SBstart + i] < 0)//正确答案
                                {
                                    ans = -SourceBuff[SBstart + i];
                                    Source[i] = -SourceBuff[SBstart + i];
                                    Samples[0] = answer.Substring(0,p);
                                }
                                else
                                    Source[i] = SourceBuff[SBstart + i];
                            }
                            Bpointer += deta;
                        }
                        else
                        {
                            //Console.WriteLine("会进入到else中");
                            
                            SBpointer = SBstart;
                            Buff_refresh();
                            for (int i = 0; i < deta; i++)
                            {
                                string answer = Buff[Bpointer + i];
                                int p = answer.IndexOf("#");
                                Samples[i + 1] = answer.Substring(p + 1).Replace("#", " ");
                                if (SourceBuff[SBstart + i] < 0)//正确答案
                                {
                                    ans = -SourceBuff[SBstart + i];
                                    Source[i] = -SourceBuff[SBstart + i];
                                    Samples[0] = answer.Substring(0, p);
                                }
                                else
                                    Source[i] = SourceBuff[SBstart + i];
                            }
                            Bpointer += deta;
                            /*
                            SBpointer = SBstart;
                            Buff_refresh();
                            for (int i = 0; i < deta; i++)
                            {
                                string answer = Buff[Bpointer + i];
                                int p = answer.IndexOf("#");
                                answer = answer.Substring(p + 1);
                                p = answer.IndexOf("#");
                                Samples[i] = answer.Substring(0, p).Replace("#", " ");
                                if (SourceBuff[SBstart + i] < 0)//正确答案
                                {
                                    ans = -SourceBuff[SBstart + i];
                                    Source[i] = -SourceBuff[SBstart + i];
                                }
                                else
                                    Source[i] = SourceBuff[SBstart + i];
                            }
                            Bpointer += deta;
                            */
                        }
                    }
                    else if (TestType == 3 || TestType == 4)
                    {
                        //
                        //Console.WriteLine("开始");
                        //for (int i = 0; i < Buff_count; i++)
                        //    Console.WriteLine(i + ":" + Buff[i]);
                        //Console.WriteLine("结束");
                        //
                        int deta = NumLimit[TestType];
                        int SBstart = SBpointer - Buff_count + Bpointer;
                        //Console.WriteLine("SBpointer:"+SBpointer+"Bpointer"+Bpointer+"SBstart"+SBstart);
                        if (SBstart < 0) return -1;
                        if (deta + Bpointer <= Buff_count)
                        {
                            for (int i = 0; i < deta; i++)
                            {
                                string answer = Buff[Bpointer + i];
                                int p = answer.IndexOf("#");
                                Samples[i + 1] = answer.Substring(p + 1).Replace("#", " ");
                                if (SourceBuff[SBstart + i] < 0)//正确答案
                                {
                                    ans = -SourceBuff[SBstart + i];
                                    Source[i] = -SourceBuff[SBstart + i];
                                    Samples[0] = answer.Substring(0, p).Replace("@","？");
                                }
                                else
                                    Source[i] = SourceBuff[SBstart + i];
                            }
                            Bpointer += deta;
                        }
                        else
                        {
                            SBpointer = SBstart;
                            Buff_refresh();
                            for (int i = 0; i < deta; i++)
                            {
                                string answer = Buff[Bpointer + i];
                                int p = answer.IndexOf("#");
                                Samples[i + 1] = answer.Substring(p + 1).Replace("#", " ");
                                if (SourceBuff[SBstart + i] < 0)//正确答案
                                {
                                    ans = -SourceBuff[SBstart + i];
                                    Source[i] = -SourceBuff[SBstart + i];
                                    Samples[0] = answer.Substring(0, p).Replace("@", "？");
                                }
                                else
                                    Source[i] = SourceBuff[SBstart + i];
                            }
                            Bpointer += deta;
                        }
                        //Console.WriteLine("sample中为" + Samples[0] + "+" + Samples[1] + "+" + Samples[2] + "+" + Samples[3] + "+" + Samples[4]);
                        //Console.WriteLine(ans);
                    }
                    else if (TestType == 5)
                    {
                        int deta = NumLimit[TestType];
                        int SBstart = SBpointer - Buff_count + Bpointer;
                        //Console.WriteLine("SBpointer:"+SBpointer+"Bpointer"+Bpointer+"SBstart"+SBstart);
                        if (SBstart < 0) return -1;
                        if (deta + Bpointer <= Buff_count)
                        {
                            string stmp = Buff[Bpointer];//得到的数据串，不含行号
                            int p0 = stmp.IndexOf("#");
                            Samples[0] = stmp.Substring(0, p0);
                            stmp = stmp.Substring(p0 + 1);
                            p0 = stmp.IndexOf("#");
                            Samples[1] = stmp.Substring(0, p0);
                            stmp = stmp.Substring(p0 + 1);
                            p0 = stmp.IndexOf("#");
                            Samples[2] = stmp.Substring(0, p0);
                            stmp = stmp.Substring(p0 + 1);
                            p0 = stmp.IndexOf("#");
                            Samples[3] = stmp.Substring(0, p0);
                            stmp = stmp.Substring(p0 + 1);
                            p0 = stmp.IndexOf("#");
                            Samples[4] = stmp.Substring(0, p0);
                            for (int i = 0; i < deta; i++)
                            {
                                if (SourceBuff[SBstart + i] < 0)
                                    ans = i + 1;
                            }
                            Bpointer += deta;
                            //Console.WriteLine("ans:" + ans);
                            //Console.WriteLine("source:" + SourceBuff[SBstart] + "--" + SourceBuff[SBstart + 1] + "--" + SourceBuff[SBstart + 2] + "--" + SourceBuff[SBstart+3]);
                            //Console.WriteLine("sample:" + Samples[0] + "--" + Samples[1] + "--" + Samples[2] + "--" + Samples[3] + "--" + Samples[4]);
                        }
                        else
                        {
                            SBpointer = SBstart;
                            Buff_refresh();
                            string stmp = Buff[Bpointer];//得到的数据串，不含行号
                            int p0 = stmp.IndexOf("#");
                            Samples[0] = stmp.Substring(0, p0);
                            stmp = stmp.Substring(p0 + 1);
                            p0 = stmp.IndexOf("#");
                            Samples[1] = stmp.Substring(0, p0);
                            stmp = stmp.Substring(p0 + 1);
                            p0 = stmp.IndexOf("#");
                            Samples[2] = stmp.Substring(0, p0);
                            stmp = stmp.Substring(p0 + 1);
                            p0 = stmp.IndexOf("#");
                            Samples[3] = stmp.Substring(0, p0);
                            stmp = stmp.Substring(p0 + 1);
                            p0 = stmp.IndexOf("#");
                            Samples[4] = stmp.Substring(0, p0);
                            for (int i = 0; i < deta; i++)
                            {
                                if (SourceBuff[SBstart + i] < 0)
                                    ans = i + 1;
                            }
                            Bpointer += deta;
                            //Console.WriteLine("ans:" + ans);
                            //Console.WriteLine("source:" + SourceBuff[SBstart] + "--" + SourceBuff[SBstart + 1] + "--" + SourceBuff[SBstart + 2] + "--" + SourceBuff[SBstart + 3]);
                            //Console.WriteLine("sample:" + Samples[0] + "--" + Samples[1] + "--" + Samples[2] + "--" + Samples[3] + "--" + Samples[4]);
                        }
                    }
                    else
                    { return -1; }

                }
                else
                {
                    if (TestType == 1 || TestType == 2)
                    {
                        int[] choose_num = new int[Source_size];
                        int choose_c = 0;
                        int safe = 0;
                        const int safe_limit = 20;
                        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
                        for (int i = 0; i < Source_size; i++)
                        {
                            safe++;
                            if (safe >= safe_limit) return -1;//出错次数过多，本次刷新失败

                            int tmp = ra.Next(1, Buff_count);
                            for (int j = 0; j < choose_c; j++)
                            {
                                if (tmp == choose_num[j])
                                {
                                    i--;
                                    break;
                                }
                            }
                            choose_num[i] = tmp;
                            choose_c = i + 1;
                            if (choose_c >= Buff_count)//假设buff_count=2,则会生成四次，中间因为多次生成同样的点导致死循环
                            {
                                break;
                            }
                        }
                        //已经生成了要考察的四个题目号
                        int answer = -1;
                        answer = ra.Next(1, choose_c + 1);//L304
                        if (answer == -1)
                            return -1;//error
                        //Console.WriteLine("choose_c:"+choose_c);
                        for (int j = 0; j < choose_c; j++)
                        {
                            if (j == answer - 1)//该条目为正确答案
                            {
                                //----1011#abc#qwe#qwe#
                                //Console.WriteLine(j + "..." + choose_num[j] + "..." + Buff[choose_num[j]]);
                                string tmp = Buff[choose_num[j]];
                                int p1 = tmp.IndexOf("#");
                                //Console.WriteLine(p1);
                                string ans_id = tmp.Substring(0, p1);//Line 316
                                string tmp1 = tmp.Substring(p1 + 1);
                                int p2 = tmp1.IndexOf("#");
                                string tmp2 = tmp1.Substring(0, p2);
                                //Console.WriteLine(p2 + "..." + tmp2);
                                string tmp3 = tmp1.Substring(p2 + 1);
                                tmp3 = tmp3.Replace("#", " ").Trim();
                                ans = Convert.ToInt32(ans_id);//LINE 327
                                Source[j] = ans;
                                Samples[j + 1] = tmp3;
                                Samples[0] = tmp2;
                            }
                            else
                            {
                                string tmp = Buff[choose_num[j]];
                                //Console.WriteLine(tmp);
                                //Console.WriteLine(choose_num[j] + "..." + j);
                                int p1 = tmp.IndexOf("#");
                                string ans_id = tmp.Substring(0, p1);//L333
                                string tmp1 = tmp.Substring(p1 + 1);
                                //Console.WriteLine(tmp1 + "..." + ans_id);
                                int p2 = tmp1.IndexOf("#");
                                //string tmp2 = tmp1.Substring(0, p2);
                                string tmp3 = tmp1.Substring(p2 + 1);
                                tmp3 = tmp3.Replace("#", " ");
                                Source[j] = Convert.ToInt32(ans_id);
                                Samples[j + 1] = tmp3;
                            }
                        }
                    }
                    else if (TestType == 3 || TestType == 4)
                    {
                        int[] choose_num = new int[Source_size];//存放选择行的数组
                        int choose_c = 0;//存放选择的真正行数
                        int safe = 0;
                        const int safe_limit = 20;
                        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
                        for (int i = 0; i < Source_size; i++)
                        {
                            safe++;
                            if (safe >= safe_limit) return -1;//出错次数过多，本次刷新失败
                            bool flg = false;
                            int tmp = ra.Next(1, Buff_count);
                            //Console.WriteLine("tmp=:"+tmp);
                            for (int j = 0; j < choose_c; j++)
                            {
                                if (tmp == choose_num[j])
                                {
                                    i--;
                                    flg = true ;
                                    break;
                                }
                            }
                            if(!flg)
                            choose_num[i] = tmp;
                            choose_c = i + 1;
                            if (choose_c >= Buff_count)//假设buff_count=2,则会生成四次，中间因为多次生成同样的点导致死循环
                            {
                                break;
                            }
                        }
                        
                        int answer = -1;
                        answer = ra.Next(1, choose_c + 1);
                        if (answer == -1)
                            return -1;//error
                        //Console.WriteLine("随机的答案序号为："+answer);
                        //Console.WriteLine("choose_c为：" + choose_c);
                        //Console.WriteLine("choose_num为：" + choose_num[0]+choose_num[1]+choose_num[2]+choose_num[3]+"end");
                        for (int j = 0; j < choose_c; j++)
                        {
                            if (j == answer - 1)//该条目为正确答案
                            {
                                //----1#abcqwe#qwe#
                                string tmp = Buff[choose_num[j]];
                                int p1 = tmp.IndexOf("#");
                                string ans_id = tmp.Substring(0, p1);
                                string tmp1 = tmp.Substring(p1 + 1);
                                int p2 = tmp1.IndexOf("#");
                                string tmp2 = tmp1.Substring(0, p2);
                                string tmp3 = tmp1.Substring(p2 + 1);
                                tmp3 = tmp3.Replace("#", "");
                                ans = Convert.ToInt32(ans_id);
                                Source[j] = ans;
                                Samples[j + 1] = tmp3;
                                Samples[0] = tmp2.Replace("@","？");
                            }
                            else
                            {
                                string tmp = Buff[choose_num[j]];
                                int p1 = tmp.IndexOf("#");
                                string ans_id = tmp.Substring(0, p1);
                                string tmp1 = tmp.Substring(p1 + 1);
                                int p2 = tmp1.IndexOf("#");
                                string tmp3 = tmp1.Substring(p2 + 1);
                                tmp3 = tmp3.Replace("#", "");
                                Source[j] = Convert.ToInt32(ans_id);
                                Samples[j + 1] = tmp3;
                            }
                        }
                        //Console.WriteLine("sample中为" + Samples[0] + "+" + Samples[1] + "+" + Samples[2] + "+" + Samples[3] + "+" + Samples[4]);
                        //Console.WriteLine(ans);
                        //Console.WriteLine();
                    }
                    else if (TestType == 5)
                    {
                        int choose = 1;
                        int p0 = 0;
                        Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
                        choose = ra.Next(1,Buff_count+1);
                        string stmp = Buff[choose-1];
                        int lineid = 0;
                        p0 = stmp.IndexOf("#");
                        lineid = Convert.ToInt32(stmp.Substring(0,p0));
                        stmp = stmp.Substring(p0+1);
                        p0 = stmp.IndexOf("#");
                        Samples[0] = stmp.Substring(0,p0);
                        stmp = stmp.Substring(p0+1);
                        p0 = stmp.IndexOf("#");
                        Samples[1] = stmp.Substring(0, p0);
                        stmp = stmp.Substring(p0 + 1);
                        p0 = stmp.IndexOf("#");
                        Samples[2] = stmp.Substring(0, p0);
                        stmp = stmp.Substring(p0 + 1);
                        p0 = stmp.IndexOf("#");
                        Samples[3] = stmp.Substring(0, p0);
                        stmp = stmp.Substring(p0 + 1);
                        p0 = stmp.IndexOf("#");
                        Samples[4] = stmp.Substring(0, p0);
                        ans = ra.Next(1,5);//1-4
                        for (int i = 0; i < Source_size; i++)
                        {
                            Source[i] = lineid;
                        }
                        //Console.WriteLine("ans:"+ans);
                        //Console.WriteLine("source:" + Source[0] +"--"+ Source[1] +"--"+ Source[2]+"--"+Source[3]);
                        //Console.WriteLine("sample:" + Samples[0] + "--" + Samples[1] + "--" + Samples[2] + "--" + Samples[3]+"--"+Samples[4]);
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.StackTrace);
                return -1;
            }
            return 0;
        }
        
        public string GetSample(int place)//0-4
        {
            if (place < Sample_size && place >= 0)
                return Samples[place];
            else
                return "";
        }

        public int GetSource(int id)//1-4
        {
            if (id > 0 && id <= Source_size)
                return Source[id-1];
            else
                return 0;
        }

        public int Get_ans()
        {
            return ans;
        }

        public int all_clear()//清空内存中所有从文件生成的数据,不包括测试类型，测试次数，以及课文号
        {
            ans = 0;
            for (int i = 0; i < Sample_size; i++)
                Samples[i] = "";
            for (int j = 0; j < Source_size; j++)
                Source[j] = 0;
            for (int k = 0; k < Buff_size; k++)
                Buff[k] = "";
            Buff_count = 0;
            Bpointer = 0;
            //SBcount = 0;
            //SBpointer = 0;实验表明不能清除SBpointer
            return 0;
        }

        public void set_config(int testtype, int testcount, int classnum = 0)//重新生成该对象,但是不刷新缓冲区和当前题目
        {
            if (testtype > 0)
            {
                TestMode = false;
                TestType = testtype;
            }
            else
            {
                TestMode = true;
                TestType = -testtype;
            }
            TestCount = (testcount > TestCountLimit) ? TestCountLimit : testcount;
            ClassNum = classnum;
            init_flag = true;

            all_clear();
        }

        //记忆曲线实现部分所用变量
        bool TestMode;//用于标明是记忆曲线模式还是普通模式
        int[] NumLimit = {4,4,4,4,4,4,0};//不同模式下需要记录的答案数,第一个4是无用的
        const int MaxNL = 4;//最大单次需要记录的答案数
        int[] SourceBuff;//用于存储所有的生成的题目的来源
        int SBcount;//用于存放生成的区域大小
        const int SBLimit = MaxNL * TestCountLimit;//上限120
        int SBpointer;//指向当前写入部分
        const string dataPath = @"..\..\..\data\";
        //只有在用户做完题的时候（刷新前）才把题目存入缓冲区，正确的题号置为负数即可标志答案
        //添加一个新函数来把Source缓冲区的数据写到SB缓冲区
        //添加一个函数使用SB缓冲区中的数据来生成对应的记录文本文件保存(需要包含时间用于实现记忆曲线)
        public bool getMode()
        {
            return TestMode;
        }

        public void writeSB()//将source内数据写入到SBuff中
        {
            if (TestType == 5)
            {
                int deta = NumLimit[TestType];
                if (SBpointer + deta <= SBcount)//当前位置加上增量不超过缓冲区大小
                {
                    for (int i = 0; i < deta; i++)
                    {
                        if (ans-1 == i)
                        {
                            SourceBuff[SBpointer + i] = -Source[i];
                        }
                        else
                            SourceBuff[SBpointer + i] = Source[i];
                        //Console.WriteLine(SourceBuff[SBpointer]);
                    }
                    SBpointer += deta;
                }
            }
            else
            {
                int deta = NumLimit[TestType];
                if (SBpointer + deta <= SBcount)//当前位置加上增量不超过缓冲区大小
                {
                    for (int i = 0; i < deta; i++)
                    {
                        if (ans == Source[i])
                        {
                            SourceBuff[SBpointer + i] = -Source[i];
                        }
                        else
                            SourceBuff[SBpointer + i] = Source[i];
                        //Console.WriteLine(SourceBuff[SBpointer]);
                    }
                    SBpointer += deta;
                }
            }
        }

        public int product_log(String tid,DateTime tdatetime)//按照日期生成对应测试的记录文件，相同日期的写在同一文件中
        {
            //需要tid来标志唯一测试号，需要时间来生成文件名，其余信息可通过数据库查找
            //文件格式为                             |  --测试号（tid）--具体时刻（tdatetime）
            // 每一行为一次测试，数据量由TestType确定|  1204#1313#...等
            //写入顺序与SBuff中的顺序一致
            //记忆曲线生成需要先把相应信息还原到SBbuff中，在修改Buff_refresh的记忆生成改为从SBuff中读取题号
            //修改Sample中的记忆曲线生成改为从Buff中顺序按照读，因为Buff的上限是一定的，这个上限少于SBuff中记录数
            //因此在Sample刷新至Buff中数据用尽时要重新刷新Buff，Buff在此处的刷新策略按照SBpointer的数值来从文件中读取相应记录
            //并修改SBpointer的值
            //记忆曲线的题目是基于前面的
            try
            {
                if (!init_flag)
                    return -1;
                string filename = tdatetime.Date.ToString();
                int _i = filename.IndexOf(" ");
                filename = filename.Substring(0,_i);
                filename=filename.Replace("/","_");
                string filepath = dataPath + filename + ".txt";
                //Console.WriteLine(filename);
                //Console.WriteLine(filepath);
                FileStream fs = new FileStream(filepath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs,Encoding.Unicode);
                string title = "=" + tid + "=" + filename;
                //Console.WriteLine(title);
                int deta=NumLimit[TestType];
                sw.WriteLine(title);
                for (int i = 0; i < SBpointer; i+=deta)
                {
                    string tmp = "";
                    for (int j = 0; j < deta; j++)
                    {
                        //Console.WriteLine(i+j);
                        tmp = tmp + SourceBuff[i + j].ToString() + "#";
                        //Console.WriteLine(tmp);
                    }
                    sw.WriteLine(tmp);
                }
                sw.Close();
                fs.Close();
                return 1;
            }
            catch (Exception e)
            { //Console.WriteLine(e.StackTrace); 
            }
            return -1;
        }

        public int M_readinit(String tid, DateTime tdatetime)//用于记忆曲线初始化，找到文件中的对应的记录还原到SBuff
        {
            try
            {
                if (!init_flag)
                    return -1;
                //这里暂时不清楚加了对SBpointer的初始化有没问题
                SBpointer = 0;
                string filename = tdatetime.Date.ToString();
                int _i = filename.IndexOf(" ");
                filename = filename.Substring(0, _i);
                filename = filename.Replace("/", "_");
                string filepath = dataPath + filename+".txt";
                FileStream fs = new FileStream(filepath,FileMode.Open);
                StreamReader sr = new StreamReader(fs,Encoding.Unicode);
                int deta=NumLimit[TestType];
                string tmp = sr.ReadLine();
                bool read_flag = false;
                int count = -1;
                while (tmp != null)
                {
                    if (tmp.Contains("="))
                    {
                        int p1 = tmp.IndexOf("=");
                        string stmp1 = tmp.Substring(p1+1);
                        int p2 = stmp1.IndexOf("=");
                        string stmp2 = stmp1.Substring(0,p2);
                        if (tid.Equals(stmp2))
                        { read_flag = true; }
                        else
                            read_flag = false;
                    }
                    if (read_flag)
                    {
                        if (count >= 0)
                        {
                            for (int i = 0; i < deta; i++)
                            {
                                int k1 = tmp.IndexOf("#");
                                string temp = tmp.Substring(0, k1);
                                SourceBuff[SBpointer + i] = Convert.ToInt32(temp);
                                tmp = tmp.Substring(k1+1);
                            }
                            SBpointer += deta;
                           
                        }
                        count++;
                        if (count >= TestCount)//已经读取了足够数据
                            break;
                    }
                    tmp=sr.ReadLine();
                }
                if (SBpointer == NumLimit[TestType] * TestCount)
                {
                    SBcount = NumLimit[TestType] * TestCount;
                    SBpointer = 0;
                }
                else
                {
                    SBcount = SBpointer;
                    SBpointer = 0;
                }
                sr.Close();
                fs.Close();
                //
                //Console.WriteLine("开始");
                //for (int i = 0; i < SBcount; i++)
                //  Console.WriteLine(SourceBuff[i]);
                //Console.WriteLine("结束");
                //
            }
            catch (Exception e)
            { Console.WriteLine(e.StackTrace); }
            return 0;
        }
    }
}//abcde
