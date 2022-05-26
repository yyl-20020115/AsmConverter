[Bits 64]
	align 64, db 0x90
	global __gmpn_gcd_11
	
	;.def	__gmpn_gcd_11
	;.scl	2
	;.type	32
	;.endef
__gmpn_gcd_11:

	push	rdi
	push	rsi
	mov	rdi, rcx
	mov	rsi, rdx

	jmp	Lodd

	align 16, db 0x90
Ltop:
	;BAD 	cmovc	%rdx, %rdi
	;BAD 	cmovc	%rax, %rsi		
	shr	rdi, cl
Lodd:
	mov	rdx, rsi
	sub	rdx, rdi
	bsf	rcx, rdx
	mov	rax, rdi
	sub	rdi, rsi
	jnz	Ltop

Lend:	
	
	pop	rsi
	pop	rdi
	ret
	
