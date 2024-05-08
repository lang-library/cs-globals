#include "dllmain.h"

#include <nlohmann/json.hpp>
using nljson = nlohmann::json;

#include "peglib-dll.h"

#include "strconv.h"
#include <windows.h>

extern IMAGE_DOS_HEADER __ImageBase;

const std::wstring ModulePath()
{
    static thread_local wchar_t fileName[_MAX_PATH];
    ::GetModuleFileNameW((HMODULE)&__ImageBase, fileName, sizeof(fileName));
    return fileName;
}

static nljson add2(const nljson &args)
{
    if (args.size() != 2) throw std::runtime_error(format("add2() expects 2 arguments but %u passed", args.size()));
    return ((double)args.at(0)) + ((double)args.at(1));
}

static ::unicode_ostream cout(std::cout, ::GetConsoleOutputCP());
static ::unicode_ostream cerr(std::cerr, ::GetConsoleOutputCP());

static nljson write_to_stdout(const nljson &args)
{
    if (args.size() != 1) throw std::runtime_error(format("write_to_stdout() expects 1 arguments but %u passed", args.size()));
    std::string s = (std::string)args[0];
    cout << s << std::endl;
    return nullptr;
}

static nljson write_to_stderr(const nljson &args)
{
    if (args.size() != 1) throw std::runtime_error(format("write_to_stdout() expects 1 arguments but %u passed", args.size()));
    std::string s = (std::string)args[0];
    cerr << s << std::endl;
    return nullptr;
}

static nljson parse(const nljson &args)
{
    if (args.size() != 2) throw std::runtime_error(format("@arse() expects 2 arguments but %u passed", args.size()));
    std::string grammar = (std::string)args[0];
    std::string input = (std::string)args[1];
    //cout << "grammar: " << grammar << std::endl;
    //cout << "input: " << input << std::endl;
    return parse_to_nljson(grammar, input);
}

//static thread_local std::string lastError = "";

extern "C"
__declspec(dllexport)
const char* Call(const char*namePtr, const char* inputPtr)
{
    std::string name = namePtr;
    std::string input = inputPtr;
    nljson args = nljson::parse(input);
    //lastError = "";
    nljson result = nljson();
    try
    {
        if (name=="add2")
        {
            result = add2(args);
        }
        else if (name=="write_to_stdout")
        {
            result = write_to_stdout(args);
        }
        else if (name=="write_to_stderr")
        {
            result = write_to_stderr(args);
        }
        else if (name=="parse")
        {
            result = parse(args);
        }
        else
        {
            throw std::runtime_error(format("%s() not defined", name.c_str()));
        }
        nljson array = nljson::array();
        array.push_back(result);
        result = array;
    }
    catch (std::exception& e)
    {
        std::string err_msg = /*std::string("error: ") + */ e.what();
        //lastError = err_msg;
        result = err_msg;
    }
    static thread_local std::string jsonStdStr;
    jsonStdStr = result.dump(2);
    return jsonStdStr.c_str();
}

#if fales
extern "C"
__declspec(dllexport)
const char* LastError()
{
    return lastError.c_str();
}
#endif
