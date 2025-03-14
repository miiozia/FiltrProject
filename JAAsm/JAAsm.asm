; Temat projektu: Filtr monochromatyczny
; Algorytm sumuje wartoœci kana³ów (RGB) dla ka¿dego piksela, a nastêpnie dieli uzyskan¹ sumê przez 3.
; Tabela zapisanych, uzyskanych w ten sposób wartoœci stanowi podstawê do wygenerowania grafiki z na³o¿onym filtrem.
; Data wykonania projektu: semestr V
; Rok akademicki 2023/2024, 
; Autor: Marta Miozga 
; Kierunek:  Informatyka Katowice AEiI
; Wersja: 1.0

.data
	;double word do wprowadzenia liczby 3 do podzialu
	stable dword 3.0

.code
GrayOUT proc
	;obliczanie wielkosci tablicy
	mov rsi,[rsp+8*5]
	sub rsi,r9
	imul r9, 4

	xor r13,r13
	;wyzerowanie licznika

	;przesuniecie wskaznikow na poczatek tablicy
	add rcx, r9
	add rdx, r9
	add r8, r9

	;zaladowanie dzielnika do xmm4
	movss xmm4, [stable]
addLoop:
    ;warunek konca petli - jezeli rozmiar taki sam jak licznik - koniec
	cmp r13, rsi
	jge save
	
	;przekazanie wartosci do xmm
	movss xmm0, dword ptr [rcx + r13*4]
    movss xmm1, dword ptr [rdx + r13*4]  
	movss xmm2, dword ptr [r8 + r13*4]  

	;dodawanie do siebie kanalow r,g,b
	addps xmm0, xmm1
	addps xmm0, xmm2

	;podzielenie wyniku przez 3
	divps xmm0, xmm4

	;zapisanie wyniku do tablicy r
	movss dword ptr [rcx + r13*4], xmm0

	;inkrementacja licznika i skok do poczatku petli
	inc r13
	jmp addLoop
save:
	ret
GrayOUT endp
end