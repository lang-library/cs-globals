//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.2.1
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class PegASTVector : global::System.IDisposable, global::System.Collections.IEnumerable, global::System.Collections.Generic.IEnumerable<PegAST>
 {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal PegASTVector(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PegASTVector obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(PegASTVector obj) {
    if (obj != null) {
      if (!obj.swigCMemOwn)
        throw new global::System.ApplicationException("Cannot release ownership as memory is not owned");
      global::System.Runtime.InteropServices.HandleRef ptr = obj.swigCPtr;
      obj.swigCMemOwn = false;
      obj.Dispose();
      return ptr;
    } else {
      return new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
    }
  }

  ~PegASTVector() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          PegParserDLLPINVOKE.delete_PegASTVector(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public PegASTVector(global::System.Collections.IEnumerable c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (PegAST element in c) {
      this.Add(element);
    }
  }

  public PegASTVector(global::System.Collections.Generic.IEnumerable<PegAST> c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (PegAST element in c) {
      this.Add(element);
    }
  }

  public bool IsFixedSize {
    get {
      return false;
    }
  }

  public bool IsReadOnly {
    get {
      return false;
    }
  }

  public PegAST this[int index]  {
    get {
      return getitem(index);
    }
    set {
      setitem(index, value);
    }
  }

  public int Capacity {
    get {
      return (int)capacity();
    }
    set {
      if (value < 0 || (uint)value < size())
        throw new global::System.ArgumentOutOfRangeException("Capacity");
      reserve((uint)value);
    }
  }

  public bool IsEmpty {
    get {
      return empty();
    }
  }

  public int Count {
    get {
      return (int)size();
    }
  }

  public bool IsSynchronized {
    get {
      return false;
    }
  }

  public void CopyTo(PegAST[] array)
  {
    CopyTo(0, array, 0, this.Count);
  }

  public void CopyTo(PegAST[] array, int arrayIndex)
  {
    CopyTo(0, array, arrayIndex, this.Count);
  }

  public void CopyTo(int index, PegAST[] array, int arrayIndex, int count)
  {
    if (array == null)
      throw new global::System.ArgumentNullException("array");
    if (index < 0)
      throw new global::System.ArgumentOutOfRangeException("index", "Value is less than zero");
    if (arrayIndex < 0)
      throw new global::System.ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
    if (count < 0)
      throw new global::System.ArgumentOutOfRangeException("count", "Value is less than zero");
    if (array.Rank > 1)
      throw new global::System.ArgumentException("Multi dimensional array.", "array");
    if (index+count > this.Count || arrayIndex+count > array.Length)
      throw new global::System.ArgumentException("Number of elements to copy is too large.");
    for (int i=0; i<count; i++)
      array.SetValue(getitemcopy(index+i), arrayIndex+i);
  }

  public PegAST[] ToArray() {
    PegAST[] array = new PegAST[this.Count];
    this.CopyTo(array);
    return array;
  }

  global::System.Collections.Generic.IEnumerator<PegAST> global::System.Collections.Generic.IEnumerable<PegAST>.GetEnumerator() {
    return new PegASTVectorEnumerator(this);
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
    return new PegASTVectorEnumerator(this);
  }

  public PegASTVectorEnumerator GetEnumerator() {
    return new PegASTVectorEnumerator(this);
  }

  // Type-safe enumerator
  /// Note that the IEnumerator documentation requires an InvalidOperationException to be thrown
  /// whenever the collection is modified. This has been done for changes in the size of the
  /// collection but not when one of the elements of the collection is modified as it is a bit
  /// tricky to detect unmanaged code that modifies the collection under our feet.
  public sealed class PegASTVectorEnumerator : global::System.Collections.IEnumerator
    , global::System.Collections.Generic.IEnumerator<PegAST>
  {
    private PegASTVector collectionRef;
    private int currentIndex;
    private object currentObject;
    private int currentSize;

    public PegASTVectorEnumerator(PegASTVector collection) {
      collectionRef = collection;
      currentIndex = -1;
      currentObject = null;
      currentSize = collectionRef.Count;
    }

    // Type-safe iterator Current
    public PegAST Current {
      get {
        if (currentIndex == -1)
          throw new global::System.InvalidOperationException("Enumeration not started.");
        if (currentIndex > currentSize - 1)
          throw new global::System.InvalidOperationException("Enumeration finished.");
        if (currentObject == null)
          throw new global::System.InvalidOperationException("Collection modified.");
        return (PegAST)currentObject;
      }
    }

    // Type-unsafe IEnumerator.Current
    object global::System.Collections.IEnumerator.Current {
      get {
        return Current;
      }
    }

    public bool MoveNext() {
      int size = collectionRef.Count;
      bool moveOkay = (currentIndex+1 < size) && (size == currentSize);
      if (moveOkay) {
        currentIndex++;
        currentObject = collectionRef[currentIndex];
      } else {
        currentObject = null;
      }
      return moveOkay;
    }

    public void Reset() {
      currentIndex = -1;
      currentObject = null;
      if (collectionRef.Count != currentSize) {
        throw new global::System.InvalidOperationException("Collection modified.");
      }
    }

    public void Dispose() {
        currentIndex = -1;
        currentObject = null;
    }
  }

  public PegASTVector() : this(PegParserDLLPINVOKE.new_PegASTVector__SWIG_0(), true) {
  }

  public PegASTVector(PegASTVector other) : this(PegParserDLLPINVOKE.new_PegASTVector__SWIG_1(PegASTVector.getCPtr(other)), true) {
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Clear() {
    PegParserDLLPINVOKE.PegASTVector_Clear(swigCPtr);
  }

  public void Add(PegAST x) {
    PegParserDLLPINVOKE.PegASTVector_Add(swigCPtr, PegAST.getCPtr(x));
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  private uint size() {
    uint ret = PegParserDLLPINVOKE.PegASTVector_size(swigCPtr);
    return ret;
  }

  private bool empty() {
    bool ret = PegParserDLLPINVOKE.PegASTVector_empty(swigCPtr);
    return ret;
  }

  private uint capacity() {
    uint ret = PegParserDLLPINVOKE.PegASTVector_capacity(swigCPtr);
    return ret;
  }

  private void reserve(uint n) {
    PegParserDLLPINVOKE.PegASTVector_reserve(swigCPtr, n);
  }

  public PegASTVector(int capacity) : this(PegParserDLLPINVOKE.new_PegASTVector__SWIG_2(capacity), true) {
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  private PegAST getitemcopy(int index) {
    global::System.IntPtr cPtr = PegParserDLLPINVOKE.PegASTVector_getitemcopy(swigCPtr, index);
    PegAST ret = (cPtr == global::System.IntPtr.Zero) ? null : new PegAST(cPtr, true);
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private PegAST getitem(int index) {
    global::System.IntPtr cPtr = PegParserDLLPINVOKE.PegASTVector_getitem(swigCPtr, index);
    PegAST ret = (cPtr == global::System.IntPtr.Zero) ? null : new PegAST(cPtr, true);
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private void setitem(int index, PegAST val) {
    PegParserDLLPINVOKE.PegASTVector_setitem(swigCPtr, index, PegAST.getCPtr(val));
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  public void AddRange(PegASTVector values) {
    PegParserDLLPINVOKE.PegASTVector_AddRange(swigCPtr, PegASTVector.getCPtr(values));
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  public PegASTVector GetRange(int index, int count) {
    global::System.IntPtr cPtr = PegParserDLLPINVOKE.PegASTVector_GetRange(swigCPtr, index, count);
    PegASTVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new PegASTVector(cPtr, true);
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Insert(int index, PegAST x) {
    PegParserDLLPINVOKE.PegASTVector_Insert(swigCPtr, index, PegAST.getCPtr(x));
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  public void InsertRange(int index, PegASTVector values) {
    PegParserDLLPINVOKE.PegASTVector_InsertRange(swigCPtr, index, PegASTVector.getCPtr(values));
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveAt(int index) {
    PegParserDLLPINVOKE.PegASTVector_RemoveAt(swigCPtr, index);
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveRange(int index, int count) {
    PegParserDLLPINVOKE.PegASTVector_RemoveRange(swigCPtr, index, count);
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  public static PegASTVector Repeat(PegAST value, int count) {
    global::System.IntPtr cPtr = PegParserDLLPINVOKE.PegASTVector_Repeat(PegAST.getCPtr(value), count);
    PegASTVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new PegASTVector(cPtr, true);
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Reverse() {
    PegParserDLLPINVOKE.PegASTVector_Reverse__SWIG_0(swigCPtr);
  }

  public void Reverse(int index, int count) {
    PegParserDLLPINVOKE.PegASTVector_Reverse__SWIG_1(swigCPtr, index, count);
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetRange(int index, PegASTVector values) {
    PegParserDLLPINVOKE.PegASTVector_SetRange(swigCPtr, index, PegASTVector.getCPtr(values));
    if (PegParserDLLPINVOKE.SWIGPendingException.Pending) throw PegParserDLLPINVOKE.SWIGPendingException.Retrieve();
  }

}
