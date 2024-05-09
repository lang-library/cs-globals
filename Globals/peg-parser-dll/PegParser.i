/* 変更 */
%module (directors="1") PegParserDLL

%{
#include "pegparser.h"
%}

%include <windows.i> 
%include <std_string.i>
%include <std_wstring.i>
%include <std_vector.i>
%include <std_shared_ptr.i>

%shared_ptr(PegAST)
%shared_ptr(PegResult)

%template(PegASTVector) std::vector<std::shared_ptr<PegAST>>;

//%feature("director") CallbackBase;

%include "pegparser.h"
