%module (directors="1") NLJsonParserDLL

%{
#include "nljson.h"
%}

%include <windows.i> 
%include <std_string.i>
%include <std_wstring.i>
%include <std_map.i>
%include <std_vector.i>
%include <std_shared_ptr.i>

%shared_ptr(NLJsonAST)
%shared_ptr(NLJsonResult)
%shared_ptr(NLJsonParser)

%template(NLJsonASTDictionary) std::map<std::wstring, std::shared_ptr<NLJsonAST>>;

%template(NLJsonASTList) std::vector<std::shared_ptr<NLJsonAST>>;

//%feature("director") CallbackBase;

%typemap(ctype)  void* "void *"
%typemap(imtype) void* "System.IntPtr"
%typemap(cstype) void* "System.IntPtr"
%typemap(csin)   void* "$csinput"
%typemap(in)     void* %{ $1 = $input; %}
%typemap(out)    void* %{ $result = $1; %}
%typemap(csout, excode=SWIGEXCODE)  void* {
    System.IntPtr cPtr = $imcall;$excode
    return cPtr;
    }
%typemap(csvarout, excode=SWIGEXCODE2) void* %{
    get {
        System.IntPtr cPtr = $imcall;$excode
        return cPtr;
   }
%}

%include "nljson.h"
