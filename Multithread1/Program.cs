using System.Threading;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;


public class App
{
    class MyBarrier
    {

        public static IEnumerable<string> FillData(int size)
        {

            List<string> data = new List<string>(size);

            Random r = new Random();

            for (int i = 0; i < size; i++)
            {

                data.Add(GetString(r));

            }

            return data;

        }

        //返回长度为6的小写字母  

        public static string GetString(Random r)
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {

                sb.Append((char)(r.Next(26) + 97));

            }

            return sb.ToString();

        }



        public static int[] CalculationInTask(object p)
        {

            var p1 = p as Tuple<int, int, Barrier, List<string>>;

            Barrier barrier = p1.Item3;

            List<string> data = p1.Item4;



            int start = p1.Item1 * p1.Item2;

            int end = start + p1.Item2;



            Console.WriteLine("Task {0}:partition from {1} to {2}", Task.CurrentId, start, end);

            int[] charCount = new int[26];



            for (int j = start; j < end; j++)
            {

                char c = data[j][0];

                charCount[c - 97]++;

            }

            Console.WriteLine("Calculation completed from task {0}.{1} times a,{2} times z", Task.CurrentId, charCount[0], charCount[25]);

            barrier.RemoveParticipant();

            Console.WriteLine("Task {0} removed from barrier,remaining participants {1}", Task.CurrentId, barrier.ParticipantsRemaining);

            return charCount;

        }



        public static void Go()
        {

            const int numberTasks = 2;

            const int partitionSize = 1000000;

            var data = new List<string>(FillData(partitionSize * numberTasks));



            var barrier = new Barrier(numberTasks + 1);



            var taskFactory = new TaskFactory();

            var tasks = new Task<int[]>[numberTasks];

            for (int i = 0; i < numberTasks; i++)
            {

                tasks[i] = taskFactory.StartNew<int[]>(CalculationInTask,

                    Tuple.Create(i, partitionSize, barrier, data));

            }



            barrier.SignalAndWait();

            var resultCollection = tasks[0].Result;



            char ch = 'a';

            int sum = 0;

            foreach (var x in resultCollection)
            {

                Console.WriteLine("{0}, count: {1}", ch++, x);

                sum += x;

            }



            Console.WriteLine("main finished {0}", sum);

            Console.WriteLine("remaining {0}, phase {1}", barrier.ParticipantsRemaining, barrier.CurrentPhaseNumber);



        }





    }  
    
    public static void Main(string[] args)
    {
        MyBarrier.Go();
        Console.ReadKey();
    }
}