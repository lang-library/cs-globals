#include "pegparser.h"
#include "peglib.h"
#include "strconv.h"


::unicode_ostream uout(std::cout, ::GetConsoleOutputCP());

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
    uout << "(A)" << result->name << " " << result->is_token << std::endl;
    if (ast.is_token)
    {
        uout << "(B.1)" << ast.token.size() << std::endl;
        std::string utf8_token = std::string(ast.token.begin(), ast.token.end());
        #if 0x0
        std::string utf8_token;
        for (size_t i=0; i<ast.token.size(); i++)
        {
            uout << "i=" << i << std::endl;
            utf8_token += ast.token[i];
        }
        #endif
        uout << "(B.1A)" << std::endl;
        std::wstring token = utf8_to_wide(utf8_token);
        uout << "(B.2)" << std::endl;
        result->token = token;
        uout << "(B.3)" << std::endl;
        result->name_choice = result->name;
        uout << "(B.4)" << std::endl;
    }
    else
    {
        result->choice = ast.choice;
        result->name_choice = ::format(L"%s/%u", utf8_to_wide(ast.name).c_str(), ast.choice);
        for (auto node : ast.nodes) {
            result->nodes.push_back(convert_ast(node));
        }
    }
    uout << "(Z)" << std::endl;
    return std::shared_ptr<PegAST>(result);
}

PegParser::PegParser(const std::wstring& grammar)
{
    this->parser_ptr = std::shared_ptr<peg::parser>(new peg::parser(wide_to_utf8(grammar)));
}

std::shared_ptr<PegResult> PegParser::Parse(const std::wstring &input)
//std::shared_ptr<PegResult> PegParser::Parse(const void* input)
{
    std::string utf8_input = wide_to_utf8(input);
    uout << "(1)" << std::endl;
    //peg::parser parser(wide_to_utf8(grammar));
    auto &parser = *(this->parser_ptr);
    PegResult *result = new PegResult();
    if (static_cast<bool>(parser) == false)
    {
        result->error = true;
        result->error_msg = L"[grammar_error]";
        return std::shared_ptr<PegResult>(result);
    }
    uout << "(2)" << std::endl;
    parser.enable_ast();
    std::shared_ptr<peg::Ast> ast_ptr;
    if (!parser.parse(utf8_input, ast_ptr))
    {
        result->error = true;
        result->error_msg = L"[input_error]";
        return std::shared_ptr<PegResult>(result);
    }
    uout << "(3)" << std::endl;
    result->ast = convert_ast(ast_ptr);
    uout << "(4)" << std::endl;
    return std::shared_ptr<PegResult>(result);
}

std::shared_ptr<PegParser> CreateParser(const std::wstring& grammar)
{
    return std::shared_ptr<PegParser>(new PegParser(grammar));
}
