	.text
	.align	32, 0x90
	.globl	__gmpn_hamdist
	
	.def	__gmpn_hamdist
	.scl	2
	.type	32
	.endef
__gmpn_hamdist:

	push	rdi
	push	rsi
	mov	rdi, rcx
	mov	rsi, rdx
	mov	rdx, r8

	push	rbx
	push	rbp

	mov	r10, [rdi]
	xor	r10, [rsi]

	mov	%r8d, edx
	and	%r8d, 3

	xor	ecx, ecx
	.byte	0xf3,0x49,0x0f,0xb8,0xc2	

	lea	r9, [%rip + Ltab]

;BAD 	movslq	(%r9,%r8,4), %r8
	add	r8, r9
	jmp	*%r8


L3:
	mov	r10, [rdi + 8]
	mov	r11, [rdi + 16]
	xor	r10, [rsi + 8]
	xor	r11, [rsi + 16]
	xor	ebp, ebp
	sub	rdx, 4
;BAD 	jle	Lx3
	mov	r8, [rdi + 24]
	mov	r9, [rdi + 32]
	add	rdi, 24
	add	rsi, 24
	jmp	Le3

L0:
	mov	r9, [rdi + 8]
	xor	r9, [rsi + 8]
	mov	r10, [rdi + 16]
	mov	r11, [rdi + 24]
	xor	ebx, ebx
	xor	r10, [rsi + 16]
	xor	r11, [rsi + 24]
	add	rdi, 32
	add	rsi, 32
	sub	rdx, 4
;BAD 	jle	Lx4

	.align	16, 0x90
Ltop:
Le0:
	.byte	0xf3,0x49,0x0f,0xb8,0xe9
	mov	r8, [rdi]
	mov	r9, [rdi + 8]
	add	rax, rbx
Le3:
	.byte	0xf3,0x49,0x0f,0xb8,0xda
	xor	r8, [rsi]
	xor	r9, [rsi + 8]
	add	rcx, rbp
Le2:
	.byte	0xf3,0x49,0x0f,0xb8,0xeb
	mov	r10, [rdi + 16]
	mov	r11, [rdi + 24]
	add	rdi, 32
	add	rax, rbx
Le1:
	.byte	0xf3,0x49,0x0f,0xb8,0xd8
	xor	r10, [rsi + 16]
	xor	r11, [rsi + 24]
	add	rsi, 32
	add	rcx, rbp
	sub	rdx, 4
	jg	Ltop

Lx4:
	.byte	0xf3,0x49,0x0f,0xb8,0xe9
	add	rax, rbx
Lx3:
	.byte	0xf3,0x49,0x0f,0xb8,0xda
	add	rcx, rbp
	.byte	0xf3,0x49,0x0f,0xb8,0xeb	
	add	rax, rbx
	add	rcx, rbp
Lx2:
	add	rax, rcx
Lx1:
	pop	rbp
	pop	rbx
	pop	rsi
	pop	rdi
	ret

L2:
	mov	r11, [rdi + 8]
	xor	r11, [rsi + 8]
	sub	rdx, 2
;BAD 	jle	Ln2
	mov	r8, [rdi + 16]
	mov	r9, [rdi + 24]
	xor	ebx, ebx
	xor	r8, [rsi + 16]
	xor	r9, [rsi + 24]
	add	rdi, 16
	add	rsi, 16
	jmp	Le2
Ln2:
	.byte	0xf3,0x49,0x0f,0xb8,0xcb
	jmp	Lx2

L1:
	dec	rdx
;BAD 	jle	Lx1
	mov	r8, [rdi + 8]
	mov	r9, [rdi + 16]
	xor	r8, [rsi + 8]
	xor	r9, [rsi + 16]
	xor	ebp, ebp
	mov	r10, [rdi + 24]
	mov	r11, [rdi + 32]
	add	rdi, 40
	add	rsi, 8
	jmp	Le1

	
		.section .rdata,"dr"
	.align	8, 0x90
Ltab:
	.long	L0-Ltab
	.long	L1-Ltab
	.long	L2-Ltab
	.long	L3-Ltab
