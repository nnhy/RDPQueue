using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLife.Log;

namespace RDPQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            //XTrace.UseConsole();
            MyService.ServiceMain();
        }
    }
}