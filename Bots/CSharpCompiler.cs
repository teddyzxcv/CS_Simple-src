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
        public class CollectibleAssemblyLoadContext : AssemblyLoadContext
        {
            public CollectibleAssemblyLoadContext() : base(isCollectible: true)
            { }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                return null;
            }
        }

        static CSharpCompilation Compilation;
        public static bool BuildTheCode(string input, out string[] error)
        {
            var dotnetCoreDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            error = new string[0];
            var compilation = CSharpCompilation.Create("LibraryName")
               .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
               .AddReferences(
                   MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                   MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                   MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "mscorlib.dll")),
                   MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "netstandard.dll")),
                   MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Runtime.dll")))
               .AddSyntaxTrees(ParseThecode(input));
            Compilation = compilation;
            if (!compilation.GetDiagnostics().IsEmpty)
            {
                error = new string[compilation.GetDiagnostics().Length];
                foreach (var compilerMessage in compilation.GetDiagnostics())
                    Console.WriteLine(compilerMessage);
                for (int i = 0; i < error.Length; i++)
                {
                    error[i] = compilation.GetDiagnostics().ToList()[i].ToString();
                }
                return true;
            }
            else
                return false;
            // Debug output. In case your environment is different it may show some messages.
        }

        public static SyntaxTree ParseThecode(string sourcecode)
        {
            SyntaxTree snt = CSharpSyntaxTree.ParseText(sourcecode);
            SyntaxNode snn = snt.GetRoot();
            return snt;
        }
        public static void Run()
        {
            using (var memoryStream = new MemoryStream())
            {
                var emitResult = Compilation.Emit(memoryStream);
                if (emitResult.Success)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var context = new CollectibleAssemblyLoadContext();
                    var assembly = context.LoadFromStream(memoryStream);
                    assembly.GetTypes().ToList().ForEach(Console.WriteLine);
                    assembly.GetTypes().FirstOrDefault().GetMethods().ToList().ForEach(Console.WriteLine);
                    MethodInfo entryPoint = assembly.GetType("Test.Program").GetMethod("Main");
                    entryPoint.Invoke(null, new object[] { new string[] { "arg1", "arg2", "etc" } });
                    context.Unload();
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
            }

        }

    }
}
