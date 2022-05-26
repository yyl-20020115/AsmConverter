	section .text
	align 32, db 0x90
	global __gmpn_andn_n
	
	;.def	__gmpn_andn_n
	;.scl	2
	;.type	32
	;.endef
__gmpn_andn_n:

	push	rdi
	push	rsi
	mov	rdi, rcx
	mov	rsi, rdx
	mov	rdx, r8
	mov	rcx, r9

	mov	r8, [rdx]
	not	r8
	mov	eax, ecx
	and	eax, 3
	je	Lb00
	cmp	eax, 2
	jc	Lb01
	je	Lb10

Lb11:
	and	r8, [rsi]
	mov	[rdi], r8
	inc	rcx
	lea	rsi, [rsi - 8]
	lea	rdx, [rdx - 8]
	lea	rdi, [rdi - 8]
	jmp	Le11
Lb10:
	add	rcx, 2
	lea	rsi, [rsi - 16]
	lea	rdx, [rdx - 16]
	lea	rdi, [rdi - 16]
	jmp	Le10
Lb01:
	and	r8, [rsi]
	mov	[rdi], r8
	dec	rcx
	jz	Lret
	lea	rsi, [rsi + 8]
	lea	rdx, [rdx + 8]
	lea	rdi, [rdi + 8]

	align 16, db 0x90
Ltop:
	mov	r8, [rdx]
	not	r8
Lb00:
	mov	r9, [rdx + 8]
	not	r9
	and	r8, [rsi]
	and	r9, [rsi + 8]
	mov	[rdi], r8
	mov	[rdi + 8], r9
Le11:
	mov	r8, [rdx + 16]
	not	r8
Le10:
	mov	r9, [rdx + 24]
	not	r9
	lea	rdx, [rdx + 32]
	and	r8, [rsi + 16]
	and	r9, [rsi + 24]
	lea	rsi, [rsi + 32]
	mov	[rdi + 16], r8
	mov	[rdi + 24], r9
	lea	rdi, [rdi + 32]
	sub	rcx, 4
	jnz	Ltop

Lret:
	pop	rsi
	pop	rdi
	ret
	



