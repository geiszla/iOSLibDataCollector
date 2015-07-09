using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace iOSLibDataCollector.LibIMobileDevice
{
    class Lockdown
    {
        public enum LockdownError
        {
            LOCKDOWN_E_SUCCESS = 0,
            LOCKDOWN_E_INVALID_ARG = -1,
            LOCKDOWN_E_INVALID_CONF = -2,
            LOCKDOWN_E_PLIST_ERROR = -3,
            LOCKDOWN_E_PAIRING_FAILED = -4,
            LOCKDOWN_E_SSL_ERROR = -5,
            LOCKDOWN_E_DICT_ERROR = -6,
            LOCKDOWN_E_NOT_ENOUGH_DATA = -7,
            LOCKDOWN_E_MUX_ERROR = -8,
            LOCKDOWN_E_NO_RUNNING_SESSION = -9,
            LOCKDOWN_E_INVALID_RESPONSE = -10,
            LOCKDOWN_E_MISSING_KEY = -11,
            LOCKDOWN_E_MISSING_VALUE = -12,
            LOCKDOWN_E_GET_PROHIBITED = -13,
            LOCKDOWN_E_SET_PROHIBITED = -14,
            LOCKDOWN_E_REMOVE_PROHIBITED = -15,
            LOCKDOWN_E_IMMUTABLE_VALUE = -16,
            LOCKDOWN_E_PASSWORD_PROTECTED = -17,
            LOCKDOWN_E_USER_DENIED_PAIRING = -18,
            LOCKDOWN_E_PAIRING_DIALOG_RESPONSE_PENDING = -19,
            LOCKDOWN_E_MISSING_HOST_ID = -20,
            LOCKDOWN_E_INVALID_HOST_ID = -21,
            LOCKDOWN_E_SESSION_ACTIVE = -22,
            LOCKDOWN_E_SESSION_INACTIVE = -23,
            LOCKDOWN_E_MISSING_SESSION_ID = -24,
            LOCKDOWN_E_INVALID_SESSION_ID = -25,
            LOCKDOWN_E_MISSING_SERVICE = -26,
            LOCKDOWN_E_INVALID_SERVICE = -27,
            LOCKDOWN_E_SERVICE_LIMIT = -28,
            LOCKDOWN_E_MISSING_PAIR_RECORD = -29,
            LOCKDOWN_E_SAVE_PAIR_RECORD_FAILED = -30,
            LOCKDOWN_E_INVALID_PAIR_RECORD = -31,
            LOCKDOWN_E_MISSING_ACTIVATION_RECORD = -33,
            LOCKDOWN_E_SERVICE_PROHIBITED = -34,
            LOCKDOWN_E_ESCROW_LOCKED = -35,
            LOCKDOWN_E_UNKNOWN_ERROR = -256
        }

        #region DllImport
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern LockdownError lockdownd_client_new_with_handshake(IntPtr device, out IntPtr lockDownClient, string label);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern LockdownError lockdownd_get_value(IntPtr client, string domain, string key, out IntPtr result);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern LockdownError lockdownd_client_free(IntPtr client);
        #endregion

        public static LockdownError getDeviceProperties(iDevice device)
        {
            XDocument deviceProperties;
            LockdownError lockdownReturnCode = GetProperties(device, out deviceProperties);
            if (lockdownReturnCode != LockdownError.LOCKDOWN_E_SUCCESS)
            {
                LibiMobileDevice.FreeDevice(device.Handle);
                return lockdownReturnCode;
            }

            IEnumerable<XElement> keys = deviceProperties.Descendants("dict").Descendants("key");
            device.iOSVersion = keys.Where(x => x.Value == "ProductVersion").Select(x => (x.NextNode as XElement).Value).FirstOrDefault();
            device.Name = keys.Where(x => x.Value == "DeviceName").Select(x => (x.NextNode as XElement).Value).FirstOrDefault();

            return lockdownReturnCode;
        }

        static LockdownError GetProperties(iDevice device, out XDocument result)
        {
            result = new XDocument();

            CollectionForm.logWriter.WriteLine("[INFO] Connecting to lockdown client.");
            IntPtr lockdownClient;
            LockdownError returnCode = Lockdown.lockdownd_client_new_with_handshake(device.Handle, out lockdownClient, "CycriptGUI");
            if (returnCode != LockdownError.LOCKDOWN_E_SUCCESS || lockdownClient == IntPtr.Zero)
            {
                CollectionForm.logWriter.WriteLine("[ERROR] Couldn't connect to lockdown client. Lockdown error code " + (int)returnCode + ": " + returnCode + ".");
                return returnCode;
            }
            CollectionForm.logWriter.WriteLine("[INFO] Successfully connected to lockdown client.");

            CollectionForm.logWriter.WriteLine("[INFO] Getting data from lockdown client.");
            IntPtr resultPlist;
            if ((returnCode = lockdownd_get_value(lockdownClient, null, null, out resultPlist)) != LockdownError.LOCKDOWN_E_SUCCESS
                || resultPlist == IntPtr.Zero)
            {
                CollectionForm.logWriter.WriteLine("[ERROR] Couldn't get data from lockdown client. Lockdown error code " + (int)returnCode + ": " + returnCode + ".");
                lockdownd_client_free(lockdownClient);
                return returnCode;
            }
            CollectionForm.logWriter.WriteLine("[INFO] Data has been successfully got from lockdown client.");

            CollectionForm.logWriter.WriteLine("[INFO] Converting properties list to xml format.");
            try
            {
                result = LibiMobileDevice.PlistToXml(resultPlist);
            }

            catch
            {
                CollectionForm.logWriter.WriteLine("[ERROR] Couldn't convert returned data from plist to xml format.");
                lockdownd_client_free(lockdownClient);
                return LockdownError.LOCKDOWN_E_UNKNOWN_ERROR;
            }
            CollectionForm.logWriter.WriteLine("[INFO] Successfully converted plist to xml.");

            lockdownd_client_free(lockdownClient);
            return returnCode;
        }
    }
}
