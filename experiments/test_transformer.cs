using System;
using System.IO;
using System.Linq;
using System.Reflection;

class TestTransformer 
{
    static void Main() 
    {
        // Load the transformer assembly 
        var transformerPath = Path.Combine("..", "..", "RegularExpressions.Transformer.CppToJava", "bin", "Debug", "net8.0", "RegularExpressions.Transformer.CppToJava.dll");
        
        if (!File.Exists(transformerPath))
        {
            Console.WriteLine("Building transformer first...");
            System.Diagnostics.Process.Start("dotnet", "build ../RegularExpressions.Transformer.CppToJava").WaitForExit();
        }
        
        try 
        {
            var assembly = Assembly.LoadFrom(transformerPath);
            var transformerType = assembly.GetType("Platform.RegularExpressions.Transformer.CppToJava.CppToJavaTransformer");
            var transformer = Activator.CreateInstance(transformerType);
            var transformMethod = transformerType.GetMethod("Transform");
            
            // Test cases for cout rules
            string[] testCases = {
                "cout << \"Hello World\";",
                "cout << variable;",
                "cout << \"multi word text\";",
                "cout << myVar;"
            };
            
            Console.WriteLine("Testing merged cout rule with actual transformer:");
            Console.WriteLine("===============================================");
            
            foreach (string test in testCases) 
            {
                string result = (string)transformMethod.Invoke(transformer, new object[] { test });
                Console.WriteLine($"Input:  {test}");
                Console.WriteLine($"Output: {result}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Building and trying again...");
            
            // Build the project
            var buildProcess = System.Diagnostics.Process.Start("dotnet", "build ../RegularExpressions.Transformer.CppToJava");
            buildProcess.WaitForExit();
            
            Console.WriteLine("Build completed. Please run the test again.");
        }
    }
}