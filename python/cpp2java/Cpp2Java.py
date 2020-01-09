# -*- coding: utf-8 -*-
# authors: Ethosa, Konard

from retranslator import Translator


class Cpp2Java(Translator):
    """Translator of C ++ code to Java code

    Extends:
        Translator
    """
    def __init__(self, codeString="", rules=[], useRegex=False):
        """initialize class

        Keyword Arguments:
            codeString {str} -- source code on C++ (default: {""})
            rules {list} -- include your own rules (default: {[]})
            useRegex {bool} -- this parameter tells you to use regex (default: {False})
        """
        rules.extend(Cpp2Java.RULES)
        Translator.__init__(self, codeString, rules, useRegex)

    RULES = [
        # #include ... using ...
        # #include ... using ... class Program { ... }
        (r"\A[\r\n]*(?P<begin>((#include[^\r\n]+|using[^\r\n]+|\s+)[\r\n]+)+)(?P<body>(.|\n)+)$",
         r"\g<begin>class Program {\n\g<body>}", None, 0),

        # Set cursor
        # class Program {\r\n
        # class Program {\r\n\t/*cursor*/
        (r"(?P<begin>class Program {[^\r\n]*)\r?\n", r"\g<begin>\n\t/*cursor*/", None, 0),

        # Move cursor
        # /*cursor*/...\r\n
        # ...\r\n\t/*cursor*/
        (r"/\*cursor\*/(?P<begin>[^\r\n]+)\r?\n(?!}$)", r"\g<begin>\n\t/*cursor*/", None, 1000),

        # Remove cursor
        # /*cursor*/
        #
        (r"/\*cursor\*/", r"", None, 0),

        # int main()
        # public static void main(String[] args)
        (r"int main\([^\)\r\n]*\)", r"public static void main(String[] args)", None, 0),

        # std::string
        # String
        (r"(std::)?string", r"String", None, 0),

        # word.clear();
        # word = "";
        (r"(?P<variable>[_a-zA-Z0-9]+)\.clear\(\)", r"\g<variable> = \"\"", None, 0),

        # x.c_str()
        # x
        (r"(?P<variable>[_a-zA-Z0-9]+)\.c_str\(\)", r"\g<variable>", None, 0),

        # void a()
        # {
        # }
        # ----------
        # void a() {
        # }
        ((r"(?P<begin>[\r\n]*)(?P<indent>[^#/\*]{1,2}[ ]*)(?P<blockInfo>[^\r\n{};]+)[\r\n]+(?P=indent){"),
         (r"\g<begin>\g<indent>\g<blockInfo> {"),
         None, 0),

        # str.pop_back()
        # str = str.substring(0, str.length() - 1)
        (r"(?P<variable>[_a-zA-Z0-9]+)\.pop_back\(\)",
         r"\g<variable> = \g<variable>.substring(0, \g<variable>.length() - 1)", None, 0),

        # cout << text;
        # System.out.print(text);
        (r"(std::)?cout[ ]*<<[ ]*(?P<couted>[\S ]+);", r"System.out.print(\g<couted>);", None, 0),

        # System.out.print(... << ...)
        # System.out.print(... + ...)
        (r"System.out.print\([ ]*([^<]+)<<[ ]*([^\)]+)\);", r'System.out.print(\1+ \2);', None, 1000),

        # std::endl
        # "\n"
        (r"(std::)?endl", r'"\\n"', None, 0),

        # int main() { ... getline
        # int main() { Scanner input = new Scanner(System.in); ... getline
        ((r"(?P<method>\r?\n[ \t]*[a-zA-Z ]+ [a-zA-Z]+[ \t]*\([^\)\r\n]*\)([^\{]|\n)+"
          r"{[\r\n]*)(?P<indent>[ \t]+)(?P<end>((?!(Scanner input = new Scanner\(Syst"
          r"em\.in\);|(?P=indent)[^\r\n]+\r?\n[ \t]+}))(.|\n))+?getline\(cin)"),
         r"\g<method>\g<indent>Scanner input = new Scanner(System.in);\n\g<indent>\g<end>", None, 0),

        # ^ ... Scanner
        # ^ import java.util.Scanner; ... Scanner
        (r"\A(?P<begin>((?!import java\.util\.Scanner;)(.|\n))+?)Scanner",
         r"import java.util.Scanner;\n\g<begin>Scanner", None, 0),

        # getline(cin, x);
        # x = input.nextLine();
        (r"getline\(cin, (?P<variable>[_a-zA-Z0-9]+)\);",
         r"\g<variable> = input.nextLine();", None, 0),

        # fstream file(path, std::ios::in);
        # FileInputStream file = new FileInputStream(path);
        (r"fstream (?P<stream>[_a-zA-Z0-9]+)\((?P<path>[_a-zA-Z0-9]+), std::ios::in\);",
         r"FileInputStream \g<stream> = new FileInputStream(\g<path>);", None, 0),

        # ^ ... FileInputStream
        # ^ import java.io.FileInputStream; ... FileInputStream
        (r"\A(?P<begin>((?!import java\.io\.FileInputStream;)(.|\n))+?)FileInputStream",
         r"import java.io.FileInputStream;\n\g<begin>FileInputStream", None, 0),

        # fstream file(path, std::ios::out);
        # FileOutputStream file = new FileOutputStream(path);
        (r"fstream (?P<stream>[_a-zA-Z0-9]+)\((?P<path>[_a-zA-Z0-9]+), std::ios::out\);",
         r"FileOutputStream \g<stream> = new FileOutputStream(\g<path>);", None, 0),

        # ^ ... FileOutputStream
        # ^ import java.io.FileOutputStream; ... FileOutputStream
        (r"\A(?P<begin>((?!import java\.io\.FileOutputStream;)(.|\n))+?)FileOutputStream",
         r"import java.io.FileOutputStream;\n\g<begin>FileOutputStream", None, 0),

        # &x
        # x
        (r"&(?P<spaces>[ ]*)(?P<variable>[_a-zA-Z0-9]+)", r"\g<spaces>\g<variable>", None, 0),

        # ... auto
        # ... var
        ((r"(?P<begin>[\r\n]+)(?P<before>[^#/\*]{1,2}[^\"]*)auto"),
         (r"\g<begin>\g<before>var"),
         None, 0),

        # [Other case]
        # ... auto
        # ... var
        ((r"(?P<begin>[^\"]+[\s]+)auto"),
         (r"\g<begin>var"),
         None, 0),

        # catch (char* msg)
        # catch (Exception e/*replacement*msg*e.getMessage()*/)
        (r"catch \(char\* (?P<message>[_a-zA-Z0-9]+)\)",
         r"catch (Exception e/*replacement*\g<message>*e.getMessage()*/)", None, 0),

        # std::map<x, x>
        # HashMap<x, x>
        (r"(std::)?map[ ]*<[ ]*(?P<key>[_a-zA-Z0-9]+)[ ]*,[ ]*(?P<value>[_a-zA-Z0-9]+)[ ]*>",
         r"HashMap<\g<key>, \g<value>>", None, 0),

        # typedef HashMap<String, String> DictArr;
        # /*replacement*DictArr*HashMap<String, String>*/
        (r"typedef (?P<type>[^;\r\n]+) (?P<alias>[_a-zA-Z0-9]+);",
         r"/*replacement*\g<alias>*\g<type>*/", None, 0),

        # /*replacement*msg*e.getMessage()*/ ... msg
        # /*replacement*msg*e.getMessage()*/ ... e.getMessage()
        ((r"(?P<begin>/\*replacement\*(?P<pattern>[^\*\r\n]+)"
          r"\*(?P<substitution>[^\*\r\n]+)\*/(.|\n)+)(?P=pattern)"),
         r"\g<begin>\g<substitution>", None, 1000),

        # HashMap<String, String>::iterator iter = dictionary.find(word);
        # /*replacement*iter*dictionary.containsKey(word)*/
        ((r"HashMap<[_a-zA-Z0-9]+, [_a-zA-Z0-9]+>::iterator"
          r" (?P<iterator>[_a-zA-Z0-9]+) = (?P<map>[_a-zA-Z0-9]+)\.find\((?P<key>[_a-zA-Z0-9]+)\);"),
         r"/*replacement*\g<iterator>*\g<map>.containsKey(\g<key>)*/", None, 0),

        # /*replacement*msg*e.getMessage()*/ ... msg
        # /*replacement*msg*e.getMessage()*/ ... e.getMessage()
        ((r"(?P<begin>/\*replacement\*(?P<pattern>[^\*\r\n]+)"
          r"\*(?P<substitution>[^\*\r\n]+)\*/(.|\n)+)(?P=pattern)"),
         r"\g<begin>\g<substitution>", None, 1000),

        # /*replacement*msg*e.getMessage()*/
        #
        (r"/\*replacement\*[^\*\r\n]+\*[^\*\r\n]+\*/", r"", None, 0),

        # _getch();
        # System.in.read()
        (r"_getch\(\);", r"System.in.read();", None, 0),

        # HashMap<String, String> dictionary;
        # HashMap<String, String> dictionary = new HashMap<String, String>();
        (r"(?P<type>HashMap<[_a-zA-Z0-9]+, [_a-zA-Z0-9]+>)[ ]*(?P<variable>[_a-zA-Z0-9]+);",
         r"\g<type> \g<variable> = new \g<type>();", None, 0),

        # HashMap... dictionary ...
        # dictionary["hello"] = 1;
        # -------------------------
        # HashMap... dictionary ...
        # dictionary.put("hello", 1);
        ((r"HashMap<(?P<type>[^>]+)> (?P<var>[_a-zA-Z0-9]+)(?P<other>[\s\S]+)"
          r"(?P<var_again>(?P=var))[ ]*\[[ ]*(?P<key>[^\]]+)\]"
          r"[ ]*=[ ]*(?P<value>[^;]+)"),
         (r"HashMap<\g<type>> \g<var>\g<other>\g<var_again>.put(\g<key>, \g<value>)"),
         None, 1000),

        # HashMap... dictionary ...
        # dictionary["hello"]
        # -------------------------
        # HashMap... dictionary ...
        # dictionary.get("hello")
        ((r"HashMap<(?P<type>[^>]+)> (?P<var>[_a-zA-Z0-9]+)(?P<other>[\s\S]+)"
          r"(?P<var_again>(?P=var))[ ]*\[[ ]*(?P<key>[^\]]+)\]"),
         (r"HashMap<\g<type>> \g<var>\g<other>\g<var_again>.get(\g<key>)"),
         None, 1000),

        # dictionary.containsKey(word) == dictionary.end()
        # dictionary.containsKey(word)
        ((r"(?P<baseExpression>(?P<variable>[_a-zA-Z0-9]+)"
          r"\.containsKey\([_a-zA-Z0-9]+\)) == (?P=variable)\.end\(\)"),
         r"!\g<baseExpression>", None, 0),

        # ^ ... HashMap
        # ^ import java.util.HashMap; ... HashMap
        (r"\A(?P<begin>((?!import java\.util\.HashMap;)(.|\n))+?)HashMap",
         r"import java.util.HashMap;\n\g<begin>HashMap", None, 0),

        # ---------=== std::vector ===---------
        # std::vector<x>
        # ArrayList<x>
        ((r"(std::)?vector[ ]*<[ ]*(?P<key>[_a-zA-Z0-9]+)[ ]*>"),
         (r"ArrayList<\g<key>>"),
         None, 0),

        # array.at(0)
        # array.get(0)
        ((r"ArrayList<(?P<type>[^>]+)> (?P<var>[_a-zA-Z0-9]+)(?P<other>[\s\S]+)"
          r"(?P<var_again>(?P=var)).at\([ ]*(?P<key>[^\)]+)\)"),
         (r"ArrayList<\g<type>> \g<var>\g<other>\g<var_again>.get(\g<key>)"),
         None, 1000),

        # array[0]
        # array.get(0)
        ((r"ArrayList<(?P<type>[^>]+)> (?P<var>[_a-zA-Z0-9]+)(?P<other>[\s\S]+)"
          r"(?P<var_again>(?P=var))\[[ ]*(?P<key>[^\]]+)\]"),
         (r"ArrayList<\g<type>> \g<var>\g<other>\g<var_again>.get(\g<key>)"),
         None, 1000),

        # array.push_back(0)
        # array.add(0)
        ((r"ArrayList<(?P<type>[^>]+)> (?P<var>[_a-zA-Z0-9]+)(?P<other>[\s\S]+)"
          r"(?P<var_again>(?P=var)).push_back\([ ]*(?P<val>[^\)]+)\)"),
         (r"ArrayList<\g<type>> \g<var>\g<other>\g<var_again>.add(\g<val>)"),
         None, 1000),

        # array.size()
        # array.length()
        ((r"ArrayList<(?P<type>[^>]+)> (?P<var>[_a-zA-Z0-9]+)(?P<other>[\s\S]+)"
          r"(?P<var_again>(?P=var)).size\([ ]*\)"),
         (r"ArrayList<\g<type>> \g<var>\g<other>\g<var_again>.length()"),
         None, 1000),

        # ArrayList<String> array;
        # ArrayList<String> array = new ArrayList<String>();
        (r"(?P<type>ArrayList<[_a-zA-Z0-9]+>)[ ]*(?P<variable>[_a-zA-Z0-9]+);",
         r"\g<type> \g<variable> = new \g<type>();", None, 0),

        # ^ ... ArrayList
        # ^ import java.util.ArrayList; ... ArrayList
        (r"\A(?P<begin>((?!import java\.util\.ArrayList;)(.|\n))+?)ArrayList",
         r"import java.util.ArrayList;\n\g<begin>ArrayList", None, 0),

        #
        #
        ((r""),
         (r""),
         None, 0)
    ]
