1:        
        PREFETCHNTA                 ;
        MOV                         ;
        MOV                         ;
        PXOR                        ;
        PCMPEQB                     ;
7:        
        MOV                         ;
        SHR                         ;
        .align       56, 0x90
11:        
        PREFETCHNTA                 ;
        PREFETCHNTA                 ;
        MOVD                        ;
        MOVD                        ;
        MOV                         ;
        MOV                         ;
        MOVQ                        ;
        MOVQ                        ;
        PSRLQ                       ;
        PSRLQ                       ;
        PUNPCKLWD                   ;
        PUNPCKLWD                   ;
        PUNPCKLDQ                   ;
        PUNPCKLDQ                   ;
        PCMPEQB                     ;
        PCMPEQB                     ;
        PUNPCKLBW                   ;
        PUNPCKLBW                   ;
        PUNPCKLBW                   ;
        PUNPCKLBW                   ;
        PSUBB                       ;
        PSUBB                       ;

        PMULLW                      ;
        PMULLW                      ;
        MOVD                        ;
        MOVD                        ;
        PUNPCKLBW                   ;
        PUNPCKLBW                   ;
        PMULLW                      ;
        PMULLW                      ;

        PCMPEQW                     ;
        PUNPCKLBW                   ;

        PADDW                       ;
        PADDW                       ;
        PSRLW                       ;
        PSRLW                       ;
        PAND                        ;
        PAND                        ;
        PACKUSWB                    ;
        PACKUSWB                    ;

        MOVD                        ;
        MOVD                        ;

        ADD                         ;
        ADD                         ;
        DEC                         ;
        JNZ                         ;

        MOV                         ;
        AND                         ;
        CMP                         ;
        JZ                          ;

// last single pixel
        MOVD                        ;
        MOV                         ;
        MOVQ                        ;
        PSRLQ                       ;
        PUNPCKLWD                   ;
        PUNPCKLDQ                   ;
        PCMPEQB                     ;
        PUNPCKLBW                   ;
        PUNPCKLBW                   ;
        PSUBB                       ;
        PMULLW                      ;
        MOVD                        ;
        PUNPCKLBW                   ;
        PMULLW                      ;
        PCMPEQW                     ;
        PUNPCKLBW                   ;
        PADDW                       ;
        PSRLW                       ;
        PACKUSWB                    ;
        MOVD                        ;
        ADD                         ;
        ADD                         ;
        JMP
        JMP
94:        
        ADD                         ;
        ADD                         ;
        DECL                        ;
        PUSHW                       ;
        POPW                        ;
        JNZ                         ;
101:        
        NOP
        .BYTE        0x31,0x32
        MOVB
        MOVW
        MOVL
        MOV
        PUSH
        EMMS                        ;
