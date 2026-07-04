# Travel Planner

Travel Planner je web aplikacija za organizaciju putovanja koja korisniku omogućava da u jednom mestu prati sve važne detalje vezane za putovanje: plan putovanja, destinacije, aktivnosti po danima, troškove, budžet, checklist i dijeljenje plana sa drugima.

Cilj aplikacije nije samo da prikupi informacije, već da omogući lakše planiranje i pregled cijelokupnog putovanja od početka do kraja.

## Šta aplikacija omogućava

- kreiranje i upravljanje planovima putovanja
- dodavanje destinacija i njihovih detalja
- planiranje aktivnosti po danima
- evidenciju troškova i praćenje budžeta
- checklist / packing list za stvari koje treba pripremiti
- registraciju i prijavu korisnika
- dijeljenje plana putem tokena i QR koda
- administrativni panel za upravljanje sadržajem i korisnicima

## Glavne funkcionalnosti

### Upravljanje planovima
Korisnik može kreirati novi plan putovanja, unositi osnovne informacije kao što su naziv, opis, datumi putovanja, planirani budžet i napomene. Nakon toga može pregledati svoje planove, otvoriti detalje, izmeniti ih ili obrisati.

### Destinacije
Svaki plan može sadržati jednu ili više destinacija. Za svaku destinaciju moguće je unijeti naziv, lokaciju, datume boravka i kratki opis.

### Aktivnosti po danima
Aplikacija podržava organizaciju aktivnosti po datumima, sa mogućnošću dodavanja naziva aktivnosti, vremena, lokacije, troškova, statusa i opisa. Aktivnosti se mogu pregledati i kroz kalendar.

### Troškovi i budžet
Korisnik može evidentirati troškove po kategorijama, a sistem automatski računa ukupan iznos i preostali budžet u odnosu na planirani budžet.

### Checklist
Pored samog plana, korisnik može pratiti stvari i obaveze koje treba obaviti prije putovanja, kao što su pasoš, karta, rezervacija, putno osiguranje i slično.

### Dijeljenje plana
Plan može biti dijeljen sa drugim osobama putem linka ili QR koda. Postoje dva nivoa pristupa: pregled i uređivanje.

## Tehnologije

### Frontend
- React
- Vite
- React Router
- Context API
- Axios

### Backend
- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT autentikacija
- Service Fabric (struktura projekta i host aplikacije)

## Struktura projekta

- frontend/ - React aplikacija
- WebProjekat.Domain/ - domenski modeli
- WebProjekat.Application/ - servisni sloj i DTO objekti
- WebProjekat.Infrastructure/ - baza, migracije i persistence sloj
- WebProjekat/ - Service Fabric projekat
- UserService (stateless) - autentifikacija i korisnici
- TravelPlanService (stateful) - planovi, destinacije, aktivnosti
- ExpenseService (stateless) - troškovi i dijeljenje

## Preduslovi

Da biste pokrenuli projekat, potrebno je imati:

- Node.js i npm
- .NET SDK
- SQL Server (SQL Server Express)
- Visual Studio ili VS Code

## Pokretanje aplikacije

### 1. Klonirajte projekat

```bash
git clone <repo-url>
cd PROJmain
```

Ili direktno otvaranjem u VS ili VS Code

### 2. Pokretanje backend-a

Otvorite PROJmain.sln u Visual Studio-u, postavite WebProjekat (Service Fabric projekat) kao Startup Project, i pritisnite F5. Ovo će pokrenuti sve mikroservise lokalno.

### 3. Pokretanje frontend-a

```bash
cd frontend
npm install
npm run dev
```

Frontend će biti dostupan u browseru kroz Vite razvojni server.

## Konfiguracija frontend-a

Frontend koristi promijenljivu okruženja za API URL. U fajlu [frontend/.env](frontend/.env) nalazi se osnovna vrednost:

```env
VITE_API_URL=http://localhost:8594
```

## Napomene o implementaciji

Projekat je realizovan tako da prati osnovne zahtijeve iz specifikacije:

- razdvojene komponente na frontend-u
- forme za unos podataka
- validacija osnovnih polja na backend-u
- JWT autentifikacija
- podrška za dijeljenje plana
- struktura projekta podijeljena na domenski, aplikativni i infrastrukturni sloj

## Autor

Bojana Savic PR109/2022
