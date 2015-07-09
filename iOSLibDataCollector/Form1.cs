using iOSLibDataCollector.LibIMobileDevice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iOSLibDataCollector
{
    public partial class CollectionForm : Form
    {
        List<iDevice> deviceList;
        string[] readDevices;
        string savePath;

        public static StreamWriter logWriter;

        public CollectionForm()
        {
            InitializeComponent();
        }

        #region Events
        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Text = "Collecting...";
            startButton.Enabled = false;
            browseButton.Enabled = false;
            savePathBox.Enabled = false;

            Task getApplicationsTask = Task.Factory.StartNew(() =>
            {
                if (CollectData() == 0)
                {
                    BeginInvoke(new MethodInvoker(() => { progressTextBox.Text = "Data collected successfully."; }));
                }

                else
                {
                    BeginInvoke(new MethodInvoker(() => { progressTextBox.Text = "Data collection failed. Check log for details."; }));
                }
                logWriter.Close();

                BeginInvoke(new MethodInvoker(() =>
                {
                    progressBar.Value = 0;
                    startButton.Text = "Collect Data!";
                    startButton.Enabled = true;
                    browseButton.Enabled = true;
                    savePathBox.Enabled = true;
                }));
            });
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selectPathDialog = new FolderBrowserDialog();
            selectPathDialog.SelectedPath = Directory.GetCurrentDirectory();
            if (selectPathDialog.ShowDialog() == DialogResult.OK)
            {
                savePathBox.Text = selectPathDialog.SelectedPath;
            }
        }

        private void CollectionForm_Shown(object sender, EventArgs e)
        {
            savePathBox.Text = Directory.GetCurrentDirectory();
        }
        #endregion

        #region Functions
        int CollectData()
        {
            if (!Directory.Exists(savePathBox.Text))
            {
                MessageBox.Show("The above path is not valid, please select another!", "Invalid Path",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            if (!Directory.Exists(savePath = savePathBox.Text + @"\iOSData"))
            {
                Directory.CreateDirectory(savePath);
            }

            deviceList = new List<iDevice>();
            readDevices = getReadDevices();

            logWriter = new StreamWriter(savePath + @"\log.txt");

            logWriter.WriteLine("[INFO] Logging started.");
            logWriter.WriteLine("[INFO] Searching for devices.");
            BeginInvoke(new MethodInvoker(() => { progressTextBox.Text = "Searching for iDevices..."; }));

            List<iDevice> foundDevices = LibiMobileDevice.GetDevices();
            deviceList = readDevices != null ?
                foundDevices.Where(x => !readDevices.Any(y => x.Udid == y)).ToList() : foundDevices;

            if (deviceList.Count == 0)
            {
                MessageBox.Show("No new device found. Please check the connection and if the proper driver is installed! If you want to get data from an already read device, please delete it from 'done.txt'",
                    "No New Device Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                logWriter.WriteLine("[INFO] No new device found. Exiting.");
                return 0;
            }
            logWriter.WriteLine("[INFO] Search completed successfully, " + deviceList.Count + " not read device found.");

            BeginInvoke(new MethodInvoker(() => { progressBar.Maximum = deviceList.Count * 3; }));
            foreach (iDevice currDevice in deviceList)
            {
                logWriter.WriteLine("[INFO] Connecting to " + currDevice.Udid + ".");
                BeginInvoke(new MethodInvoker(() => { progressTextBox.Text = "Connecting to " + currDevice.Udid + "..."; progressBar.PerformStep(); }));

                LibiMobileDevice.IDeviceError libReturnCode = LibiMobileDevice.NewDevice(out currDevice.Handle, currDevice.Udid);

                if (libReturnCode != LibiMobileDevice.IDeviceError.IDEVICE_E_SUCCESS || currDevice.Handle == IntPtr.Zero)
                {
                    LibiMobileDevice.FreeDevice(currDevice.Handle);

                    logWriter.Write("[ERROR] Couldn't connect to " + currDevice.Udid + ". iDevice error code " + (int)libReturnCode + ": " + libReturnCode);
                    if (MessageBox.Show("Couldn't connect to " + currDevice.Udid + ". Do you want to continue data collection from other devices (if there's any)?",
                        "Connection Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                    {
                        logWriter.WriteLine(" Exiting.");
                        return 1;
                    }

                    else
                    {
                        logWriter.WriteLine(" Continue collecting from other devices (if there's any).");
                        continue;
                    }
                }
                logWriter.WriteLine("[INFO] Successfully connected to " + currDevice.Udid + ".");

                logWriter.WriteLine("[INFO] Getting properties of " + currDevice.Udid + ".");
                if (Lockdown.getDeviceProperties(currDevice) != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
                {
                    LibiMobileDevice.FreeDevice(currDevice.Handle);
                    continue;
                }
                logWriter.WriteLine("[INFO] Successfully got properties of " + currDevice.Udid + " (" + currDevice.Name + ").");

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                logWriter.WriteLine("[INFO] Collecting data from " + currDevice.Udid + " (" + currDevice.Name + ").");
                BeginInvoke(new MethodInvoker(() => { progressTextBox.Text = currDevice.Name + " - Collecting data..."; progressBar.PerformStep(); }));

                AFC.AFCError afcReturnCode = AFC.CollectData(currDevice, savePath);
                if (afcReturnCode != AFC.AFCError.AFC_E_SUCCESS)
                {
                    LibiMobileDevice.FreeDevice(currDevice.Handle);

                    if (MessageBox.Show("Couldn't save data from \"" + currDevice.Name + "\" (" + currDevice.Udid +
                        "). Please search for Photos.sqlite and copy manually (somewhere in 'var/mobile/Media/' folder). Do you want to continue data collection from other devices (if there's any)?",
                        "Couldn't Find Database", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                    {
                        return 1;
                    }

                    else
                    {
                        continue;
                    }
                }
                logWriter.WriteLine("[INFO] Data has been collected successfully from " + currDevice.Udid + " (" + currDevice.Name + "). Device has been added to done.txt.");

                BeginInvoke(new MethodInvoker(() => { progressBar.PerformStep(); }));

                readDevices = readDevices != null ? readDevices.Concat(new string[] { currDevice.Udid }).ToArray()
                    : new string[] { currDevice.Udid };
                LibiMobileDevice.FreeDevice(currDevice.Handle);
            }

            if (readDevices != null)
            {
                StreamWriter doneDeviceWriter = new StreamWriter(Directory.GetCurrentDirectory() + @"\done.txt");
                foreach (string currDoneDevice in readDevices)
                {
                    doneDeviceWriter.WriteLine(currDoneDevice);
                }

                doneDeviceWriter.Close();
            }

            MessageBox.Show("All data has been collected successfully to " + savePath + ".", "Collection Successful",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return 0;
        }

        string[] getReadDevices()
        {
            string readDeviceListPath = Directory.GetCurrentDirectory() + @"\done.txt";
            if (!File.Exists(readDeviceListPath))
            {
                return default(string[]);
            }

            return File.ReadAllLines(readDeviceListPath);
        }
        #endregion
    }
}
