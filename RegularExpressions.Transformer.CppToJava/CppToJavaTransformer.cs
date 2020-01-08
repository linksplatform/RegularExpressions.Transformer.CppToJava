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
            (new Regex(@"(?<begin>((#include[^\r\n]+|using[^\r\n]+|\s+)\r?\n)+)(?<body>(.|\n)+)$"), "${begin}class Program {" + Environment.NewLine + "${body}}", null, 0),
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
            // x.c_str()
            // x
            (new Regex(@"(?<variable>[_a-zA-Z0-9]+)\.c_str\(\)"), "${variable}", null, 0),
            // str.pop_back()
            // str = str.substring(0, str.length() - 1)
            (new Regex(@"(?<variable>[_a-zA-Z0-9]+)\.pop_back\(\)"), "${variable} = ${variable}.substring(0, ${variable}.length() - 1)", null, 0),
            // cout << "text";
            // System.out.print("text");
            (new Regex(@"cout << ""(?<text>[^""\r\n]+)"";"), "System.out.print(\"${text}\");", null, 0),
            // int main() { ... getline 
            // int main() { Scanner input = new Scanner(System.in); ... getline 
            (new Regex(@"(?<method>\r?\n[ \t]*[a-zA-Z ]+ [a-zA-Z]+[ \t]*\([^\)\r\n]*\)([^\{]|\n)+{[\r\n]*)(?<indent>[ \t]+)(?<end>((?!(Scanner input = new Scanner\(System\.in\);|\k<indent>[^\r\n]+\r?\n[ \t]+}))(.|\n))+?getline\(cin)"), "${method}${indent}Scanner input = new Scanner(System.in);" + Environment.NewLine + "${indent}${end}", null, 0),
            // ^ ... Scanner
            // ^ import java.util.Scanner; ... Scanner
            (new Regex(@"^(?<begin>((?!import java\.util\.Scanner;)(.|\n))+?)Scanner"), "import java.util.Scanner;" + Environment.NewLine + "${begin}Scanner", null, 0),
            // getline(cin, x);
            // x = input.nextLine();
            (new Regex(@"getline\(cin, (?<variable>[_a-zA-Z0-9]+)\);"), "${variable} = input.nextLine();", null, 0),

        }.Cast<ISubstitutionRule>().ToList();

        public static readonly IList<ISubstitutionRule> LastStage = new List<SubstitutionRule>
        {
            // // ...
            // 
            //(new Regex(@"(\r?\n)?[ \t]+//+.+"), "", null, 0),
        }.Cast<ISubstitutionRule>().ToList();

        public CppToJavaTransformer(IList<ISubstitutionRule> extraRules) : base(FirstStage.Concat(extraRules).Concat(LastStage).ToList()) { }

        public CppToJavaTransformer() : base(FirstStage.Concat(LastStage).ToList()) { }
    }
}
