using System;
using System.Text.RegularExpressions;

class TestSubstitutionRule 
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
        
        // Current separate rules approach
        var rule1 = new Regex(@"cout << ""(?<text>[^""\r\n]+)"";");
        var rule2 = new Regex(@"cout << (?<variable>[_a-zA-Z0-9]+);");
        
        // Option 1: Single regex with conditional groups
        var mergedRule1 = new Regex(@"cout << (?:""(?<text>[^""\r\n]+)""|(?<variable>[_a-zA-Z0-9]+));");
        
        // Option 2: Single regex that captures everything after <<
        var mergedRule2 = new Regex(@"cout << (?<content>(?:""[^""\r\n]+"")|(?:[_a-zA-Z0-9]+));");
        
        Console.WriteLine("Testing different merged rule approaches:");
        Console.WriteLine("=======================================");
        
        foreach (string test in testCases) 
        {
            Console.WriteLine($"Input: {test}");
            
            // Original approach
            string orig1 = rule1.Replace(test, "System.out.print(\"${text}\");");
            string orig2 = rule2.Replace(orig1, "System.out.print(${variable});");
            Console.WriteLine($"Original: {orig2}");
            
            // Option 2 - capture content and transform
            string merged2 = mergedRule2.Replace(test, match => {
                string content = match.Groups["content"].Value;
                if (content.StartsWith("\"") && content.EndsWith("\""))
                {
                    // It's a quoted string, keep the quotes
                    return $"System.out.print({content});";
                }
                else 
                {
                    // It's a variable, no quotes needed
                    return $"System.out.print({content});";
                }
            });
            Console.WriteLine($"Merged2: {merged2}");
            
            Console.WriteLine($"Match: {orig2 == merged2}");
            Console.WriteLine();
        }
    }
}