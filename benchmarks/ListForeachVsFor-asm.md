## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ListForeachVsFor.Foreach()
       push      rbp
       mov       rbp,rsp
       mov       rax,[rdi+8]
       mov       esi,[rax+14]
       mov       edx,esi
       xor       ecx,ecx
       jmp       short M00_L01
M00_L00:
       mov       r9,[rdi+10]
       mov       [r9+40],r8d
M00_L01:
       cmp       edx,esi
       jne       short M00_L04
       cmp       ecx,[rax+10]
       jae       short M00_L02
       mov       r8,[rax+8]
       cmp       ecx,[r8+8]
       jae       short M00_L05
       movsxd    r9,ecx
       mov       r8d,[r8+r9*4+10]
       inc       ecx
       mov       r9d,1
       jmp       short M00_L03
M00_L02:
       mov       ecx,[rax+10]
       inc       ecx
       xor       r8d,r8d
       xor       r9d,r9d
M00_L03:
       test      r9d,r9d
       jne       short M00_L00
       pop       rbp
       ret
M00_L04:
       call      System.ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
       int       3
M00_L05:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 92
```
**Method was not JITted yet.**
System.ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ListForeachVsFor.For()
       push      rbp
       mov       rbp,rsp
       xor       eax,eax
       mov       rsi,[rdi+8]
       mov       rdx,rsi
       mov       ecx,[rdx+10]
       test      ecx,ecx
       jle       short M00_L01
       mov       r8,[rdi+10]
M00_L00:
       mov       rdx,r8
       mov       r9,rsi
       cmp       eax,ecx
       jae       short M00_L02
       mov       r9,[r9+8]
       cmp       eax,[r9+8]
       jae       short M00_L03
       movsxd    r10,eax
       mov       r9d,[r9+r10*4+10]
       mov       [rdx+40],r9d
       inc       eax
       mov       rdx,[rdi+8]
       cmp       eax,[rdx+10]
       jl        short M00_L00
M00_L01:
       pop       rbp
       ret
M00_L02:
       call      System.ThrowHelper.ThrowArgumentOutOfRange_IndexException()
       int       3
M00_L03:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 81
```
**Method was not JITted yet.**
System.ThrowHelper.ThrowArgumentOutOfRange_IndexException()

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ListForeachVsFor.Foreach()
       push      rbp
       mov       rbp,rsp
       mov       rax,[rdi+8]
       mov       esi,[rax+14]
       mov       edx,esi
       xor       ecx,ecx
       jmp       short M00_L01
M00_L00:
       mov       r9,[rdi+10]
       mov       [r9+40],r8d
M00_L01:
       cmp       edx,esi
       jne       short M00_L04
       cmp       ecx,[rax+10]
       jae       short M00_L02
       mov       r8,[rax+8]
       cmp       ecx,[r8+8]
       jae       short M00_L05
       movsxd    r9,ecx
       mov       r8d,[r8+r9*4+10]
       inc       ecx
       mov       r9d,1
       jmp       short M00_L03
M00_L02:
       mov       ecx,[rax+10]
       inc       ecx
       xor       r8d,r8d
       xor       r9d,r9d
M00_L03:
       test      r9d,r9d
       jne       short M00_L00
       pop       rbp
       ret
M00_L04:
       call      System.ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
       int       3
M00_L05:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 92
```
**Method was not JITted yet.**
System.ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ListForeachVsFor.For()
       push      rbp
       mov       rbp,rsp
       xor       eax,eax
       mov       rsi,[rdi+8]
       mov       rdx,rsi
       mov       ecx,[rdx+10]
       test      ecx,ecx
       jle       short M00_L01
       mov       r8,[rdi+10]
M00_L00:
       mov       rdx,r8
       mov       r9,rsi
       cmp       eax,ecx
       jae       short M00_L02
       mov       r9,[r9+8]
       cmp       eax,[r9+8]
       jae       short M00_L03
       movsxd    r10,eax
       mov       r9d,[r9+r10*4+10]
       mov       [rdx+40],r9d
       inc       eax
       mov       rdx,[rdi+8]
       cmp       eax,[rdx+10]
       jl        short M00_L00
M00_L01:
       pop       rbp
       ret
M00_L02:
       call      System.ThrowHelper.ThrowArgumentOutOfRange_IndexException()
       int       3
M00_L03:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 81
```
**Method was not JITted yet.**
System.ThrowHelper.ThrowArgumentOutOfRange_IndexException()

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ListForeachVsFor.Foreach()
       push      rbp
       mov       rbp,rsp
       mov       rax,[rdi+8]
       mov       esi,[rax+14]
       mov       edx,esi
       xor       ecx,ecx
       jmp       short M00_L01
