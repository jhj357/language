using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPapp
{
    class Memory
    {
        int[] Tm = {0,1,5,120,240,480,960,1680,3600 };//以hour记 放大十倍 最佳记忆点
        int[] Tmd = {0,2,3,60,120,240,480,600,600 };//以分钟记 偏差数列
        const int TLimit=9;
        DateTime upDate;//传入时间的偏差后上限
        DateTime downDate;//传入时间的偏差前上限
        DateTime dt;//传入的时间
        int Level;
        bool init_flg;

        public Memory(DateTime datetime)
        {
            Level = 0;
            dt = datetime;
        }

        public int compare(DateTime now,DateTime ago,int level)//对于给定的level,当前时间位于时间段的何处/-1/0/1
        {
            return 0;
        }

        public void refresh(int level)//以默认时间修改上下限
        {
            init_flg = true;
            Level = level;
            int deta1=Tm[level];
            int deta2 = Tmd[level];
            //upDate
            upDate = dt;
            upDate = upDate.AddHours(-(double)deta1/10);
            upDate = upDate.AddMinutes((double)deta2);
            //downDate
            downDate = dt;
            downDate = downDate.AddHours(-(double)deta1/10);
            downDate = downDate.AddMinutes(-(double)deta2);
        }

        public int Diff(DateTime now,DateTime ago)//计算两个时间点 差的等级
        {
            TimeSpan ts = now - ago;
            int time = ts.Hours*10;
            for (int i = 0; i < TLimit; i++)
            {
                if (time <= Tm[i])
                {
                    return i;
                }
            }
             return 9;
        }

        public DateTime getUp()
        {
            if (init_flg)
                return upDate;
            else
                return dt;
        }

        public DateTime getDown()
        {
            if (init_flg)
                return downDate;
            else
                return dt;
        }

        public int getTLimit()
        {
            return TLimit;
        }

        public DateTime getNextTime()
        {
            return dt; 
        }

        public DateTime getMemoryDate(int level)//
        {
            Level = level;
            int deta = Tm[level];
            //MemoryDate
            DateTime memorydate;
            memorydate= dt.AddHours((double)deta / 10);
            return memorydate;
        }

        public void min_refresh(int min)//以给定时间修改上下限
        {
            init_flg = true;
            //upDate
            upDate = dt;
            upDate = upDate.AddMinutes((double)min);
            //downDate
            downDate = dt;
            downDate = downDate.AddMinutes(-(double)min);
        }
    }
}
