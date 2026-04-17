
namespace Architecture.App1.CloudResource.Hardware
{
    public class CPU : IHardware
    {
        public int Core { get; init; }

        public int ClockSpeed { get; init; }
        public int IPC { get; init; }

        public int Speed => Core * ClockSpeed * IPC;

        public int Price { get; set; }
        public override string ToString()
        {
            return $"Core {Core} , Speed {Speed} , Price {Price}";
        }
    }
}
