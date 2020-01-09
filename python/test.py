# -*- coding: utf-8 -*-
# authors: Ethosa, Konard

from cpp2java import Cpp2Java

if __name__ == "__main__":
    cpp2java = Cpp2Java(useRegex=True)
    script = """
#include <iostream>
#include <array>
#include <vector>
#include <map>

int main()
{
    std::cout << "Hello world" << std::endl << "Hello" << std::endl << "World" << std::endl;
    std::vector<std::string> vector;
    vector.push_back("hello");
    cout << vector.at(0) << ", " << vector[0] << std::endl;
    cout << vector.size() << std::endl;
    std::map<std::string, int> map;
    map["hello"] = 123123;
    cout << map["hello"];
    if (true)
    {
    }
    return 0;
    auto i = 0;
    for (auto& j: vector)
    {
        j += " auto";
    }
}
"""
    print(cpp2java.translate(script))
