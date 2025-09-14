#include <iostream>
#include <map>

using namespace std;

int main()
{
    map<string, string> dictionary;
    string userPath;
    dictionary[userPath] = "test";
    
    unsigned int i = 0;
    if (userPath[i] != ' ') {
        cout << "Found character: " << userPath[i];
    }
    
    if (!userPath.empty()) {
        throw "Error message";
    }
    
    return 0;
}