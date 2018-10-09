#include <iostream>
#include <string>
#include <locale>

using namespace std;

int main()
{
    cout << "Hello world!" << endl;
    string s1 = "中文";
    cout << s1  << endl;
    string s3 = "中文";
    wcout.imbue(locale(""));
    cout << s3  << endl;
    return 0;
}
