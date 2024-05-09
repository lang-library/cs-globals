//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.2.1
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class PegAST : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  private bool swigCMemOwnBase;

  internal PegAST(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwnBase = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PegAST obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~PegAST() {
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
          PegParserDLLPINVOKE.delete_PegAST(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public string name {
    set {
      PegParserDLLPINVOKE.PegAST_name_set(swigCPtr, value);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = PegParserDLLPINVOKE.PegAST_name_get(swigCPtr);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public bool is_token {
    set {
      PegParserDLLPINVOKE.PegAST_is_token_set(swigCPtr, value);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      bool ret = PegParserDLLPINVOKE.PegAST_is_token_get(swigCPtr);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string token {
    set {
      PegParserDLLPINVOKE.PegAST_token_set(swigCPtr, value);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = PegParserDLLPINVOKE.PegAST_token_get(swigCPtr);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public int choice {
    set {
      PegParserDLLPINVOKE.PegAST_choice_set(swigCPtr, value);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      int ret = PegParserDLLPINVOKE.PegAST_choice_get(swigCPtr);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string name_choice {
    set {
      PegParserDLLPINVOKE.PegAST_name_choice_set(swigCPtr, value);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = PegParserDLLPINVOKE.PegAST_name_choice_get(swigCPtr);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public SWIGTYPE_p_std__vectorT_std__shared_ptrT_PegAST_t_t nodes {
    set {
      PegParserDLLPINVOKE.PegAST_nodes_set(swigCPtr, SWIGTYPE_p_std__vectorT_std__shared_ptrT_PegAST_t_t.getCPtr(value));
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      global::System.IntPtr cPtr = PegParserDLLPINVOKE.PegAST_nodes_get(swigCPtr);
      SWIGTYPE_p_std__vectorT_std__shared_ptrT_PegAST_t_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_std__vectorT_std__shared_ptrT_PegAST_t_t(cPtr, false);
      if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public PegAST() : this(PegParserDLLPINVOKE.new_PegAST(), true) {
  }

}
