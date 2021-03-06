[Bits 64]
	section .text
	align 64, db 0x90
	global __gmpn_copyi
	
	;.def	__gmpn_copyi
	;.scl	2
	;.type	32
	;.endef
__gmpn_copyi:

	push	rdi
	push	rsi
	mov	rdi, rcx
	mov	rsi, rdx
	mov	rdx, r8


	cmp	rdx, 7
	jbe	Lbc

	test	dil, 8
	jz	Lrp_aligned

	movsq
	dec	rdx

Lrp_aligned:
	test	sil, 8
	jnz	Luent

	jmp	Lam

	align 16, db 0x90
Latop:
movdqa	xmm0, [rsi + 0]
	movdqa	xmm1, [rsi + 16]
	movdqa	xmm2, [rsi + 32]
	movdqa	xmm3, [rsi + 48]
	lea	rsi, [rsi + 64]
	movdqa	[rdi], xmm0
	movdqa	[rdi + 16], xmm1
	movdqa	[rdi + 32], xmm2
	movdqa	[rdi + 48], xmm3
	lea	rdi, [rdi + 64]
Lam:
	sub	rdx, 8
	jnc	Latop

	test	dl, 4
	jz	_1
	movdqa	xmm0, [rsi]
	movdqa	xmm1, [rsi + 16]
	lea	rsi, [rsi + 32]
	movdqa	[rdi], xmm0
	movdqa	[rdi + 16], xmm1
	lea	rdi, [rdi + 32]

_1:
	test	dl, 2
	jz	_2
	movdqa	xmm0, [rsi]
	lea	rsi, [rsi + 16]
	movdqa	[rdi], xmm0
	lea	rdi, [rdi + 16]

_2:
	test	dl, 1
	jz	_3
	mov	r8, [rsi]
	mov	[rdi], r8

_3:
	pop	rsi
	pop	rdi
	ret

Luent:


	cmp	rdx, 16
	jc	Lued0

	add	rsp, -56
	movdqa	[rsp], xmm6
	movdqa	[rsp + 16], xmm7
	movdqa	[rsp + 32], xmm8

	movaps	xmm7, [rsi + 120]
	movaps	xmm6, [rsi + 104]
	movaps	xmm5, [rsi + 88]
	movaps	xmm4, [rsi + 72]
	movaps	xmm3, [rsi + 56]
	movaps	xmm2, [rsi + 40]
	lea	rsi, [rsi + 128]
	sub	rdx, 32
	jc	Lued1

	align 16, db 0x90
Lutop:
movaps	xmm1, [rsi - 104]
	sub	rdx, 16
	movaps	xmm0, [rsi - 120]
	db 0x66,0x0f,0x3a,0x0f,254,8
	movaps	xmm8, [rsi - 136]
	movdqa	[rdi + 112], xmm7
	db 0x66,0x0f,0x3a,0x0f,245,8
	movaps	xmm7, [rsi + 120]
	movdqa	[rdi + 96], xmm6
	db 0x66,0x0f,0x3a,0x0f,236,8
	movaps	xmm6, [rsi + 104]
	movdqa	[rdi + 80], xmm5
	db 0x66,0x0f,0x3a,0x0f,227,8
	movaps	xmm5, [rsi + 88]
	movdqa	[rdi + 64], xmm4
	db 0x66,0x0f,0x3a,0x0f,218,8
	movaps	xmm4, [rsi + 72]
	movdqa	[rdi + 48], xmm3
	db 0x66,0x0f,0x3a,0x0f,209,8
	movaps	xmm3, [rsi + 56]
	movdqa	[rdi + 32], xmm2
	db 0x66,0x0f,0x3a,0x0f,200,8
	movaps	xmm2, [rsi + 40]
	movdqa	[rdi + 16], xmm1
	db 0x66,65,0x0f,0x3a,0x0f,192,8
	lea	rsi, [rsi + 128]
	movdqa	[rdi], xmm0
	lea	rdi, [rdi + 128]
	jnc	Lutop

