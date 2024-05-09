#ifndef PEGPARSER_H
#define PEGPARSER_H

#include <string>
#include <vector>
#include <memory>

#include "peglib.h"

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

#if 0x0
class PegParser
{
public:
    //explicit PegParser();
    std::shared_ptr<PegResult> Parse(const std::wstring& grammar, const std::wstring& input);
};
#else
class PegParser
{
private:
    std::shared_ptr<peg::parser> parser_ptr;
public:
    explicit PegParser(const std::wstring& grammar);
    std::shared_ptr<PegResult> Parse(const std::wstring& input);
    //std::shared_ptr<PegResult> Parse(const void* input);
};
#endif

extern std::shared_ptr<PegParser> CreateParser(const std::wstring& grammar);

#endif // PEGPARSER_H
