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
            "cout << myVar;",
            "cout << \"multi word text\";",
            "cout << anotherVariable;"
        };
        
        // Original separate rules
        var rule1 = new Regex(@"cout << ""(?<text>[^""\r\n]+)"";");
        var rule2 = new Regex(@"cout << (?<variable>[_a-zA-Z0-9]+);");
        
        // Proposed merged rule - handles both quoted strings and variables
        var mergedRule = new Regex(@"cout << (?:""(?<text>[^""\r\n]+)""|(?<variable>[_a-zA-Z0-9]+));");
        
        Console.WriteLine("Testing original rules vs merged rule:");
        Console.WriteLine("=====================================");
        
        foreach (string test in testCases) 
        {
            Console.WriteLine($"Input: {test}");
            
            // Test original rules
            string result1 = rule1.Replace(test, "System.out.print(\"${text}\");");
            string result2 = rule2.Replace(result1, "System.out.print(${variable});");
            Console.WriteLine($"Original: {result2}");
            
            // Test merged rule
            string mergedResult = mergedRule.Replace(test, m => {
                if (m.Groups["text"].Success)
                    return $"System.out.print(\"{m.Groups["text"].Value}\");";
                else
                    return $"System.out.print({m.Groups["variable"].Value});";
            });
            Console.WriteLine($"Merged: {mergedResult}");
            
            Console.WriteLine($"Match: {result2 == mergedResult}");
            Console.WriteLine();
        }
    }
}