Lued1:
movaps	xmm1, [rsi - 104]
	movaps	xmm0, [rsi - 120]
	movaps	xmm8, [rsi - 136]
	db 0x66,0x0f,0x3a,0x0f,254,8
	movdqa	[rdi + 112], xmm7
	db 0x66,0x0f,0x3a,0x0f,245,8
	movdqa	[rdi + 96], xmm6
	db 0x66,0x0f,0x3a,0x0f,236,8
	movdqa	[rdi + 80], xmm5
	db 0x66,0x0f,0x3a,0x0f,227,8
	movdqa	[rdi + 64], xmm4
	db 0x66,0x0f,0x3a,0x0f,218,8
	movdqa	[rdi + 48], xmm3
	db 0x66,0x0f,0x3a,0x0f,209,8
	movdqa	[rdi + 32], xmm2
	db 0x66,0x0f,0x3a,0x0f,200,8
	movdqa	[rdi + 16], xmm1
	db 0x66,65,0x0f,0x3a,0x0f,192,8
	movdqa	[rdi], xmm0
	lea	rdi, [rdi + 128]

	movdqa	xmm6, [rsp]
	movdqa	xmm7, [rsp + 16]
	movdqa	xmm8, [rsp + 32]
	add	rsp, 56

Lued0:
test	dl, 8
	jz	_4
	movaps	xmm3, [rsi + 56]
	movaps	xmm2, [rsi + 40]
	movaps	xmm1, [rsi + 24]
	movaps	xmm0, [rsi + 8]
	movaps	xmm4, [rsi - 8]
	db 0x66,0x0f,0x3a,0x0f,218,8
	movdqa	[rdi + 48], xmm3
	db 0x66,0x0f,0x3a,0x0f,209,8
	movdqa	[rdi + 32], xmm2
	db 0x66,0x0f,0x3a,0x0f,200,8
	movdqa	[rdi + 16], xmm1
	db 0x66,0x0f,0x3a,0x0f,196,8
	lea	rsi, [rsi + 64]
	movdqa	[rdi], xmm0
	lea	rdi, [rdi + 64]

_4:
	test	dl, 4
	jz	_5
	movaps	xmm1, [rsi + 24]
	movaps	xmm0, [rsi + 8]
	db 0x66,0x0f,0x3a,0x0f,200,8
	movaps	xmm3, [rsi - 8]
	movdqa	[rdi + 16], xmm1
	db 0x66,0x0f,0x3a,0x0f,195,8
	lea	rsi, [rsi + 32]
	movdqa	[rdi], xmm0
	lea	rdi, [rdi + 32]

_5:
	test	dl, 2
	jz	_6
	movdqa	xmm0, [rsi + 8]
	movdqa	xmm3, [rsi - 8]
	db 0x66,0x0f,0x3a,0x0f,195,8
	lea	rsi, [rsi + 16]
	movdqa	[rdi], xmm0
	lea	rdi, [rdi + 16]

_6:
	test	dl, 1
	jz	_7
	mov	r8, [rsi]
	mov	[rdi], r8

_7:
	pop	rsi
	pop	rdi
	ret




Lbc:
	lea	rdi, [rdi - 8]
	sub	edx, 4
	jc	Lend

	align 16, db 0x90
Ltop:
	mov	r8, [rsi]
	mov	r9, [rsi + 8]
	lea	rdi, [rdi + 32]
	mov	r10, [rsi + 16]
	mov	r11, [rsi + 24]
	lea	rsi, [rsi + 32]
	mov	[rdi - 24], r8
	mov	[rdi - 16], r9

	mov	[rdi - 8], r10
	mov	[rdi], r11


Lend:
	test	dl, 1
	jz	_8
	mov	r8, [rsi]
	mov	[rdi + 8], r8
	lea	rdi, [rdi + 8]
	lea	rsi, [rsi + 8]
_8:
	test	dl, 2
	jz	_9
	mov	r8, [rsi]
	mov	r9, [rsi + 8]
	mov	[rdi + 8], r8
	mov	[rdi + 16], r9
_9:
	pop	rsi
	pop	rdi
	ret
	

