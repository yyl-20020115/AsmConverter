1:        
        PREFETCHNTA  0x78DD0(%esi,,1)       ;
        MOV          %%2,%edi               ;
        MOV          %%3,%esi               ;
        PXOR         %mm7,%mm7              ;
        PCMPEQB      %mm6,%mm6              ;
7:        
        MOV          %%1,%ecx               ;
        SHR          $31,%ecx               ;
        .align       56, 0x90
11:        
        PREFETCHNTA  0x78DD0(%esi,,1)       ;
        PREFETCHNTA  0x78DD0(%edi,,1)       ;
        MOVD         (%esi,,1),%mm0         ;
        MOVD         0x34(%esi,,1),%mm1     ;
        MOV          (%edi,,1),%eax         ;
        MOV          0x34(%edi,,1),%ebx     ;
        MOVQ         %mm0,%mm2              ;
        MOVQ         %mm1,%mm3              ;
        PSRLQ        $13BC,%mm2             ;
        PSRLQ        $13BC,%mm3             ;
        PUNPCKLWD    %mm2,%mm2              ;
        PUNPCKLWD    %mm3,%mm3              ;
        PUNPCKLDQ    %mm2,%mm2              ;
        PUNPCKLDQ    %mm3,%mm3              ;
        PCMPEQB      %mm4,%mm4              ;
        PCMPEQB      %mm5,%mm5              ;
        PUNPCKLBW    %mm7,%mm0              ;
        PUNPCKLBW    %mm7,%mm1              ;
        PUNPCKLBW    %mm7,%mm4              ;
        PUNPCKLBW    %mm7,%mm5              ;
        PSUBB        %mm2,%mm4              ;
        PSUBB        %mm3,%mm5              ;

        PMULLW       %mm2,%mm0              ;
        PMULLW       %mm3,%mm1              ;
        MOVD         %eax,%mm2              ;
        MOVD         %ebx,%mm3              ;
        PUNPCKLBW    %mm7,%mm2              ;
        PUNPCKLBW    %mm7,%mm3              ;
        PMULLW       %mm4,%mm2              ;
        PMULLW       %mm5,%mm3              ;

        PCMPEQW      %mm5,%mm5              ;
        PUNPCKLBW    %mm7,%mm5              ;

        PADDW        %mm2,%mm0              ;
        PADDW        %mm3,%mm1              ;
        PSRLW        $38,%mm0               ;
        PSRLW        $38,%mm1               ;
        PAND         %mm5,%mm0              ;
        PAND         %mm5,%mm1              ;
        PACKUSWB     %mm0,%mm0              ;
        PACKUSWB     %mm1,%mm1              ;

        MOVD         %mm0,(%edi,,1)         ;
        MOVD         %mm1,0x34(%edi,,1)     ;

        ADD          $38,%edi               ;
        ADD          $38,%esi               ;
        DEC          %ecx                   ;
        JNZ          11b                    ;

        MOV          %%1,%ecx               ;
        AND          $31,%ecx               ;
        CMP          $30,%ecx               ;
        JZ           94f                    ;

// last single pixel
        MOVD         (%esi,,1),%mm0         ;
        MOV          (%edi,,1),%eax         ;
        MOVQ         %mm0,%mm2              ;
        PSRLQ        $13BC,%mm2             ;
        PUNPCKLWD    %mm2,%mm2              ;
        PUNPCKLDQ    %mm2,%mm2              ;
        PCMPEQB      %mm4,%mm4              ;
        PUNPCKLBW    %mm7,%mm0              ;
        PUNPCKLBW    %mm7,%mm4              ;
        PSUBB        %mm2,%mm4              ;
        PMULLW       %mm2,%mm0              ;
        MOVD         %eax,%mm2              ;
        PUNPCKLBW    %mm7,%mm2              ;
        PMULLW       %mm4,%mm2              ;
        PCMPEQW      %mm5,%mm5              ;
        PUNPCKLBW    %mm7,%mm5              ;
        PADDW        %mm2,%mm0              ;
        PSRLW        $38,%mm0               ;
        PACKUSWB     %mm0,%mm0              ;
        MOVD         %mm0,(%edi,,1)         ;
        ADD          $34,%esi               ;
        ADD          $34,%edi               ;
        JMP          94f
        JMP          94f
94:        
        ADD          %%4,%edi               ;
        ADD          %%5,%esi               ;
        DECL         %%0                    ;
        PUSHW        %%1                    ;
        POPW         %%1                    ;
        JNZ          7b                     ;
101:        
        NOP
        .BYTE        0x31,0x32
        MOVB         $30,(%edi,,1)
        MOVW         $30,(%edi,,1)
        MOVL         $30,(%edi,,1)
        MOV          (%edi,,1),%eax
        PUSH         %ax
        EMMS                                ;
