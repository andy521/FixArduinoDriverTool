﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceProcess;
using System.Diagnostics;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;

namespace FixDriver
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        String folder1 = "amd64";
        String folder2 = "win7";
        //ProgressDialogController controller;
        public MainWindow()
        {
            InitializeComponent();
            if (!Environment.Is64BitOperatingSystem)
            {
                folder1 = "x86";
            }
            if (Environment.OSVersion.ToString().Contains("6.2"))
            {
                folder2 = "win8";
            }
            //webad.Navigate("http://clz.me/ad/ad1.html");
        }

        //string sourcePath;
        //string targetPath = System.Environment.SystemDirectory + "\\drivers\\usbser.sys";
        private void CopyFile()
        {
            string sourcePath, sourcePath2;
            string targetPath = System.Environment.SystemDirectory + "\\drivers\\usbser.sys";
            string targetPath2 = System.Environment.SystemDirectory + "\\DriverStore\\FileRepository\\" + FindFolder(@"C:\windows\inf\setupapi.dev.log");
            if (!File.Exists(targetPath))
            {
                sourcePath = ".\\drivers\\"+ folder2+"\\"+ folder1 +"\\usbser.sys";
                //if (Is64)
                //{
                //    sourcePath = ".\\drivers\\amd64\\usbser.sys";
                //}
                //else
                //{
                //    sourcePath = ".\\drivers\\x86\\usbser.sys";
                //}
                System.IO.File.Copy(sourcePath, targetPath, true);
            }

            if (!Directory.Exists(targetPath2))
            {
                AddDirectorySecurity(targetPath2, Environment.UserName, FileSystemRights.FullControl, AccessControlType.Allow);
                Directory.CreateDirectory(targetPath2);
            }
            sourcePath2= ".\\drivers\\" + folder2 + "\\" + folder1;
            //if (Is64)
            //{
            //    sourcePath2 = ".\\drivers\\amd64";
            //}
            //else
            //{
            //    sourcePath2 = ".\\drivers\\x86";
            //}
            foreach (string file in Directory.GetFiles(sourcePath2))
            {
                File.Copy(file, targetPath2 +"\\"+ System.IO.Path.GetFileName(file), true);
            }
        }

        private void StartService()
        {
            ServiceController control = new ServiceController("Device Install Service");
            if (control.Status == ServiceControllerStatus.Stopped)
            {
                control.Start();
            }
        }

        private string FindFolder(string path)
        {
            string[] text = System.IO.File.ReadAllLines(path);
            int i = text.Length, pos = 0;
            string FolderName;
            string compare = "mdmcpq.inf_";
            for (int j = 0; j < i; j++)
            {
                if (text[j].Contains(compare))
                {
                    pos = j;
                }
            }
            int fore = text[pos].IndexOf(compare);
            if (Environment.Is64BitOperatingSystem)
                FolderName = text[pos].Substring(fore, 33);
            else
                FolderName = text[pos].Substring(fore, 39);
            return FolderName;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CopyFile();
                StartService();
                this.ShowMessageAsync("Done", "Try to install arduino driver again.");
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Error", ex.Message);
            }
            
        }



        // Adds an ACL entry on the specified file for the specified account.
        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            DirectoryInfo dInfo = new DirectoryInfo(FileName);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                            Rights,
                                                            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);

        }

        //private void SetupDriver()
        //{
        //    string command = "rundll32.exe setupapi.dll,InstallHinfSection DefaultInstall 128 " + ".\\drivers\\arduino.inf";
        //    Process p = new Process();
        //    p.StartInfo.FileName = "CMD.EXE"; //创建CMD.EXE 进程
        //    p.StartInfo.RedirectStandardInput = true; //重定向输入
        //    p.StartInfo.RedirectStandardOutput = true;//重定向输出
        //    p.StartInfo.UseShellExecute = false; // 不调用系统的Shell
        //    p.StartInfo.RedirectStandardError = true; // 重定向Error
        //    p.StartInfo.CreateNoWindow = true; //不创建窗口
        //    p.Start(); // 启动进程
        //    string s = p.StandardOutput.ReadToEnd(); //将输出赋值给 S
        //    p.WaitForExit();  // 等待退出
        //}

    }

}
