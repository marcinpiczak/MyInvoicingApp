# MyInvoicingApp

MyInvoicingApp jest prostą aplikacją pozwalającą na rejestrowanie faktur dla konkretnego kontrahenta i przypisywanie jej do utworzonego wcześniej budżetu w ramach środków na nim się znajdujących.

## Instalacja:

W celu uruchomienia aplikacji należy stworzyć nową bazę danych lub odtworzyć bazę z przykładowymi danymi.

### I. Nowa baza danych 

1. utworzenie nowej bazy danych np. MyInvoicingAppDb korzystając ze skryptu ```create database MyInvoicingAppDb```
1. podmiana bazy w pliku konfiguracyjnym `appsettings.xml` jeżeli jest to wymagane
1. wykonenie `Update-Database` w VisualStudio z poziomu **Package Manager Console**
1. dodanie przykładowych danych po przez wykonanie skryptu **\scripts\Sample_data.sql** 

### II. SampleDB

1. odworzonie bazy z backupu znajdującego się w katalogu **SampleDB**

## Przykładowe dane:

Przykładowe dane zawierają:
1. cztery role (w tym trzy systemowe): 
    * `Accountant` (Księgowy) - rola systemowa
    * `Manager` (Menadżer) - rola systemowa
    * `Admin` (Administrator) - rola systemowa
    * `Temporary` (Tymczasowy)
1. trzech użytkowników (z hasłami: _Qazwsx1@_):
    * `marcin` przypisany do ról: Accountant, Manager, Admin - może się zalogować do aplikacji i posiada dostęp do wszystkich modułów
    * `tomek` przypisany do ról: Accountant, Manager - może się zalogować do aplikacji i posiada dostęp do wszystkich modułów poza modułem administracyjnym
    * `kasia` nie przypisany do żadnej roli - może się zalogować do aplikacji ale nie posiada dostępu do żadnego z modułów
1. zestaw przykładowych:
    * `klientów`
    * `budżetów`
    * `faktur`
    
## Dostępne moduły:

#### Funkcjonalności dostępne we wszystkich modułach
* Eksport danych z tabel z danymi do PDF i Excel
* Sortowanie po dostępnych kolumnach tabel
* Filtrowanie po zawartości dostępnych kolumn tabel

### Budżety

Moduł pozwala na tworzenie, modyfikowanie, zamykanie, otwieranie i przeglądanie stworzonych budżetów. W szczególach budżetów znajduje się także lista z fakturami przypisanymi do danego budżetu.

#### Dostępne funkcjonalności
* Tworzenie nowego budżetu
* Modyfikowanie istniejącego budżetu
* Zamykanie otwartego budżetu
* Otwieranie zamkniętego budżetu
* Przeglądanie szczegółów budżetu wraz z fakturami przypisanymi do danego budżetu

### Klienci

Moduł pozwala na tworzenie, modyfikowanie, zamykanie, otwieranie i przeglądanie kartotek klientów. W szczegółach klienta znajduje się także lista z fakturami stworzonymi dla danego klienta.

#### Dostępne funkcjonalności
* Tworzenie nowego klienta
* Modyfikowanie istniejącego klienta
* Zamykanie otwartego klienta
* Otwieranie zamkniętego klienta
* Przeglądanie szczegółów klienta wraz z fakturami stworzonymi do danego budżetu

### Faktury

Moduł pozwala na tworzenie, modyfikowanie, anulowanie i przeglądanie faktur. Każda z faktur jest tworzona dla konkretnego klienta. Linie faktur są natomiast przypisane do określonego budżetu.

#### Dostępne funkcjonalności
* Tworzenie nowego faktury
* Modyfikowanie istniejącego faktury
* Dodanie nowej linii do faktury w trakcie jej edycji
* Anulowanie faktury
* Anulowanie linii faktury
* Przeglądanie szczegółów faktury
* Eksport faktury do PDF i Excel
* Dodawanie załaczników do faktur

### Administracja

Moduł pozwala na tworzenie, modyfikowanie, zamykanie i otwieranie ról/stanowisk oraz przypisywanie użytkowników do istniejących ról/stanowisk.

#### Dostępne funkcjonalności
* Tworzenie nowej roli/stanowiska
* Modyfikowanie istniejącej roli/stanowiska
* Zamykanie otwartej roli/stanowiska o ile nie jest to rola systemowa: `Accountant`, `Manager`, `Admin`
* Otwarcie zamkniętej roli/stanowiska
* Przypisanie użytkownika do roli/stanowiska
* Usunięcie użytkownika z roli/stanowiska

## Korzystanie z aplikacji:

Do skorzystania z aplikacji wymagane jest zalogowanie się użytkownika przypisanego przynajmniej do roli: `Accountant` lub `Manager` pozwalających na dostęp do modułów: `Budżety`, `Klienci`, `Faktury`. 
Rola `Admin` pozwala dodatkowo na dostęp do modułu administracyjnego umożliwiającego zarządzanie rolami/stanowiskami oraz przypisywanie do nich użytkowników.

##### cdn.
