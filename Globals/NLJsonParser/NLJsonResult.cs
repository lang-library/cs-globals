//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.2.1
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class NLJsonResult : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnBase;

  internal NLJsonResult(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwnBase = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(NLJsonResult obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~NLJsonResult() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwnBase) {
          swigCMemOwnBase = false;
          NLJsonParserDLLPINVOKE.delete_NLJsonResult(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public bool error {
    set {
      NLJsonParserDLLPINVOKE.NLJsonResult_error_set(swigCPtr, value);
      if (NLJsonParserDLLPINVOKE.SWIGPendingException.Pending) throw NLJsonParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      bool ret = NLJsonParserDLLPINVOKE.NLJsonResult_error_get(swigCPtr);
      if (NLJsonParserDLLPINVOKE.SWIGPendingException.Pending) throw NLJsonParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string error_msg {
    set {
      NLJsonParserDLLPINVOKE.NLJsonResult_error_msg_set(swigCPtr, value);
      if (NLJsonParserDLLPINVOKE.SWIGPendingException.Pending) throw NLJsonParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = NLJsonParserDLLPINVOKE.NLJsonResult_error_msg_get(swigCPtr);
      if (NLJsonParserDLLPINVOKE.SWIGPendingException.Pending) throw NLJsonParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public NLJsonAST ast {
    set {
      NLJsonParserDLLPINVOKE.NLJsonResult_ast_set(swigCPtr, NLJsonAST.getCPtr(value));
      if (NLJsonParserDLLPINVOKE.SWIGPendingException.Pending) throw NLJsonParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      global::System.IntPtr cPtr = NLJsonParserDLLPINVOKE.NLJsonResult_ast_get(swigCPtr);
      NLJsonAST ret = (cPtr == global::System.IntPtr.Zero) ? null : new NLJsonAST(cPtr, true);
      if (NLJsonParserDLLPINVOKE.SWIGPendingException.Pending) throw NLJsonParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public NLJsonResult() : this(NLJsonParserDLLPINVOKE.new_NLJsonResult(), true) {
  }

}
