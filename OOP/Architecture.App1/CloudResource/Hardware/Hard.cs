namespace Architecture.App1.CloudResource.Hardware
{
    public class Hard : IHardware
    {
        public int CapacityGB { get; init; }
        public int Price { get; set; }
        public bool IsSSD { get; set; }

        public override string ToString()
        {
            string hard = IsSSD ? "SSD" : "HDD";
            return $"type {hard} , Capacity {CapacityGB} , Price { Price }";
        }

    }
}
