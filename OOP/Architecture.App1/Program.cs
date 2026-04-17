using Architecture.App1.CloudResource.VMFactory;
using Architecture.App1.CloudResource.VMService;

namespace Architecture.App1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VMFactory vmFactory = new VMFactory();

            VirtualMachine  vmStandard = vmFactory.CreateStandardVM();
            VirtualMachine  vmGaming= vmFactory.CreateGamingVM();
            Console.WriteLine(vmStandard.VMInfo());
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(vmGaming.VMInfo());
        }
    }
}
