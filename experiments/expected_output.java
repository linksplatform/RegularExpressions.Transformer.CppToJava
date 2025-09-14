import java.util.HashMap;
import java.util.Scanner;
import java.io.FileInputStream;
import java.io.FileOutputStream;

class Program {
    public static void main(String[] args) throws Exception
    {
        HashMap<String, String> dictionary = new HashMap<String, String>();
        Scanner input = new Scanner(System.in);
        String userPath;
        System.out.print("Введите полный путь к файлу со словарём: ");
        userPath = input.nextLine();
        
        try {
            FileInputStream file = new FileInputStream(userPath);
            Scanner fileInput = new Scanner(file);
            
            String line;
            String word;
            while (fileInput.hasNextLine()) {
                line = fileInput.nextLine();
                word = "";
                
                for (int j = 0; j < line.length(); j++)
                {
                    if (line.charAt(j) != '\t' && line.charAt(j) != ' ')
                    {
                        word += line.charAt(j);
                    }
                    else
                    {
                        break;
                    }
                }
                
                String definition = "";
                for (int i = word.length(); i < line.length(); i++)
                {
                    if (line.charAt(i) != '\t' && line.charAt(i) != ' ')
                    {
                        for (int j = i; j < line.length(); j++)
                        {
                            definition += line.charAt(j);
                        }
                        break;
                    }
                }
                
                dictionary.put(word, definition);
                word = "";
                definition = "";
            }
            
            fileInput.close();
            file.close();
        } catch (Exception e) {
            throw new Exception("Файл не найден.");
        }
        
        System.out.print("Введите полный путь к файлу с текстом: ");
        userPath = input.nextLine();
        
        try {
            FileInputStream inputFile = new FileInputStream(userPath);
            Scanner textInput = new Scanner(inputFile);
            
            System.out.print("Введите полный путь к выходному файлу: ");
            String outputPath;
            outputPath = input.nextLine();
            
            FileOutputStream outputFile = new FileOutputStream(outputPath);
            java.io.PrintWriter writer = new java.io.PrintWriter(outputFile);
            
            String text;
            String finalText = "";
            while (textInput.hasNextLine()) {
                text = textInput.nextLine();
                String currentWord = "";
                for (int i = 0; i < text.length(); i++)
                {
                    if (text.charAt(i) != ' ' && text.charAt(i) != ',' && text.charAt(i) != '.' && text.charAt(i) != '!' && text.charAt(i) != '?' && text.charAt(i) != ':' && text.charAt(i) != ';')
                    {
                        currentWord += text.charAt(i);
                    }
                    else
                    {
                        if (dictionary.containsKey(currentWord))
                        {
                            finalText += dictionary.get(currentWord);
                        }
                        else
                        {
                            finalText += currentWord;
                        }
                        finalText += text.charAt(i);
                        currentWord = "";
                    }
                }
                
                if (!currentWord.isEmpty())
                {
                    if (dictionary.containsKey(currentWord))
                    {
                        finalText += dictionary.get(currentWord);
                    }
                    else
                    {
                        finalText += currentWord;
                    }
                    currentWord = "";
                }
                
                finalText += '\n';
            }
            
            finalText = finalText.substring(0, finalText.length() - 1);
            writer.print(finalText);
            
            textInput.close();
            inputFile.close();
            writer.close();
            outputFile.close();
            
        } catch (Exception e) {
            throw new Exception("Файл не найден.");
        }
        
        System.out.print("Обработка завершена.");
        System.in.read();
        
        input.close();
    }
}