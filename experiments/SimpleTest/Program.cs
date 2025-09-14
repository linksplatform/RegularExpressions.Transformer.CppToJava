using System;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main() 
    {
        // Test the exact regex pattern from the updated code
        var mergedRule = new Regex(@"cout << (?<content>(?:""[^""\r\n]+"")|(?:[_a-zA-Z0-9]+));");
        
        string[] testCases = {
            "cout << \"Hello World\";",
            "cout << variable;",
            "cout << \"text with spaces\";", 
            "cout << myVar;",
            "int main() { cout << \"Hello\"; cout << x; }"
        };
        
        Console.WriteLine("Testing merged cout rule:");
        Console.WriteLine("========================");
        
        foreach (string test in testCases) 
        {
            string result = mergedRule.Replace(test, "System.out.print(${content});");
            Console.WriteLine($"Input:  {test}");
            Console.WriteLine($"Output: {result}");
            Console.WriteLine();
        }
    }
}
