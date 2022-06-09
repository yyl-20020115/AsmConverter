    cld
    mov  %%0,%esi
    mov  %%1,%edi
    mov  %%2,%ecx
5:    
    mov  (%esi,,1),%al
    inc  %al                ;
    mov  %al,(%edi,,1)
    inc  %esi
    inc  %edi
    dec  %ecx
    jnz  5b                 ;
    ret
