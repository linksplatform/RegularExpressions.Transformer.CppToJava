# Analysis of Missing Transformation Rules

## Current Issues Based on Manual Translation

### 1. Exception Handling
- **C++**: `throw "string literal";`
- **Expected Java**: `throw new Exception("string literal");`
- **Current Rule**: Only covers `catch (char* msg)` but not `throw` statements

### 2. File Operations and Scanner Usage
- **C++**: Multiple `getline(file, line)` with file streams
- **Expected Java**: Needs `Scanner fileInput = new Scanner(file)` and `fileInput.hasNextLine()` + `fileInput.nextLine()`
- **Current Rule**: Only handles `getline(cin, x)` to `input.nextLine()` but not file streams

### 3. Loop Control and Conditions
- **C++**: `while (getline(file, line))`
- **Expected Java**: `while (fileInput.hasNextLine()) { line = fileInput.nextLine(); }`
- **Current Rule**: Missing transformation for file-based getline in while loops

### 4. String Character Access
- **C++**: `line[j]` and `text[i]`
- **Expected Java**: `line.charAt(j)` and `text.charAt(i)`
- **Current Rule**: Missing transformation for string indexing

### 5. String Methods
- **C++**: `currentWord.empty()`
- **Expected Java**: `currentWord.isEmpty()`
- **Current Rule**: Missing transformation for empty() method

### 6. Map Operations
- **C++**: `dictionary[word] = definition;` and `dictionary.get(word)`
- **Expected Java**: `dictionary.put(word, definition);` and `dictionary.get(word)`
- **Current Rule**: Missing transformation for map assignment and access

### 7. File Closing and Resource Management
- **C++**: `file.close()`
- **Expected Java**: `fileInput.close(); file.close();` (for Scanner and FileInputStream)
- **Current Rule**: Missing transformation for file closing

### 8. Console Output with Variables
- **C++**: Complex cout statements with string concatenation
- **Expected Java**: System.out.print() with string concatenation
- **Current Rule**: Covers simple cases but not all patterns

### 9. Method Calls and Windows-specific Functions  
- **C++**: `SetConsoleCP(1251);`
- **Expected Java**: Should be removed or commented out
- **Current Rule**: Missing transformation for Windows-specific functions

### 10. File I/O with PrintWriter
- **C++**: `outputFile << finalText;`
- **Expected Java**: Needs `PrintWriter` and `writer.print(finalText);`
- **Current Rule**: Missing transformation for output stream operations

### 11. Variable Declarations in Try-Catch
- **C++**: Variables declared outside try-catch
- **Expected Java**: Need proper exception handling structure
- **Current Rule**: Missing comprehensive exception handling structure

### 12. Loop Variables and Casting
- **C++**: `unsigned i`, `unsigned j`
- **Expected Java**: `int i`, `int j`
- **Current Rule**: Missing transformation for unsigned to int