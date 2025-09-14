using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.RegularExpressions.Transformer.CppToJava
{
    public class CppToJavaTransformer : Transformer
    {
        public static readonly IList<ISubstitutionRule> FirstStage = new List<SubstitutionRule>
        {
            // #include ... using ... 
            // #include ... using ... class Program { ... }
            (new Regex(@"\A(?<begin>((#include[^\r\n]+|using[^\r\n]+|\s+)\r?\n)+)(?<body>(.|\n)+)$"), "${begin}class Program {" + Environment.NewLine + "${body}}", null, 0),
            // Set cursor
            // class Program {\r\n
            // class Program {\r\n\t/*cursor*/
            (new Regex(@"(?<begin>class Program {[^\r\n]*)\r?\n"), "${begin}" + Environment.NewLine + "\t/*cursor*/", null, 0),
            // Move cursor
            // /*cursor*/...\r\n
            // ...\r\n\t/*cursor*/
            (new Regex(@"/\*cursor\*/(?<begin>[^\r\n]+)\r?\n(?!}$)"), "${begin}" + Environment.NewLine + "\t/*cursor*/", null, int.MaxValue),
            // Remove cursor
            // /*cursor*/
            //
            (new Regex(@"/\*cursor\*/"), "", null, 0),
            // int main()
            // public static void main(String[] args)
            (new Regex(@"int main\([^\)\r\n]*\)"), "public static void main(String[] args)", null, 0),
            // std::string
            // String
            (new Regex(@"(std::)?string"), "String", null, 0),
            // word.clear();
            // word = "";
            (new Regex(@"(?<variable>[_a-zA-Z0-9]+)\.clear\(\)"), "${variable} = \"\"", null, 0),
            // x.c_str()
            // x
            (new Regex(@"(?<variable>[_a-zA-Z0-9]+)\.c_str\(\)"), "${variable}", null, 0),
            // str.pop_back()
            // str = str.substring(0, str.length() - 1)
            (new Regex(@"(?<variable>[_a-zA-Z0-9]+)\.pop_back\(\)"), "${variable} = ${variable}.substring(0, ${variable}.length() - 1)", null, 0),
            // cout << "text";
            // System.out.print("text");
            (new Regex(@"cout << ""(?<text>[^""\r\n]+)"";"), "System.out.print(\"${text}\");", null, 0),
            // cout << text;
            // System.out.print(text);
            (new Regex(@"cout << (?<variable>[_a-zA-Z0-9]+);"), "System.out.print(${variable});", null, 0),
            // int main() { ... getline 
            // int main() { Scanner input = new Scanner(System.in); ... getline 
            (new Regex(@"(?<method>\r?\n[ \t]*[a-zA-Z ]+ [a-zA-Z]+[ \t]*\([^\)\r\n]*\)([^\{]|\n)+{[\r\n]*)(?<indent>[ \t]+)(?<end>((?!(Scanner input = new Scanner\(System\.in\);|\k<indent>[^\r\n]+\r?\n[ \t]+}))(.|\n))+?getline\(cin)"), "${method}${indent}Scanner input = new Scanner(System.in);" + Environment.NewLine + "${indent}${end}", null, 0),
            // ^ ... Scanner
            // ^ import java.util.Scanner; ... Scanner
            (new Regex(@"\A(?<begin>((?!import java\.util\.Scanner;)(.|\n))+?)Scanner"), "import java.util.Scanner;" + Environment.NewLine + "${begin}Scanner", null, 0),
            // getline(cin, x);
            // x = input.nextLine();
            (new Regex(@"getline\(cin, (?<variable>[_a-zA-Z0-9]+)\);"), "${variable} = input.nextLine();", null, 0),
            // fstream file(path, std::ios::in);
            // FileInputStream file = new FileInputStream(path);
            (new Regex(@"fstream (?<stream>[_a-zA-Z0-9]+)\((?<path>[_a-zA-Z0-9]+), std::ios::in\);"), "FileInputStream ${stream} = new FileInputStream(${path});", null, 0),
            // ^ ... FileInputStream
            // ^ import java.io.FileInputStream; ... FileInputStream
            (new Regex(@"\A(?<begin>((?!import java\.io\.FileInputStream;)(.|\n))+?)FileInputStream"), "import java.io.FileInputStream;" + Environment.NewLine + "${begin}FileInputStream", null, 0),
            // fstream file(path, std::ios::out);
            // FileOutputStream file = new FileOutputStream(path);
            (new Regex(@"fstream (?<stream>[_a-zA-Z0-9]+)\((?<path>[_a-zA-Z0-9]+), std::ios::out\);"), "FileOutputStream ${stream} = new FileOutputStream(${path});", null, 0),
            // ^ ... FileOutputStream
            // ^ import java.io.FileOutputStream; ... FileOutputStream
            (new Regex(@"\A(?<begin>((?!import java\.io\.FileOutputStream;)(.|\n))+?)FileOutputStream"), "import java.io.FileOutputStream;" + Environment.NewLine + "${begin}FileOutputStream", null, 0),
            // &x
            // x
            (new Regex(@"&(?<variable>[_a-zA-Z0-9]+)"), "${variable}", null, 0),
            // catch (char* msg)
            // catch (Exception e/*replacement*msg*e.getMessage()*/)
            (new Regex(@"catch \(char\* (?<message>[_a-zA-Z0-9]+)\)"), "catch (Exception e/*replacement*${message}*e.getMessage()*/)", null, 0),
            // map<x, x>
            // HashMap<x, x>
            (new Regex(@"map<(?<key>[_a-zA-Z0-9]+), (?<value>[_a-zA-Z0-9]+)>"), "HashMap<${key}, ${value}>", null, 0),
            // typedef HashMap<String, String> DictArr;
            // /*replacement*DictArr*HashMap<String, String>*/
            (new Regex(@"typedef (?<type>[^;\r\n]+) (?<alias>[_a-zA-Z0-9]+);"), "/*replacement*${alias}*${type}*/", null, 0),
            // /*replacement*msg*e.getMessage()*/ ... msg
            // /*replacement*msg*e.getMessage()*/ ... e.getMessage()
            (new Regex(@"(?<begin>/\*replacement\*(?<pattern>[^\*\r\n]+)\*(?<substitution>[^\*\r\n]+)\*/(.|\n)+)\k<pattern>"), "${begin}${substitution}", null, int.MaxValue),
            // HashMap<String, String>::iterator iter = dictionary.find(word);
            // /*replacement*iter*dictionary.containsKey(word)*/
            (new Regex(@"HashMap<[_a-zA-Z0-9]+, [_a-zA-Z0-9]+>::iterator (?<iterator>[_a-zA-Z0-9]+) = (?<map>[_a-zA-Z0-9]+)\.find\((?<key>[_a-zA-Z0-9]+)\);"), "/*replacement*${iterator}*${map}.containsKey(${key})*/", null, 0),
            // /*replacement*msg*e.getMessage()*/ ... msg
            // /*replacement*msg*e.getMessage()*/ ... e.getMessage()
            (new Regex(@"(?<begin>/\*replacement\*(?<pattern>[^\*\r\n]+)\*(?<substitution>[^\*\r\n]+)\*/(.|\n)+)\k<pattern>"), "${begin}${substitution}", null, int.MaxValue),
            // /*replacement*msg*e.getMessage()*/
            // 
            (new Regex(@"/\*replacement\*[^\*\r\n]+\*[^\*\r\n]+\*/"), "", null, 0),
            // _getch();
            // System.in.read()
            (new Regex(@"_getch\(\);"), "System.in.read();", null, 0),
            // HashMap<String, String> dictionary;
            // HashMap<String, String> dictionary = HashMap<String, String>();
            (new Regex(@"(?<type>HashMap<[_a-zA-Z0-9]+, [_a-zA-Z0-9]+>) (?<variable>[_a-zA-Z0-9]+);"), "${type} ${variable} = new ${type}();", null, 0),
            // dictionary.containsKey(word) == dictionary.end()
            // dictionary.containsKey(word)
            (new Regex(@"(?<baseExpression>(?<variable>[_a-zA-Z0-9]+)\.containsKey\([_a-zA-Z0-9]+\)) == \k<variable>\.end\(\)"), "!${baseExpression}", null, 0),
            // ^ ... HashMap
            // ^ import java.util.HashMap; ... HashMap
            (new Regex(@"\A(?<begin>((?!import java\.util\.HashMap;)(.|\n))+?)HashMap"), "import java.util.HashMap;" + Environment.NewLine + "${begin}HashMap", null, 0),
            
            // Additional transformation rules for more complete C++ to Java translation
            
            // throw "string literal";
            // throw new Exception("string literal");
            (new Regex(@"throw ""(?<message>[^""\r\n]+)"";"), "throw new Exception(\"${message}\");", null, 0),
            
            // string[index]
            // string.charAt(index)
            (new Regex(@"(?<string>[_a-zA-Z0-9]+)\[(?<index>[_a-zA-Z0-9]+)\]"), "${string}.charAt(${index})", null, 0),
            
            // variable.empty()
            // variable.isEmpty()
            (new Regex(@"(?<variable>[_a-zA-Z0-9]+)\.empty\(\)"), "${variable}.isEmpty()", null, 0),
            
            // dictionary[key] = value;
            // dictionary.put(key, value);
            (new Regex(@"(?<dict>[_a-zA-Z0-9]+)\[(?<key>[_a-zA-Z0-9]+)\] = (?<value>[_a-zA-Z0-9]+);"), "${dict}.put(${key}, ${value});", null, 0),
            
            // iter->second
            // dictionary.get(key)
            (new Regex(@"(?<iter>[_a-zA-Z0-9]+)->second"), "${iter}", null, 0),
            
            // unsigned int variable
            // int variable  
            (new Regex(@"unsigned (?<type>int|long|short) (?<variable>[_a-zA-Z0-9]+)"), "${type} ${variable}", null, 0),
            
            // unsigned variable
            // int variable
            (new Regex(@"unsigned (?<variable>[_a-zA-Z0-9]+)"), "int ${variable}", null, 0),
            
            // Complex while loop and Scanner creation rules commented out temporarily
            // (new Regex(@"while \(getline\((?<file>[_a-zA-Z0-9]+), (?<line>[_a-zA-Z0-9]+)\)\)"), "while (${file}Scanner.hasNextLine()) { ${line} = ${file}Scanner.nextLine();", null, 0),
            // (new Regex(@"(?<declaration>FileInputStream (?<file>[_a-zA-Z0-9]+) = new FileInputStream\([^)]+\);)(?!\s*Scanner)"), "${declaration}" + Environment.NewLine + "\t\t\tScanner ${file}Scanner = new Scanner(${file});", null, 0),
            
            // file.close(); (simple case for now)
            // fileScanner.close(); file.close(); - commented out to avoid conflicts
            // (new Regex(@"(?<file>[_a-zA-Z0-9]+)\.close\(\);"), "${file}Scanner.close();" + Environment.NewLine + "\t\t\t${file}.close();", null, 0),
            
            // SetConsoleCP(1251);
            // // SetConsoleCP(1251); (comment out Windows-specific)
            (new Regex(@"SetConsoleCP\([^)]+\);"), "// SetConsoleCP - Windows specific function removed", null, 0),
            
            // outputFile << finalText;
            // PrintWriter writer = new PrintWriter(outputFile); writer.print(finalText);
            (new Regex(@"(?<file>[_a-zA-Z0-9]+) << (?<text>[_a-zA-Z0-9]+);"), "java.io.PrintWriter writer = new java.io.PrintWriter(${file});" + Environment.NewLine + "\t\t\twriter.print(${text});", null, 0),
            
            // Simplified exception handling - just fix the throw statement
            // Complex try-catch structure transformation commented out for now
            // (new Regex(@"if \(!(?<file>[_a-zA-Z0-9]+)\)"), "try {", null, 0),
            // (new Regex(@"throw new Exception\(""Файл не найден.""\);"), "} catch (Exception e) {" + Environment.NewLine + "\t\t\tthrow new Exception(\"Файл не найден.\");" + Environment.NewLine + "\t\t}", null, 0),
            
            // Remove problematic complex rule for now
            // (new Regex(@"java\.io\.PrintWriter writer = new java\.io\.PrintWriter\((?<file>[_a-zA-Z0-9]+)\);[\s\S]*?writer\.print\([^)]+\);"), "${0}" + Environment.NewLine + "\t\t\twriter.close();" + Environment.NewLine + "\t\t\t${file}.close();", null, 0),

        }.Cast<ISubstitutionRule>().ToList();

        public static readonly IList<ISubstitutionRule> LastStage = new List<SubstitutionRule>
        {
            // #include <map>
            // 
            (new Regex(@"#include <[a-zA-Z\.]+>\r?\n"), "", null, 0),
            // using namespace std;
            // 
            (new Regex(@"using [^;\r\n]+;\r?\n"), "", null, 0),
        }.Cast<ISubstitutionRule>().ToList();

        public CppToJavaTransformer(IList<ISubstitutionRule> extraRules) : base(FirstStage.Concat(extraRules).Concat(LastStage).ToList()) { }

        public CppToJavaTransformer() : base(FirstStage.Concat(LastStage).ToList()) { }
    }
}
