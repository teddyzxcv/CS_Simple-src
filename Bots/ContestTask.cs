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
using System.IO.Packaging;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class ContestTask
    {
        public string Condition = "";
        public List<string> Input = new List<string>();
        public List<string> Output = new List<string>();
        int Id;

        public ContestTask()
        {
            string[] Alltask = Directory.GetDirectories(@"D:\home\Task");
            Random rd = new Random();
            int r = rd.Next(0, Alltask.Length);
            Condition = File.ReadAllText(Alltask[r] + @"\c.txt");
            string[] Alltest = Directory.GetFiles(Alltask[r]);
            for (int i = 1; i <= Alltest.Length / 2; i++)
            {
                Input.Add(File.ReadAllText(Alltask[r] + @"\" + i));
                Output.Add(File.ReadAllText(Alltask[r] + @"\" + i + ".a"));
            }
        }
    }
}