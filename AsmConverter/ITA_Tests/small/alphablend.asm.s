1:        
        prefetchnta  0x80(%esi,,1)          ;
        mov          %%2,%edi               ;
        mov          %%3,%esi               ;
        pxor         %mm7,%mm7              ;
        pcmpeqb      %mm6,%mm6              ;
7:        
        mov          %%1,%ecx               ;
        shr          $1,%ecx                ;
        .align       8, 0x90
11:        
        prefetchnta  0x80(%esi,,1)          ;
        prefetchnta  0x80(%edi,,1)          ;
        movd         (%esi,,1),%mm0         ;
        movd         0x4(%esi,,1),%mm1      ;
        mov          (%edi,,1),%eax         ;
        mov          0x4(%edi,,1),%ebx      ;
        movq         %mm0,%mm2              ;
        movq         %mm1,%mm3              ;
        psrlq        $18,%mm2               ;
        psrlq        $18,%mm3               ;
        punpcklwd    %mm2,%mm2              ;
        punpcklwd    %mm3,%mm3              ;
        punpckldq    %mm2,%mm2              ;
        punpckldq    %mm3,%mm3              ;
        pcmpeqb      %mm4,%mm4              ;
        pcmpeqb      %mm5,%mm5              ;
        punpcklbw    %mm7,%mm0              ;
        punpcklbw    %mm7,%mm1              ;
        punpcklbw    %mm7,%mm4              ;
        punpcklbw    %mm7,%mm5              ;
        psubb        %mm2,%mm4              ;
        psubb        %mm3,%mm5              ;

        pmullw       %mm2,%mm0              ;
        pmullw       %mm3,%mm1              ;
        movd         %eax,%mm2              ;
        movd         %ebx,%mm3              ;
        punpcklbw    %mm7,%mm2              ;
        punpcklbw    %mm7,%mm3              ;
        pmullw       %mm4,%mm2              ;
        pmullw       %mm5,%mm3              ;

        pcmpeqw      %mm5,%mm5              ;
        punpcklbw    %mm7,%mm5              ;

        paddw        %mm2,%mm0              ;
        paddw        %mm3,%mm1              ;
        psrlw        $8,%mm0                ;
        psrlw        $8,%mm1                ;
        pand         %mm5,%mm0              ;
        pand         %mm5,%mm1              ;
        packuswb     %mm0,%mm0              ;
        packuswb     %mm1,%mm1              ;

        movd         %mm0,(%edi,,1)         ;
        movd         %mm1,0x4(%edi,,1)      ;

        add          $8,%edi                ;
        add          $8,%esi                ;
        dec          %ecx                   ;
        jnz          11b                    ;

        mov          %%1,%ecx               ;
        and          $1,%ecx                ;
        cmp          $0,%ecx                ;
        jz           94f                    ;

// last single pixel
        movd         (%esi,,1),%mm0         ;
        mov          (%edi,,1),%eax         ;
        movq         %mm0,%mm2              ;
        psrlq        $18,%mm2               ;
        punpcklwd    %mm2,%mm2              ;
        punpckldq    %mm2,%mm2              ;
        pcmpeqb      %mm4,%mm4              ;
        punpcklbw    %mm7,%mm0              ;
        punpcklbw    %mm7,%mm4              ;
        psubb        %mm2,%mm4              ;
        pmullw       %mm2,%mm0              ;
        movd         %eax,%mm2              ;
        punpcklbw    %mm7,%mm2              ;
        pmullw       %mm4,%mm2              ;
        pcmpeqw      %mm5,%mm5              ;
        punpcklbw    %mm7,%mm5              ;
        paddw        %mm2,%mm0              ;
        psrlw        $8,%mm0                ;
        packuswb     %mm0,%mm0              ;
        movd         %mm0,(%edi,,1)         ;
        add          $4,%esi                ;
        add          $4,%edi                ;
        jmp          94f
        jmp          94f
94:        
        add          %%4,%edi               ;
        add          %%5,%esi               ;
        decl         %%0                    ;
        pushw        %%1                    ;
        popw         %%1                    ;
        jnz          7b                     ;
101:        
        nop
        .byte        0x1,0x2
        movb         $0,(%edi,,1)
        movw         $0,(%edi,,1)
        movl         $0,(%edi,,1)
        mov          (%edi,,1),%eax
        push         %ax
        emms                                ;
