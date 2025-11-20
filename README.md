# Krojačnica

## Opis
SewItYourWay je aplikacija koja upravlja klijentima, zaposlenima, materijalima, artiklima, narudžbama i dobavljačima.  
Baza omogućava praćenje mjera, ponuda, narudžbi, računa, probanja, otkupa i isplata. Korištena je **MySQL baza podataka**.

Na repozitorijumu se nalazi i SQL skripta za kreiranje baze (`skripta za bazu.txt`) i skripta sa test podacima (`skripta za testne podatke.txt`).  
Projekat se povezuje sa bazom putem **Entity Framework-a**.

---

## Tabele

Glavne tabele i njihova svrha:

| Tabela | Svrha |
|--------|-------|
| `OSOBA` | Osnovni podaci o osobama (ime, prezime, kontakt). |
| `KLIJENT` | Klijenti koji naručuju proizvode i usluge. |
| `ZAPOSLENI` | Zaposleni u krojačnici sa korisničkim imenom i lozinkom. |
| `MJERE` | Mjere klijenata za individualne proizvode. |
| `PONUDA` | Ponude usluga sa cijenom. |
| `USLUGA` | Specifične usluge povezane sa ponudom. |
| `MATERIJAL` | Materijali koji se koriste za izradu proizvoda. |
| `BOJA` | Dostupne boje materijala ili artikala. |
| `ARTIKAL` | Proizvodi (haljine, odijela) sa materijalom, bojom i slikom. |
| `NARUDZBA` | Narudžbe klijenata, sa statusom i zaposlenim koji obrađuje. |
| `STAVKA_NARUDZBE` | Stavke unutar narudžbi, uključuju količinu, cijenu i mjere. |
| `RACUN` | Računi vezani za narudžbe. |
| `PROBA` | Evidencija probanja proizvoda sa komentarima. |
| `DOBAVLJAC` | Dobavljači materijala. |
| `OTKUP` | Evidencija otkupa materijala od dobavljača. |
| `MATERIJAL_DOBAVLJAC` | Veza između materijala i dobavljača sa cijenom. |
| `ISPLATA` | Evidencija isplata dobavljačima. |
| `ADMIN` | Zaposleni sa administratorskim privilegijama. |
| `MATERIJAL_ZALIHA` | Trenutna dostupna količina materijala po boji. |

> Dodatne informacije o šemi dostupne su u ER dijagramu baze.

---

## Sample podaci
- Svaka tabela sadrži po 2 test unosa radi lakšeg testiranja.  
- Sample podaci su konzistentni sa stranim ključevima, ali lozinke i vrijednosti su za testiranje i nisu sigurnosno hashirane.

---

## Instalacija

1. Otvorite **MySQL Workbench** ili drugi MySQL klijent.  
2. Pokrenite skriptu `skripta za bazu.txt` da kreirate bazu i tabele:  
3. Opcionalno, ubacite sample podatke: `skripta za testne podatke.txt`
4. U appsettings.json fajlu postavite connection string za MySQL bazu
5. Pokrenuti aplikaciju u Visual Studio
