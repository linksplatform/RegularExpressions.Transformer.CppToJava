#include <iostream>
#include <fstream>
#include <map>
#include <windows.h>
#include <conio.h>

using namespace std;

int main()
{
    SetConsoleCP(1251);
    map<string, string> dictionary;
    string userPath;
    cout << "Введите полный путь к файлу со словарём: ";
    getline(cin, userPath);
    
    fstream file(userPath, std::ios::in);
    if (!file)
    {
        throw "Файл не найден.";
    }
    
    string line;
    string word;
    while (getline(file, line))
    {
        for (unsigned j = 0; j < line.length(); j++)
        {
            if (line[j] != '\t' && line[j] != ' ')
            {
                word += line[j];
            }
            else
            {
                break;
            }
        }
        
        string definition;
        for (unsigned i = word.length(); i < line.length(); i++)
        {
            if (line[i] != '\t' && line[i] != ' ')
            {
                for (unsigned j = i; j < line.length(); j++)
                {
                    definition += line[j];
                }
                break;
            }
        }
        
        dictionary[word] = definition;
        word.clear();
        definition.clear();
    }
    
    file.close();
    cout << "Введите полный путь к файлу с текстом: ";
    getline(cin, userPath);
    
    fstream inputFile(userPath, std::ios::in);
    if (!inputFile)
    {
        throw "Файл не найден.";
    }
    
    cout << "Введите полный путь к выходному файлу: ";
    string outputPath;
    getline(cin, outputPath);
    
    fstream outputFile(outputPath, std::ios::out);
    if (!outputFile)
    {
        throw "Не удалось создать выходной файл.";
    }
    
    string text;
    string finalText;
    while (getline(inputFile, text))
    {
        string currentWord;
        for (unsigned i = 0; i < text.length(); i++)
        {
            if (text[i] != ' ' && text[i] != ',' && text[i] != '.' && text[i] != '!' && text[i] != '?' && text[i] != ':' && text[i] != ';')
            {
                currentWord += text[i];
            }
            else
            {
                map<string, string>::iterator iter = dictionary.find(currentWord);
                if (iter != dictionary.end())
                {
                    finalText += iter->second;
                }
                else
                {
                    finalText += currentWord;
                }
                finalText += text[i];
                currentWord.clear();
            }
        }
        
        if (!currentWord.empty())
        {
            map<string, string>::iterator iter = dictionary.find(currentWord);
            if (iter != dictionary.end())
            {
                finalText += iter->second;
            }
            else
            {
                finalText += currentWord;
            }
            currentWord.clear();
        }
        
        finalText += '\n';
    }
    
    finalText.pop_back();
    outputFile << finalText;
    
    inputFile.close();
    outputFile.close();
    
    cout << "Обработка завершена.";
    _getch();
    
    return 0;
}