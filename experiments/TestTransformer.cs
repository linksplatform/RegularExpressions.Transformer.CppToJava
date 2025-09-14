using System;
using System.IO;
using Platform.RegularExpressions.Transformer.CppToJava;

namespace TestTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            var transformer = new CppToJavaTransformer();
            string cppCode = File.ReadAllText("micro_test.cpp");
            
            Console.WriteLine("=== INPUT C++ CODE ===");
            Console.WriteLine(cppCode);
            Console.WriteLine();
            Console.WriteLine("=== TRANSFORMED JAVA CODE ===");
            
            string javaCode = transformer.Transform(cppCode, null);
            Console.WriteLine(javaCode);
            
            File.WriteAllText("actual_output.java", javaCode);
            Console.WriteLine();
            Console.WriteLine("Output written to actual_output.java");
        }
    }
}