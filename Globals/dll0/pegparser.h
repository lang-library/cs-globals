#ifndef PEGPARSER_H
#define PEGPARSER_H

#include <string>
#include <vector>
#include <memory>

class PegAST
{
public:
    std::wstring name;
    bool is_token;
    std::wstring token;
    int choice;
    std::wstring name_choice;
    std::vector<std::shared_ptr<PegAST>> nodes;
};

class PegResult
{
public:
    bool error;
    std::wstring error_msg;
    std::shared_ptr<PegAST> ast;
};

class PegParser
{
public:
    //explicit PegParser();
    std::shared_ptr<PegResult> Parse(const std::wstring& grammar, const std::wstring& input);
};

#endif // PEGPARSER_H
