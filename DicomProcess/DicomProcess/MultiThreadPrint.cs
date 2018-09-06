using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatePrint;
using System.Threading;
using log4net;

namespace MultiThreadPrint
{
    class MultiThreadPrintClass
    {
        private SimulatePrintClass SimPrintObj = new SimulatePrintClass();
        CancellationTokenSource cts = new CancellationTokenSource();
        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void StartThreadTask()
        {
            
            SimPrintObj.init();
            TaskFactory taskFactory = new TaskFactory();
            int count = Int32.Parse(SimPrintObj.WaterMark.threadCount);
            Task[] tasks = new Task[count];

            for (int i = 1; i <= count; i++)
            {
                int taskID = i;
                tasks[i - 1] = Task.Factory.StartNew(() => AddTask(cts.Token, taskID));

                Console.Out.WriteLine("线程" + i + "启动中......"+"Thread" + i + "Start......");
                log.Info("Thread" + i + "start......");                
                Thread.Sleep(1000);
            }

            taskFactory.ContinueWhenAll(tasks, TasksEnded, CancellationToken.None);
            log.Info("All Threads start......");  
            check();
            
            Console.ReadKey();
            
            

        }



        public void AddTask(CancellationToken cancellationToken,int TaskID)
        {
           while (!cancellationToken.IsCancellationRequested)
                    {
                        SimPrintObj.SendDicom(TaskID);
                        Thread.Sleep(1000);
                    }
           Console.Out.WriteLine("线程：" + TaskID + " 已退出." + "Thread：" + TaskID + " has quit.");
           log.Debug("Thread：" + TaskID + " exit.");
        }



        public void check()
        {
            Console.Out.WriteLine("输入 \'Exit\' 退出程序-Input \'Exit\' to quit.");
            string input = Console.ReadLine();
            if (input.Equals("Exit"))
            {
                cts.Cancel();
                Console.Out.WriteLine("等待线程退出....Wait the thread to qiut...");
            }
            else
            {
                check();
            }
        }


        static void TasksEnded(Task[] tasks)
        {
            Console.WriteLine("所有线程已退出！All Threads are quit.");
            Console.Out.WriteLine("输入任何值退出....Input any values to quit");
        }




    }
}
