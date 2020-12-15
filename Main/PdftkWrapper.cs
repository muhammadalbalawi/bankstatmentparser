using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp1
{
    class PdftkWrapper :IDisposable
    {
        Process process;
        public PdftkWrapper(string filepath, string password)
        {
            process = new Process();
            process.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),  @"pdftotext.exe");
            process.StartInfo.Arguments = $"-opw {password} -table -marginl 50 -margint 260 -marginb 80 -enc UTF-8 {filepath} -";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
        }
        public StreamReader Start()
        {         
            process.Start();
            return  process.StandardOutput;
        }
        public void Dispose() {
            process.WaitForExit();        
        }
    }
}