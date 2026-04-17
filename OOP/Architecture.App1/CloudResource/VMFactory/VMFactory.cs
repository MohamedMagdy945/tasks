using Architecture.App1.CloudResource.Hardware;
using Architecture.App1.CloudResource.VMService;

namespace Architecture.App1.CloudResource.VMFactory
{
    public class VMFactory
    {
        public VirtualMachine CreateStandardVM()
        {
            CPU cpu = new CPU() {  Core = 4 , IPC = 2 , ClockSpeed = 2 , Price = 200};
            RAM ram = new RAM { SizeGB = 16, Price = 300 };
            Hard hard  = new Hard { CapacityGB = 512  , IsSSD = false  , Price = 300 };
            return new VirtualMachine("webServer", cpu , ram, hard);    
        }
        public VirtualMachine CreateGamingVM()
        {
            CPU cpu = new CPU() { Core = 8, IPC = 2, ClockSpeed = 2, Price = 350 };
            RAM ram = new RAM { SizeGB = 64, Price = 800 };
            Hard hard = new Hard { CapacityGB = 512, IsSSD = true, Price = 300 };
            return new VirtualMachine("GamingServer", cpu, ram, hard);
        }
    }
}
