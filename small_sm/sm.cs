using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.CSharp;


namespace Minimalism
{
    static class Program
    {
        [STAThread] // 1. its necessary? what it is this? 
        static void Main()
        {
            Application.Run(new Form1());
        }
    }

    class Form1 : Form
    {

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        
        // [DllImport("user32.dll")]
        // public static extern void keybd_event(Keys bVk, byte bScan, UInt32 dwFlags, IntPtr dwExtraInfo);

        public const int WM_HOTKEY = 0x312;

        public const byte ModAlt = 1, ModControl = 2, ModShift = 4, ModWin = 8;

        // private List<HotKey> list;
        private Dictionary<int, HotKey> hotkeys;


        public Form1()
        {
            hotkeys = new Dictionary<int, HotKey>();

            readHotkeys();
            registerHotKeys();
        }

        private void readHotkeys()
        {
            string line;
            StreamReader file = new StreamReader(@"hotkeys.txt");
            int itemNum = 0;
            while ((line = file.ReadLine()) != null)
            {
                itemNum++;
                var ar = line.Split(new string[] { "|" }, StringSplitOptions.None);
                hotkeys.Add(itemNum, new HotKey(itemNum, ar[0], ar[1], ar[2], ar[3]));
            }

            file.Close();

        }
        private void registerHotKeys()
        {
            foreach (KeyValuePair<int, HotKey> hkey in hotkeys)
            {
                var itm = hkey.Value;

                int curMod = 0;

                if (itm.isCtrl)
                {
                    curMod = curMod + ModControl;
                }

                if (itm.isAlt)
                {
                    curMod = curMod + ModAlt;
                }

                if (itm.isShift)
                {
                    curMod = curMod + ModShift;
                }

                Keys key = (Keys)Enum.Parse(typeof(Keys), itm.shortKey, true);
                Console.WriteLine("register hotkey:" + curMod.ToString() + "+" + ((int)key).ToString());

                bool success = RegisterHotKey(this.Handle, itm.num, curMod, (int)key);
            }
        }



        // private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        ~Form1()  // destructor
        {
            foreach (KeyValuePair<int, HotKey> hkey in hotkeys)
            {
                UnregisterHotKey(this.Handle, hkey.Key);
            }
        }

        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {
                case WM_HOTKEY:

                    keybd_event(0x10, 0, 0x2, 0);
                    keybd_event(0x11, 0, 0x2, 0);
                    keybd_event(0x12, 0, 0x2, 0);

                    int hotkeyIndex = m.WParam.ToInt32();

                    if (hotkeys.ContainsKey(hotkeyIndex))
                    {
                        HotKey hk = hotkeys[hotkeyIndex];

                        switch (hk.actionType) {
                            case "hotkey": 
                                SendKeys.SendWait(hk.action); 
                                break;
                            case "script": 
                                this.runScript(hk.action);
                                break;
                            case "external": 
                                this.runExternal(hk.engine, hk.action);
                                break;
                        }
                    }

                    break;
            }

            // Debug.WriteLine(m.Msg);
            base.WndProc(ref m);
        }

        private void runScript(string fileName) {
            
        }
        private void runExternal(string app, string appParams) {
System.Console.WriteLine("self:" + Application.ExecutablePath);
System.Console.WriteLine("ext:" + Path.GetDirectoryName(app));
ProcessStartInfo startInfo = new ProcessStartInfo(app);
startInfo.Arguments = appParams;
// startInfo.WorkingDirectory = Path.GetDirectoryName(app);
Process.Start(startInfo);

            // Process.Start(app, appParams);

// Process proc4 = new Process();
//                     proc4.StartInfo.FileName = app;
//                     proc4.StartInfo.Arguments = appParams;
//                     proc4.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
//                     proc4.Start();
//                     this.Opacity = 0;
//                     proc4.WaitForExit();
//                     this.Opacity = 100;

        }

        // public static object EvalJScript(string filePath)
        // {
		// 	JScript = File.ReadAllText(filePath);

        //     Microsoft.JScript.Vsa.VsaEngine Engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
        //     object Result = null;
        //     try
        //     {
        //         Result = Microsoft.JScript.Eval.JScriptEvaluate(JScript, Engine);
        //     }
        //     catch (Exception ex)
        //     {
        //         return ex.Message;
        //     }
        //     return Result;
        // }
    }

    class HotKey
    {
        public int num;
        public string actionType;
        public string hotkey;
        public string engine;
        public string action;

        public bool isCtrl = false;
        public bool isShift = false;
        public bool isAlt = false;
        public string shortKey;

        public HotKey(int num, string actionType, string hotkey, string engine, string action)
        {
            this.num = num;
            this.actionType = actionType;
            this.hotkey = hotkey;
            this.engine = engine;
            this.action = action;

            this.parse();
        }

        private void parse()
        {
            var ar = hotkey.Split(new string[] { " " }, StringSplitOptions.None);

            foreach (string sub in ar)
            {
                if (sub == "Ctrl")
                {
                    isCtrl = true;
                }
                else if (sub == "Alt")
                {
                    isAlt = true;
                }
                else if (sub == "Shift")
                {
                    isShift = true;
                }
                else
                {
                    shortKey = sub;
                }
            }

        }
    }
}