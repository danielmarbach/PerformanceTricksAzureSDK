## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ArrayForeachVsFor.Foreach()
       push      rbp
       mov       rbp,rsp
       mov       rax,[rdi+8]
       xor       esi,esi
       mov       edx,[rax+8]
       test      edx,edx
       jle       short M00_L01
       mov       rdi,[rdi+10]
       nop       word ptr [rax+rax]
M00_L00:
       movsxd    rcx,esi
       mov       ecx,[rax+rcx*4+10]
       mov       r8,rdi
       mov       [r8+40],ecx
       inc       esi
       cmp       edx,esi
       jg        short M00_L00
M00_L01:
       pop       rbp
       ret
; Total bytes of code 54
```

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ArrayForeachVsFor.For()
       push      rbp
       mov       rbp,rsp
       xor       eax,eax
       mov       rsi,[rdi+8]
       cmp       dword ptr [rsi+8],0
       jle       short M00_L01
       mov       rdx,[rdi+10]
M00_L00:
       mov       rcx,rdx
       mov       r8,rsi
       cmp       eax,[r8+8]
       jae       short M00_L02
       movsxd    r9,eax
       mov       r8d,[r8+r9*4+10]
       mov       [rcx+40],r8d
       inc       eax
       mov       rcx,[rdi+8]
       cmp       [rcx+8],eax
       jg        short M00_L00
M00_L01:
       pop       rbp
       ret
M00_L02:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 63
```

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ArrayForeachVsFor.Foreach()
       push      rbp
       mov       rbp,rsp
       mov       rax,[rdi+8]
       xor       esi,esi
       mov       edx,[rax+8]
       test      edx,edx
       jle       short M00_L01
       mov       rdi,[rdi+10]
       nop       word ptr [rax+rax]
M00_L00:
       movsxd    rcx,esi
       mov       ecx,[rax+rcx*4+10]
       mov       r8,rdi
       mov       [r8+40],ecx
       inc       esi
       cmp       edx,esi
       jg        short M00_L00
M00_L01:
       pop       rbp
       ret
; Total bytes of code 54
```

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ArrayForeachVsFor.For()
       push      rbp
       mov       rbp,rsp
       xor       eax,eax
       mov       rsi,[rdi+8]
       cmp       dword ptr [rsi+8],0
       jle       short M00_L01
       mov       rdx,[rdi+10]
M00_L00:
       mov       rcx,rdx
       mov       r8,rsi
       cmp       eax,[r8+8]
       jae       short M00_L02
       movsxd    r9,eax
       mov       r8d,[r8+r9*4+10]
       mov       [rcx+40],r8d
       inc       eax
       mov       rcx,[rdi+8]
       cmp       [rcx+8],eax
       jg        short M00_L00
M00_L01:
       pop       rbp
       ret
M00_L02:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 63
```

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ArrayForeachVsFor.Foreach()
       push      rbp
       mov       rbp,rsp
       mov       rax,[rdi+8]
       xor       esi,esi
       mov       edx,[rax+8]
       test      edx,edx
       jle       short M00_L01
       mov       rdi,[rdi+10]
       nop       word ptr [rax+rax]
M00_L00:
       movsxd    rcx,esi
       mov       ecx,[rax+rcx*4+10]
       mov       r8,rdi
       mov       [r8+40],ecx
       inc       esi
       cmp       edx,esi
       jg        short M00_L00
M00_L01:
       pop       rbp
       ret
; Total bytes of code 54
```

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ArrayForeachVsFor.For()
       push      rbp
       mov       rbp,rsp
       xor       eax,eax
       mov       rsi,[rdi+8]
       cmp       dword ptr [rsi+8],0
       jle       short M00_L01
       mov       rdx,[rdi+10]
M00_L00:
       mov       rcx,rdx
       mov       r8,rsi
       cmp       eax,[r8+8]
       jae       short M00_L02
       movsxd    r9,eax
       mov       r8d,[r8+r9*4+10]
       mov       [rcx+40],r8d
       inc       eax
       mov       rcx,[rdi+8]
       cmp       [rcx+8],eax
       jg        short M00_L00
M00_L01:
       pop       rbp
       ret
M00_L02:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 63
```

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ArrayForeachVsFor.Foreach()
       push      rbp
       mov       rbp,rsp
       mov       rax,[rdi+8]
       xor       esi,esi
       mov       edx,[rax+8]
       test      edx,edx
       jle       short M00_L01
       mov       rdi,[rdi+10]
       nop       word ptr [rax+rax]
M00_L00:
       movsxd    rcx,esi
       mov       ecx,[rax+rcx*4+10]
       mov       r8,rdi
       mov       [r8+40],ecx
       inc       esi
       cmp       edx,esi
       jg        short M00_L00
M00_L01:
       pop       rbp
       ret
; Total bytes of code 54
```

## .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
```assembly
; ArrayForeachVsFor.For()
       push      rbp
       mov       rbp,rsp
       xor       eax,eax
       mov       rsi,[rdi+8]
       cmp       dword ptr [rsi+8],0
       jle       short M00_L01
       mov       rdx,[rdi+10]
M00_L00:
       mov       rcx,rdx
       mov       r8,rsi
       cmp       eax,[r8+8]
       jae       short M00_L02
       movsxd    r9,eax
       mov       r8d,[r8+r9*4+10]
       mov       [rcx+40],r8d
       inc       eax
       mov       rcx,[rdi+8]
       cmp       [rcx+8],eax
       jg        short M00_L00
M00_L01:
       pop       rbp
       ret
M00_L02:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 63
```

