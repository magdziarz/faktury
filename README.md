# faktury
program obsługujący faktury
## Opis programu
  Program do obsługi zamówień.
* 1. Program na wejściu przyjmuje jako argument listę plików csv, xml i json
* 2. Każdy plik zawiera jedno lub więcej zamówień (format w załączniku)
* 3. Każde zamówienie należy zapisać w „bazie danych” (użyj bazy in memory)
*4. Zamówienie zawiera obowiązkowe pola:
 *a. ClientId – alfanumeryczne, bez spacji nie dłuższe niż 6 znaków
 *b. RequestId – numeryczne long
 *c. Name – alfanumeryczne ze spacjami nie dłuższe niż 255 znaków
 *d. Quantity – numeryczne int
 *e. Price – numeryczne zmiennoprzecinkowe podwójnej precyzji
*5. Program wyświetla menu z listą raportów oraz generuje wybrany raport:
 *a. Ilość zamówień
 *b. Ilość zamówień dla klienta o wskazanym identyfikatorze
 *c. Łączna kwota zamówień
 *d. Łączna kwota zamówień dla klienta o wskazanym identyfikatorze
 *e. Lista wszystkich zamówień
 *f. Lista zamówień dla klienta o wskazanym identyfikatorze
 *g. Średnia wartość zamówienia
 *h. Średnia wartość zamówienia dla klienta o wskazanym identyfikatorze
 *i. Ilość zamówień pogrupowanych po nazwie
 *j. Ilość zamówień pogrupowanych po nazwie dla klienta o wskazanym identyfikatorze
 *k. Zamówienia w podanym przedziale cenowym
*6. Możliwość ustalenia w dowolnym momencie sortowania dla dowolnego raportu w trakcie
działania programu
*7. Każdy raport można wyświetlić na ekranie, albo zapisać do pliku csv (format nie jest
narzucony)
*8. Baza danych nie jest dzielona pomiędzy uruchomieniami
*9. Nieprawidłowe dane w zamówieniu są ignorowane, ale informacja o złym formacie jest
wypisywana na ekran.
