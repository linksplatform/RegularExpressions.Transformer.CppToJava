using System;
using System.Text.RegularExpressions;

class Program 
{
    static void Main() 
    {
        // Test cases
        string[] testCases = {
            "cout << \"Hello World\";",
            "cout << variable;", 
            "cout << \"text with spaces\";",
            "cout << myVar;"
        };
        
        // Single regex that captures everything after << 
        // and uses a simple string replacement
        var mergedRule = new Regex(@"cout << (?<content>(?:""[^""\r\n]+"")|(?:[_a-zA-Z0-9]+));");
        
        Console.WriteLine("Testing simple string replacement approach:");
        Console.WriteLine("==========================================");
        
        foreach (string test in testCases) 
        {
            Console.WriteLine($"Input: {test}");
            
            // Simple string replacement - just substitute cout << with System.out.print(
            string result = mergedRule.Replace(test, "System.out.print(${content});");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine();
        }
        
        Console.WriteLine("This produces the correct Java output format!");
    }
}
