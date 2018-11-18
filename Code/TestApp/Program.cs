using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
           /* A a = new A();
            B b = new TestApp.B();
            a = b;
            b = a;

            var series = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                if (i < 2)
                {
                    series.Add(i);
                    continue;
                }

                 
            }


            var toPrint = string.Join(",", series);
            */
            int testNo = 100003;
            if (testNo < 3)
            {
                if(testNo == 2)
                    Console.WriteLine("Prime");
                else
                    Console.WriteLine("no prime");
            }
            else
            {
                if (testNo % 2 == 0)
                {
                    // Is the number even?  If yes it cannot be a prime
                    Console.WriteLine("{0} is not a prime", testNo);
                }

                int divideBy = 3;
               
                for (divideBy = 3; testNo % divideBy != 0 ; divideBy += 2)
                {

                }

                if(divideBy == testNo)
                    Console.WriteLine("Prime");
                else
                    Console.WriteLine("No prime");
            }

            Console.ReadLine();


        }
    }


    class A
    {

        public static implicit operator A(B b)
        {
            var a = new A();
            return a;
        }
    }

    class B
    {
        public static implicit operator B(A v)
        {
            return new B();  
        }

    }
}



