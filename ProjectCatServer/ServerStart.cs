using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace ProjectCatServer
{
    public class ServerStart
    {
        public static void Main(string[] args)
        {
            var ctx = new SingleThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(ctx);

            Console.WriteLine("Main Thread: {0}", Thread.CurrentThread.ManagedThreadId);
            var server = new Server(IPAddress.Any, 9001);
            server.Run();

            ctx.RunMessagePump();
        }
    }
}
