namespace HikVisionConverter.Communication
{
    public static class ResponseMapper
    {
        public static doDeviceConfigurationRequest Map(DeviceConfiguration configuration)
            => new(configuration);

        public static doDeviceStatusReportRequest Map(DeviceStatusReport configuration)
            => new(configuration);
    }
}
