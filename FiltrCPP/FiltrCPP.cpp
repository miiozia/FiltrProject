/*
Temat projektu : Filtr monochromatyczny
Algorytm sumuje wartoœci RGB kolejnych pikseli i dzieli je przez 3.
Tabela zapisanych, uzyskanych w ten sposób wartoœci stanowi podstawê do wygenerowania grafiki z na³o¿onym filtrem.
Data wykonania projektu : semestr V
Rok akademicki 2023 / 2024,
Autor: Marta Miozga
Kierunek:  Informatyka Katowice AEiI
Wersja: 1.0
*/
#include <iostream>
#include "pch.h"

void convertToMonochrome(int* redArray, int* greenArray, int* blueArray, int start, int end) {
    while (start<end) {
        redArray[start] = (redArray[start] + greenArray[start] + blueArray[start]) / 3;
        start++;
    }
}

extern "C" __declspec(dllexport) void convertInCpp(int* redArray, int* greenArray, int* blueArray, int start, int end) {
   convertToMonochrome(redArray, greenArray, blueArray, start, end);
}



