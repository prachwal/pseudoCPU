# 6502 Domain Rules

Ten dokument zawiera stale reguly domenowe dla zmian CPU/assembler/CLI w repo `pseudoCPU`. Agenci maja czytac ten plik tylko wtedy, gdy task dotyczy domeny CPU, opcode'ow, flag, rejestrow, assemblera, trace albo dokumentacji procesora.

## Core Rule

Nie pisz ogolnie `zgodne z 6502`, jezeli istnieje ryzyko off-by-one, kolejnosci bajtow, zmiany flag albo ukrytego rozszerzenia zakresu. W epicu i tasku wypisz dokladny kontrakt semantyczny.

## Current Bootstrap Scope

Projekt implementuje pseudo CPU inspirowane zgodnoscia opcode-level z 6502 etapami. Kazdy epic ma okreslac waski slice. Nie wolno cicho rozszerzac scope na pelny 6502.

Aktualne stale zalozenia po epikach #26 i #36:

- stack page: `$0100-$01FF`,
- `SP`: 8-bitowy rejestr stack pointer,
- efektywny adres stacka: `$0100 + SP`,
- push: zapis pod `$0100 + SP`, potem dekrementacja `SP`,
- pop: inkrementacja `SP`, potem odczyt spod `$0100 + SP`,
- bootstrapowy start `SP`: `$FF` do czasu osobnego epica reset/interrupt,
- bootstrapowy status byte dla `PHP` / `PLP`:
  - `Carry` = bit 0,
  - `Zero` = bit 1,
  - `Negative` = bit 7,
  - bity 2-6 sa zarezerwowane / ignorowane do osobnego epica pelnego status register.

## Opcode Task Requirements

Kazdy task dodajacy albo zmieniajacy opcode musi jawnie wskazac:

- mnemonic,
- opcode byte,
- addressing mode,
- dlugosc instrukcji,
- zmiany `PC`,
- zmiany rejestrow,
- zmiany flag,
- odczyty/zapisy pamieci,
- wymagane testy,
- wymagane aktualizacje docs,
- out-of-scope.

## Flag Rules

Nie dodawaj ani nie zmieniaj flag bez jawnego kontraktu w issue.

Dla kazdej instrukcji zapisz:

- ktore flagi zmienia,
- ktorych flag nie zmienia,
- ktore flagi sa zarezerwowane albo unsupported,
- czy zachowanie flag jest bootstrapowe, czy docelowe 6502.

## Stack Rules

Dla zmian stackowych sprawdz:

- adresowanie `$0100 + SP`,
- wrap-around 8-bit `SP`,
- LIFO ordering,
- czy push/pop balansuje `SP`,
- ktore bajty sa zapisywane i w jakiej kolejnosci,
- czy testy potwierdzaja zawartosc stacka, nie tylko wynik koncowy.

## JSR / RTS Contract

Dla `JSR abs` i `RTS` kontrakt jest utrwalony po #26:

- `JSR abs` odklada adres powrotu `PC + 2`, czyli adres ostatniego bajtu instrukcji `JSR`,
- `JSR abs` odklada high byte, potem low byte,
- `RTS` pobiera low byte, potem high byte,
- `RTS` sklada adres, zwieksza go o 1 i wraca do instrukcji po `JSR`.

## PHP / PLP Contract

Dla `PHP` / `PLP` kontrakt jest utrwalony po #36:

- `PHP` zapisuje bootstrapowy snapshot statusu na stack,
- `PHP` nie oznacza pelnego status register 6502,
- `PLP` odtwarza tylko wspierane flagi `Carry`, `Zero`, `Negative`,
- bity 2-6 sa ignorowane albo zarezerwowane,
- `PLP` nie dodaje decimal/interrupt/break/overflow live semantics.

## Assembler Rules

Bootstrapowy assembler nie wspiera:

- etykiet,
- `.org`,
- `.byte`,
- `.word`,
- makr,
- include'ow.

Task assemblera musi podac expected bytes dla kazdego nowego mnemonika.

## CLI / Trace Rules

Trace/CLI musi byc zgodny z assemblerem i decoderem.

Nie dodawaj nowych komend CLI w taskach opcode, chyba ze epic jawnie tego wymaga.

## Documentation Rules

Po zmianach CPU/assembler/CLI sprawdz:

- `docs/cpu-slice-map.md`,
- `docs/pseudoCPU-processor-guide.md`,
- aktualny chapter w `docs/epics/`,
- `docs/current-epic.md` i `docs/current-epic-summary.md`,
- ADR, jesli decyzja ma wplyw na wiele przyszlych epicow.

## ADR Trigger

Dodaj ADR w `docs/adr/`, jezeli decyzja:

- definiuje kontrakt uzywany przez wiele przyszlych epicow,
- zmienia model CPU, pamieci, flag, resetu albo assemblera,
- moze zostac zinterpretowana inaczej przez executorow,
- wymaga utrwalenia kompromisu architektonicznego.

## Out-of-scope Defaults

Dopoki epic nie powie inaczej, poza zakresem sa:

- IRQ/NMI/RESET vectors,
- `RTI`,
- pelny status register 6502,
- cycle counting,
- nowe addressing modes poza wskazanym opcode,
- etykiety i dyrektywy assemblera,
- nowe komendy CLI.
