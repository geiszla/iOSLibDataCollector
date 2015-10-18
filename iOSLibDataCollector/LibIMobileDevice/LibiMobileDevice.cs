using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace iOSLibDataCollector.LibIMobileDevice
{
    class LibiMobileDevice
    {
        public const string LibimobiledeviceDllPath = @"libimobiledevice.dll";
        const string LibplistDllPath = @"libplist.dll";
        public enum IDeviceError
        {
            IDEVICE_E_SUCCESS = 0,
            IDEVICE_E_INVALID_ARG = -1,
            IDEVICE_E_UNKNOWN_ERROR = -2,
            IDEVICE_E_NO_DEVICE = -3,
            IDEVICE_E_NOT_ENOUGH_DATA = -4,
            IDEVICE_E_BAD_HEADER = -5,
            IDEVICE_E_SSL_ERROR = -6
        }

        #region DllImport
        [DllImport(LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IDeviceError idevice_get_device_list(out IntPtr devicesPtr, out IntPtr countPtr);

        [DllImport(LibimobiledeviceDllPath, EntryPoint = "idevice_new", CallingConvention = CallingConvention.Cdecl)]
        public static extern IDeviceError NewDevice(out IntPtr iDevice, string udid);

        [DllImport(LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IDeviceError idevice_device_list_free(IntPtr devices);

        [DllImport(LibimobiledeviceDllPath, EntryPoint = "idevice_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern IDeviceError FreeDevice(IntPtr device);

        [DllImport(LibplistDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void plist_to_xml(IntPtr plist, out IntPtr xml, out int length);
        #endregion

        public static List<iDevice> GetDevices()
        {
            IntPtr devicesPtr;
            IntPtr countPtr;
            IDeviceError returnCode = idevice_get_device_list(out devicesPtr, out countPtr);

            List<iDevice> devices = new List<iDevice>();
            if (Marshal.ReadInt32(devicesPtr) != 0)
            {
                string currUdid;
                int i = 0;
                while ((currUdid = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(devicesPtr, i))) != null)
                {
                    devices.Add(new iDevice(currUdid));
                    i = i + 4;
                }

                idevice_device_list_free(devicesPtr);
            }

            return devices;
        }

        public static XDocument PlistToXml(IntPtr plistPtr)
        {
            IntPtr xmlPtr;
            int length;
            plist_to_xml(plistPtr, out xmlPtr, out length);

            byte[] resultBytes = new byte[length];
            Marshal.Copy(xmlPtr, resultBytes, 0, length);

            string resultString = Encoding.UTF8.GetString(resultBytes);
            return XDocument.Parse(resultString);
        }

        public static List<string> PtrToStringList(IntPtr listPtr, int skip)
        {
            List<string> stringList = new List<string>();
            if (Marshal.ReadInt32(listPtr) != 0)
            {
                string currString;
                int i = skip * 4;
                while ((currString = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(listPtr, i))) != null)
                {
                    stringList.Add(currString);
                    i = i + 4;
                }
            }

            return stringList;
        }
    }
}
