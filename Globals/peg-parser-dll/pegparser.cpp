#include "pegparser.h"
#include "peglib.h"
#include "strconv.h"

#if false
PegParser::PegParser()
{
}
#endif

static inline std::shared_ptr<PegAST> convert_ast(std::shared_ptr<peg::Ast> ast_ptr)
{
    const auto &ast = *ast_ptr;
    PegAST* result = new PegAST();
    result->name = utf8_to_wide(ast.name);
    result->is_token = ast.is_token;
    if (ast.is_token)
    {
        std::wstring token = utf8_to_wide(std::string(ast.token));
        result->token = token;
        result->name_choice = result->name;
    }
    else
    {
        result->choice = ast.choice;
        result->name_choice = ::format(L"%s/%u", utf8_to_wide(ast.name).c_str(), ast.choice);
        for (auto node : ast.nodes) {
            result->nodes.push_back(convert_ast(node));
        }
    }
    return std::shared_ptr<PegAST>(result);
}

std::shared_ptr<PegResult> PegParser::Parse(const std::wstring &grammar, const std::wstring &input)
{
    peg::parser parser(wide_to_utf8(grammar));
    PegResult *result = new PegResult();
    if (static_cast<bool>(parser) == false)
    {
        result->error = true;
        result->error_msg = L"[grammar_error]";
        return std::shared_ptr<PegResult>(result);
    }
    parser.enable_ast();
    std::shared_ptr<peg::Ast> ast_ptr;
    if (!parser.parse(wide_to_utf8(input), ast_ptr))
    {
        result->error = true;
        result->error_msg = L"[input_error]";
        return std::shared_ptr<PegResult>(result);
    }
    result->ast = convert_ast(ast_ptr);
    return std::shared_ptr<PegResult>(result);
}