M00_L00:
       mov       r9,[rdi+10]
       mov       [r9+40],r8d
M00_L01:
       cmp       edx,esi
       jne       short M00_L04
       cmp       ecx,[rax+10]
       jae       short M00_L02
       mov       r8,[rax+8]
       cmp       ecx,[r8+8]
       jae       short M00_L05
       movsxd    r9,ecx
       mov       r8d,[r8+r9*4+10]
       inc       ecx
       mov       r9d,1
       jmp       short M00_L03
M00_L02:
       mov       ecx,[rax+10]
       inc       ecx
       xor       r8d,r8d
       xor       r9d,r9d
M00_L03:
       test      r9d,r9d
       jne       short M00_L00
       pop       rbp
       ret
M00_L04:
       call      System.ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
       int       3
M00_L05:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 92
```
**Method was not JITted yet.**
System.ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ListForeachVsFor.For()
       push      rbp
       mov       rbp,rsp
       xor       eax,eax
       mov       rsi,[rdi+8]
       mov       rdx,rsi
       mov       ecx,[rdx+10]
       test      ecx,ecx
       jle       short M00_L01
       mov       r8,[rdi+10]
M00_L00:
       mov       rdx,r8
       mov       r9,rsi
       cmp       eax,ecx
       jae       short M00_L02
       mov       r9,[r9+8]
       cmp       eax,[r9+8]
       jae       short M00_L03
       movsxd    r10,eax
       mov       r9d,[r9+r10*4+10]
       mov       [rdx+40],r9d
       inc       eax
       mov       rdx,[rdi+8]
       cmp       eax,[rdx+10]
       jl        short M00_L00
M00_L01:
       pop       rbp
       ret
M00_L02:
       call      System.ThrowHelper.ThrowArgumentOutOfRange_IndexException()
       int       3
M00_L03:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 81
```
**Method was not JITted yet.**
System.ThrowHelper.ThrowArgumentOutOfRange_IndexException()

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ListForeachVsFor.Foreach()
       push      rbp
       mov       rbp,rsp
       mov       rax,[rdi+8]
       mov       esi,[rax+14]
       mov       edx,esi
       xor       ecx,ecx
       jmp       short M00_L01
M00_L00:
       mov       r9,[rdi+10]
       mov       [r9+40],r8d
M00_L01:
       cmp       edx,esi
       jne       short M00_L04
       cmp       ecx,[rax+10]
       jae       short M00_L02
       mov       r8,[rax+8]
       cmp       ecx,[r8+8]
       jae       short M00_L05
       movsxd    r9,ecx
       mov       r8d,[r8+r9*4+10]
       inc       ecx
       mov       r9d,1
       jmp       short M00_L03
M00_L02:
       mov       ecx,[rax+10]
       inc       ecx
       xor       r8d,r8d
       xor       r9d,r9d
M00_L03:
       test      r9d,r9d
       jne       short M00_L00
       pop       rbp
       ret
M00_L04:
       call      System.ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
       int       3
M00_L05:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 92
```
**Method was not JITted yet.**
System.ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ListForeachVsFor.For()
       push      rbp
       mov       rbp,rsp
       xor       eax,eax
       mov       rsi,[rdi+8]
       mov       rdx,rsi
       mov       ecx,[rdx+10]
       test      ecx,ecx
       jle       short M00_L01
       mov       r8,[rdi+10]
M00_L00:
       mov       rdx,r8
       mov       r9,rsi
       cmp       eax,ecx
       jae       short M00_L02
       mov       r9,[r9+8]
       cmp       eax,[r9+8]
       jae       short M00_L03
       movsxd    r10,eax
       mov       r9d,[r9+r10*4+10]
       mov       [rdx+40],r9d
       inc       eax
       mov       rdx,[rdi+8]
       cmp       eax,[rdx+10]
       jl        short M00_L00
M00_L01:
       pop       rbp
       ret
M00_L02:
       call      System.ThrowHelper.ThrowArgumentOutOfRange_IndexException()
       int       3
M00_L03:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 81
```
**Method was not JITted yet.**
System.ThrowHelper.ThrowArgumentOutOfRange_IndexException()

