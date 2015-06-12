using System;

namespace iOSLibDataCollector
{
    class iDevice
    {
        public IntPtr Handle;
        public string Udid;
        public string Name;
        public string iOSVersion;

        public iDevice(string udid)
        {
            this.Udid = udid;
        }
    }
}
