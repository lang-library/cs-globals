from cffi import FFI
ffi = FFI()
ffi.cdef("const char *Call(const char *, const char *);")
#ffi.cdef("const char *LastError();")
clib = ffi.dlopen("nuget-tools.Globals0_Native.native.dll")

answer = ffi.string(clib.Call("add2".encode(), "[11, 22]".encode())).decode()
print("answer={}".format(answer))
#error = ffi.string(clib.LastError()).decode()
#print("error={}".format(error))

answer = ffi.string(clib.Call("add2".encode(), "[1, 2, 3]".encode())).decode()
print("answer={}".format(answer))
#error = ffi.string(clib.LastError()).decode()
#print("error={}".format(error))

answer = ffi.string(clib.Call("add2".encode(), "[]".encode())).decode()
print("answer={}".format(answer))
#error = ffi.string(clib.LastError()).decode()
#print("error={}".format(error))
