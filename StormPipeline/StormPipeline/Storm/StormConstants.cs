namespace Com.Apdcomms.StormPipeline.Storm
{
    public static class StormConstants
    {
        public static class Indices
        {
            public const int
                MessageCode = 3,
                UniqueIdentifier = 4;                
        }

        public const int MessageCode = 4;
        public const int UniqueIdentifier = 5;
        public const int MaxUniqueFieldLength = 40;
        public const int MaxAttributeFieldLength = 128;
        public const int MaxLongFieldLength = 2048;
        public const string DateTimeStampFormat1 = "HH:mm:ss dd/MM/yy";
        public const string DateTimeStampFormat2 = "yyyy:MM:dd:HH:mm:ss";
    }
}
