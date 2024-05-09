#include "nljson.h"
#include "strconv.h"

#include <nlohmann/json.hpp>
//using nljson = nlohmann::json;
using nljson = nlohmann::basic_json<std::map, std::vector, std::string, bool,
                                     std::int64_t, std::uint64_t, long double>;

::unicode_ostream uout(std::cout, ::GetConsoleOutputCP());

static inline std::shared_ptr<NLJsonAST> convert_ast(nljson nl)
{
    NLJsonAST* result = new NLJsonAST();
    switch(nl.type())
    {
    case nljson::value_t::array:
        result->name = L"array";
        result->type = nl_array;
        result->token = L"[]";
        for (size_t i=0; i<nl.size(); i++)
        {
            result->vect.push_back(convert_ast(nl[i]));
        }
        return std::shared_ptr<NLJsonAST>(result);
    case nljson::value_t::boolean:
        result->name = L"boolean";
        result->type = nl_boolean;
        result->token = nl.get<bool>() ? L"true" : L"false";
        return std::shared_ptr<NLJsonAST>(result);
    case nljson::value_t::null:
        result->name = L"null";
        result->type = nl_null;
        result->token = L"null";
        return std::shared_ptr<NLJsonAST>(result);
    case nljson::value_t::number_float:
    case nljson::value_t::number_integer:
    case nljson::value_t::number_unsigned:
        result->name = L"number";
        result->type = nl_number;
        result->token = std::to_wstring(nl.get<long double>());
        return std::shared_ptr<NLJsonAST>(result);
    case nljson::value_t::object:
        result->name = L"object";
        result->type = nl_object;
        result->token = L"{:}";
        for (auto& entry : nl.items())
        {
          result->dict[utf8_to_wide(entry.key())] = convert_ast(entry.value());
        }
        return std::shared_ptr<NLJsonAST>(result);
    case nljson::value_t::string:
        result->name = L"string";
        result->type = nl_string;
        result->token = utf8_to_wide(nl.get<std::string>());
        return std::shared_ptr<NLJsonAST>(result);
    default:
        result->name = L"null";
        result->type = nl_null;
        result->token = L"null";
        return std::shared_ptr<NLJsonAST>(result);
    }
}

NLJsonParser::NLJsonParser()
{
}

std::shared_ptr<NLJsonResult> NLJsonParser::Parse(const std::wstring &input)
{
    std::string utf8_input = wide_to_utf8(input);
    NLJsonResult *result = new NLJsonResult();
    try
    {
        result->error = false;
        nljson nl = nljson::parse(
            utf8_input,
            /* callback */ nullptr,
            /* allow exceptions */ false,
            /* ignore_comments */ true);
        result->ast = convert_ast(nl);
    }
    catch (...)
    {
        result->error = true;
        result->error_msg = L"[input_error]";
    }
    return std::shared_ptr<NLJsonResult>(result);
}

std::shared_ptr<NLJsonParser> CreateParser()
{
    return std::shared_ptr<NLJsonParser>(new NLJsonParser());
}
