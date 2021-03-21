using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.CodeDom;
using Microsoft.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Build;
using Microsoft.Build.Evaluation;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Loader;
namespace Microsoft.BotBuilderSamples.Bots
{
    public class CSharpCompiler
    {
        static string path = @"D:\home\Test";
        static string pathToPowershell = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";

        public static string[] RunCommands(params string[] cmds)
        {

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathToPowershell,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    foreach (var item in cmds)
                    {
                        var cmd = item.Replace("\"", "\\\"");
                        sw.WriteLine(cmd);
                    }
                }
            }
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result.Split('\n');
        }
        public static string[] RunProject(out bool IsHasError, params string[] cmds)
        {

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathToPowershell,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    foreach (var item in cmds)
                    {
                        var cmd = item.Replace("\"", "\\\"");
                        sw.WriteLine(cmd);
                    }
                }
            }
            string error = process.StandardError.ReadToEnd();
            string result = process.StandardOutput.ReadToEnd();
            if (process.StandardError.ReadToEnd() != string.Empty)
            {
                result = error;
                IsHasError = true;
            }
            else
            {
                IsHasError = false;
            }
            process.WaitForExit();
            return result.Split('\n');
        }
        public static string RunAndCheckProject()
        {
            var result = RunProject(out bool error, path + @"\bin\Debug\net5.0\Test.exe");
            //var result = RunProject(out bool error, path + @"\bin\Debug\netcoreapp3.1\Test.exe");
            string endresult = "";
            if (error)
            {
                endresult = String.Join('\n', result);
            }
            else
            {
                endresult = ParseOutputText(result);
            }

            return ParseOutputText(result);
        }
        public static string ParseOutputText(string[] input)
        {
            var listresult = new List<string>(input);
            string endresult = "";
            bool startrecord = false;
            for (int i = 0; i < listresult.Count; i++)
            {
                if (i == listresult.Count - 1)
                {
                    startrecord = false;
                    break;
                }
                if (startrecord)
                {
                    endresult += listresult[i] + "\n";
                }
                if (listresult[i].Contains("Test.exe") && !startrecord)
                {
                    startrecord = true;
                }
            }
            return endresult;
        }
        public static string BuildProject(out bool IsHasError, string sourcecode)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                DirectoryInfo dir = new DirectoryInfo(path);
                List<string> Cmds = new List<string>();
                Cmds.Add(@"cd " + path);
                Cmds.Add("dotnet new -i Microsoft.DotNet.Common.ProjectTemplates.3.1");
                Cmds.Add("dotnet new console --force");
                RunCommands(Cmds.ToArray());
                File.WriteAllText(path + "\\Program.cs", sourcecode);
                Cmds.Clear();
                Cmds.Add(@"cd " + path);
                Cmds.Add("dotnet run");
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pathToPowershell,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                using (StreamWriter sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        foreach (var item in Cmds)
                        {
                            var cmd = item.Replace("\"", "\\\"");
                            sw.WriteLine(cmd);
                        }
                    }
                }
                string error = process.StandardError.ReadToEnd();
                string result = process.StandardOutput.ReadToEnd();
                if (error != string.Empty)
                {
                    result = error;
                    IsHasError = true;
                }
                else
                {
                    IsHasError = false;
                }
                process.WaitForExit();
                return String.Join('\n', result);
            }
            catch (Exception e)
            {
                IsHasError = true;
                return e.HelpLink + "\n" + e.Message + "\n" + e.Source + "\n" + e.StackTrace;
            }
        }
    }
}