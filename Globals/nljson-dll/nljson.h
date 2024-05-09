#ifndef NLJSON_H
#define NLJSON_H

#include <string>
#include <vector>
#include <map>
#include <memory>

//#include "peglib.h"

enum NLJsonType
{
    nl_boolean,
    nl_null,
    nl_number,
    nl_string,
    nl_object,
    nl_array
};

class NLJsonAST
{
public:
    std::wstring name;
    NLJsonType type;
    std::wstring token;
    std::vector<std::shared_ptr<NLJsonAST>> vect;
    std::map<std::wstring, std::shared_ptr<NLJsonAST>> dict;
};

class NLJsonResult
{
public:
    bool error;
    std::wstring error_msg;
    std::shared_ptr<NLJsonAST> ast;
};

class NLJsonParser
{
private:
public:
    explicit NLJsonParser();
    std::shared_ptr<NLJsonResult> Parse(const std::wstring& input);
};

extern std::shared_ptr<NLJsonParser> CreateParser();

#endif // NLJSON_H
