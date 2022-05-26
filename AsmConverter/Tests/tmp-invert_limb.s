	.text
	.align	16, 0x90
	.globl	__gmpn_invert_limb
	
	.def	__gmpn_invert_limb
	.scl	2
	.type	32
	.endef
__gmpn_invert_limb:
		
	push	%rdi
	push	%rsi
	mov	%rcx, %rdi

	mov	%rdi, %rax		
	shr	$55, %rax		
	
	.byte 0x4c,0x8d,0x05,0x00,0xfe,0xff,0xff
	;lea	r8, [rip - 512+__gmpn_invert_limb_table]
	
	;lea	-512+__gmpn_invert_limb_table(%rip), %r8
	movzwl	(%r8,%rax,2), %ecx	

	
	mov	%rdi, %rsi		
	mov	%ecx, %eax	
	imul	%ecx, %ecx	
	shr	$24, %rsi		
	inc	%rsi			
	imul	%rsi, %rcx		
	shr	$40, %rcx		
	sal	$11, %eax		
	dec	%eax
	sub	%ecx, %eax	

	
	mov	$0x1000000000000000, %rcx
	imul	%rax, %rsi		
	sub	%rsi, %rcx
	imul	%rax, %rcx
	sal	$13, %rax
	shr	$47, %rcx
	add	%rax, %rcx		

	
	mov	%rdi, %rsi		
	shr	$1,%rsi			
	sbb	%rax, %rax		
	sub	%rax, %rsi		
	imul	%rcx, %rsi		
	and	%rcx, %rax		
	shr	$1,%rax			
	sub	%rsi, %rax		
	mul	%rcx
	sal	$31, %rcx
	shr	$1,%rdx
	add	%rdx, %rcx		

	mov	%rdi, %rax
	mul	%rcx
	add	%rdi, %rax
	mov	%rcx, %rax
	adc	%rdi, %rdx
	sub	%rdx, %rax

	pop	%rsi
	pop	%rdi
	ret
	

