    CLD
    MOV  %%0,%esi
    MOV  %%1,%edi
    MOV  %%2,%ecx
5:    
    MOV  (%esi,,1),%al
    INC  %al                ;
    MOV  %al,(%edi,,1)
    INC  %esi
    INC  %edi
    DEC  %ecx
    JNZ  5b                 ;
    RET
