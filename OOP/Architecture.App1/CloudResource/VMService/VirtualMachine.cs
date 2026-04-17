using Architecture.App1.CloudResource.Hardware;

namespace Architecture.App1.CloudResource.VMService
{
    public class VirtualMachine
    {
        public string Name { get; set; }    
        public RAM Memory { get; set; }
        public Hard Storage { get; set; }
        public CPU Processor { get; set; }
        public VirtualMachine (string name , CPU processor , RAM ram, Hard storage)
        {
            Name = name;
            Memory = ram;
            Storage = storage;
            Processor = processor;
        }
        public int CalculateMonthlyCost()
        {
            return Memory.Price + Processor.Price + Storage.Price ; 
        }

        public string VMInfo()
        {
            return $" Server Name : {Name} " +
                $"\n Processor : {Processor} " +
                $"\n Storage : {Storage} " +
                $"\n Ram {Memory}";
        }

    }
}
