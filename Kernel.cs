using System;
using System.IO;
using Sys = Cosmos.System;

namespace XylOS
{
    public class Kernel : Sys.Kernel
    {
        MainProgramm main;

        protected override void BeforeRun()
        {
            var fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            Terminal.Init();

            main = new MainProgramm();
        }

        protected override void Run()
        {
            if (main.run() != 0)
            {
                Console.WriteLine("KERNEL_ERROR. YOU CAN TURN OFF COMPUTER");
            }
        }
    }
}
