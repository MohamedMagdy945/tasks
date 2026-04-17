namespace Architecture.App1.CloudResource.Hardware
{
    public class RAM : IHardware
    {
        public int SizeGB { get; init; }
        public int Price { get; set; }
        public override string ToString()
        {
            return $"SizeGB {SizeGB} , Price {Price}";
        }

    }
}
