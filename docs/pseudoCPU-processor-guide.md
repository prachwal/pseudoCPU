# pseudoCPU Processor Guide

## Spis treści

1. [Cel dokumentu](#1-cel-dokumentu)
2. [Założenia projektowe procesora](#2-założenia-projektowe-procesora)
3. [Aktualny model procesora](#3-aktualny-model-procesora)
4. [Rejestry procesora](#4-rejestry-procesora)
5. [Flagi procesora](#5-flagi-procesora)
6. [Pamięć i adresowanie](#6-pamięć-i-adresowanie)
7. [Cykl wykonania instrukcji](#7-cykl-wykonania-instrukcji)
8. [Obsługiwane instrukcje](#8-obsługiwane-instrukcje)
9. [Szczegółowa semantyka instrukcji](#9-szczegółowa-semantyka-instrukcji)
10. [Assembler bootstrapowy](#10-assembler-bootstrapowy)
11. [CLI i śledzenie wykonania](#11-cli-i-śledzenie-wykonania)
12. [Strategia testowania](#12-strategia-testowania)
13. [Zasady dodawania nowych instrukcji](#13-zasady-dodawania-nowych-instrukcji)
14. [Model rozwoju procesora w fazach](#14-model-rozwoju-procesora-w-fazach)
15. [Ograniczenia aktualnej implementacji](#15-ograniczenia-aktualnej-implementacji)
16. [Docelowy kierunek architektury](#16-docelowy-kierunek-architektury)
17. [Przykładowe programy ASM](#17-przykładowe-programy-asm)
18. [Checklisty dla agentów](#18-checklisty-dla-agentów)
19. [Słownik pojęć](#19-słownik-pojęć)

---

## 1. Cel dokumentu

Ten dokument opisuje procesor `pseudoCPU` rozwijany w repozytorium `prachwal/pseudoCPU`. Celem jest utrzymanie jednego, czytelnego źródła wiedzy o aktualnym modelu procesora, sposobie dodawania instrukcji, zasadach testowania oraz ograniczeniach przyjętych w kolejnych fazach projektu.

Dokument nie zastępuje kodu ani testów. Ma pełnić rolę instrukcji projektowej dla człowieka i agenta LLM, który wykonuje kolejne taski. Każda zmiana rozszerzająca procesor powinna aktualizować ten dokument albo powiązaną mapę funkcjonalności, jeżeli zmienia publiczny kontrakt procesora, assemblera, CLI lub testów.

Projekt jest prowadzony metodą przyrostową. Oznacza to, że procesor nie jest implementowany jako pełny emulator od razu. Każda faza dodaje wąski, działający wycinek: najpierw rdzeń wykonawczy, potem przepływ sterowania, następnie flagi, pętle licznikowe, assembler, CLI i kolejne grupy instrukcji.

---

## 2. Założenia projektowe procesora

`pseudoCPU` jest uproszczonym procesorem inspirowanym zgodnością opcode-level z MOS 6502, ale rozwijanym etapami. Priorytetem obecnych faz jest poprawność funkcjonalna, testowalność i możliwość uruchamiania małych programów, a nie pełna zgodność cyklowa z rzeczywistym układem 6502.

Podstawowe założenia są następujące:

1. Procesor ma być rozwijany pionowymi wycinkami funkcjonalności.
2. Każdy wycinek powinien zawierać semantykę CPU, dekoder opcode, assembler, testy i w razie potrzeby obsługę CLI.
3. GitHub Issues są źródłem prawdy dla zakresu, kryteriów akceptacji i wyników weryfikacji.
4. Nowe instrukcje nie powinny być dodawane przypadkowo. Każda grupa opcode powinna mieć uzasadnienie funkcjonalne.
5. W pierwszych fazach brak pełnego modelu 6502 jest świadomym ograniczeniem, a nie błędem.
6. Zgodność cyklowa, przerwania, stos, tryb dziesiętny i pełny rejestr statusu są odkładane do osobnych faz.

Najważniejsza zasada brzmi: procesor ma zawsze pozostawać w stanie testowalnym. Jeżeli dodanie instrukcji wymaga większej przebudowy, należy rozdzielić ją na osobny task architektoniczny i osobny task funkcjonalny.

---

## 3. Aktualny model procesora

Aktualna implementacja opiera się na klasie `BootstrapCpu` w projekcie `pseudoCPU.Core`. Jest to mały rdzeń wykonawczy przeznaczony do pierwszych faz projektu.

Procesor posiada:

- pamięć liniową 64 KB,
- rejestr akumulatora `A`,
- rejestr indeksowy `X`,
- licznik programu `PC`,
- flagi `Zero` i `Negative`,
- stan zatrzymania `IsHalted`,
- publiczne API do ładowania programu, wykonywania pojedynczego kroku, wykonywania limitowanej liczby kroków i wykonywania do zatrzymania.

Aktualny model nie posiada jeszcze:

- pełnego rejestru statusu jako bajtu,
- rejestru `Y`,
- stosu,
- przerwań,
- wektorów resetu/NMI/IRQ,
- licznika cykli,
- magistrali urządzeń,
- modeli MMIO,
- pełnych trybów adresowania 6502.

To ograniczenie jest intencjonalne. Obecny rdzeń ma umożliwiać bezpieczne dodawanie małych grup instrukcji oraz testowanie ich przez ASM, bajty binarne i CLI.

---

## 4. Rejestry procesora

### 4.1. Rejestr `A`

`A` jest akumulatorem. W aktualnym zakresie jest używany przez instrukcje ładowania, transferu, porównania i zapisu do pamięci.

Przykładowe instrukcje używające `A`:

- `LDA #imm`,
- `TAX`,
- `CMP #imm`,
- `STA abs`.

`A` jest rejestrem 8-bitowym. Operacje na nim powinny zachowywać się jak operacje na bajcie. Jeżeli operacja może wyjść poza zakres 0x00-0xFF, należy jawnie określić wraparound lub inne zachowanie w issue przed implementacją.

### 4.2. Rejestr `X`

`X` jest rejestrem indeksowym. W aktualnym kodzie jest już używany przez `TAX` i `INX`. W fazie 3 jest planowany jako licznik pętli przez instrukcje:

- `LDX #imm`,
- `DEX`,
- `CPX #imm`,
- `STX abs`.

Docelowo `X` może być używany także do trybów adresowania indeksowego, ale to jest poza zakresem aktualnych faz.

### 4.3. Rejestr `PC`

`PC` jest licznikiem programu. Wskazuje adres następnego bajtu instrukcji albo operandu do pobrania.

Typowy przebieg:

1. `Step()` pobiera opcode spod adresu `PC`.
2. `PC` zwiększa się po pobraniu opcode.
3. Instrukcja pobiera ewentualne operand bytes.
4. `PC` zwiększa się po każdym pobranym bajcie operandu.
5. Instrukcje skoku i branch mogą nadpisać `PC`.

W przypadku instrukcji branch relative offset jest liczony względem `PC` po pobraniu bajtu offsetu. To jest kluczowe dla poprawnego liczenia ręcznych offsetów w assemblerze bootstrapowym.

---

## 5. Flagi procesora

### 5.1. `Zero`

Flaga `Zero` oznacza, że wynik ostatniej istotnej operacji wynosi `0x00`.

Przykładowe instrukcje aktualizujące `Zero`:

- `LDA #imm`,
- `TAX`,
- `INX`,
- `CMP #imm`,
- planowane `LDX #imm`,
- planowane `DEX`,
- planowane `CPX #imm`.

Instrukcje zapisu do pamięci, takie jak `STA abs`, nie powinny same aktualizować `Zero`, chyba że osobne issue zmieni tę zasadę.

### 5.2. `Negative`

Flaga `Negative` oznacza, że bit 7 wyniku operacji jest ustawiony. Dla wartości 8-bitowych oznacza to zakres `0x80-0xFF`.

Przykład:

```asm
LDA #$80
```

Po wykonaniu takiej instrukcji oczekiwane jest:

```text
A = 0x80
Negative = true
Zero = false
```

### 5.3. `Carry`

`Carry` jest planowaną flagą fazy 3. Powinna zostać dodana jako jawna publiczna właściwość procesora.

Minimalna semantyka dla porównania 6502-style:

```text
CMP #imm:
Carry = A >= operand
Zero = A == operand
Negative = bit7(A - operand)
```

Dla planowanego `CPX #imm` analogicznie:

```text
CPX #imm:
Carry = X >= operand
Zero = X == operand
Negative = bit7(X - operand)
```

Na tym etapie `Carry` nie musi jeszcze obsługiwać `ADC`, `SBC`, rotacji ani przesunięć bitowych. Te instrukcje wymagają osobnych tasków.

### 5.4. Flagi poza aktualnym zakresem

Aktualnie poza zakresem są:

- `Overflow`,
- `Interrupt Disable`,
- `Decimal`,
- `Break`,
- pełne mapowanie status register do jednego bajtu,
- wpływ stosu na status register.

Nie należy dodawać tych flag przy okazji zwykłych tasków opcode, jeżeli nie są wymienione w issue.

### 5.5. Bootstrapowy bajt statusu dla `PHP` / `PLP`

Do czasu osobnego epica na pełny status register 6502 bootstrapowy snapshot statusu obejmuje tylko aktualnie wspierane flagi:

```text
bit 0 = Carry
bit 1 = Zero
bit 7 = Negative
bit 2-6 = reserved / ignored
```

Kontrakt dla przyszłej implementacji:

- `PHP` zapisuje na stack jedynie ten bootstrapowy snapshot.
- `PHP` ustawia bity `2-6` na `0`.
- `PLP` odtwarza tylko `Carry`, `Zero` i `Negative` z tego snapshotu.
- `PLP` ignoruje bity `2-6`; nie są one traktowane jako pełna semantyka status register 6502.

Ten kontrakt nie wprowadza jeszcze `Overflow`, `Interrupt Disable`, `Decimal` ani `Break` jako aktywnych flag procesora.

---

## 6. Pamięć i adresowanie

### 6.1. Pamięć liniowa

Procesor używa liniowej przestrzeni adresowej 64 KB. Adresy mieszczą się w zakresie:

```text
0x0000 - 0xFFFF
```

Program może zostać załadowany pod wskazany adres startowy. Po załadowaniu `PC` powinien wskazywać adres startowy.

### 6.2. Little-endian

Adresy 16-bitowe są kodowane jako little-endian:

```text
low byte, high byte
```

Przykład:

```asm
STA $1234
```

Powinien zostać zakodowany jako:

```text
8D 34 12
```

Analogicznie `JMP $0600` powinien zostać zakodowany jako:

```text
4C 00 06
```

### 6.3. Tryby adresowania aktualnie używane

Aktualny lub planowany bootstrapowy zakres obejmuje:

| Tryb | Przykład | Opis |
|---|---|---|
| Immediate | `LDA #$01` | Operand jest bajtem zapisanym bezpośrednio po opcode. |
| Implied | `TAX` | Instrukcja nie ma jawnego operandu. |
| Absolute | `STA $2000` | Operand to 16-bitowy adres little-endian. |
| Relative | `BNE $FB` | Operand to signed byte offset względem `PC` po pobraniu offsetu. |

Tryby poza aktualnym zakresem:

- zero page,
- zero page indexed,
- absolute indexed,
- indirect,
- indexed indirect,
- indirect indexed,
- accumulator addressing.

Nie należy dodawać trybów adresowania bez osobnego issue.

---

## 7. Cykl wykonania instrukcji

Aktualny rdzeń procesora działa według prostego cyklu:

```text
Fetch opcode -> Decode opcode -> Fetch operands -> Execute -> Update state
```

Metody wykonania:

| Metoda | Znaczenie |
|---|---|
| `LoadProgram(program, startAddress)` | Czyści pamięć, ładuje program, resetuje rejestry i ustawia `PC`. |
| `Step()` | Wykonuje jedną instrukcję, o ile CPU nie jest zatrzymany. |
| `RunSteps(stepCount)` | Wykonuje maksymalnie zadaną liczbę instrukcji. Nie wymaga `BRK`. |
| `RunUntilHalt(maxSteps)` | Wykonuje program do `BRK` albo do przekroczenia limitu kroków. |
| `Run(maxSteps)` | Convenience wrapper dla uruchomienia programu do zatrzymania. |

Dla bezpieczeństwa każdy dłuższy przebieg powinien mieć limit kroków. Programy z pętlą bez warunku wyjścia muszą być uruchamiane przez `RunSteps` albo przez CLI z `--max-steps`.

---

## 8. Obsługiwane instrukcje

### 8.1. Aktualny slice

| Instrukcja | Opcode | Tryb | Status |
|---|---:|---|---|
| `LDA #imm` | `0xA9` | Immediate | Implemented |
| `TAX` | `0xAA` | Implied | Implemented |
| `INX` | `0xE8` | Implied | Implemented |
| `STA abs` | `0x8D` | Absolute | Implemented |
| `BRK` | `0x00` | Implied | Implemented |
| `CMP #imm` | `0xC9` | Immediate | Implemented, `Carry` planned in phase 3 |
| `JMP abs` | `0x4C` | Absolute | Implemented |
| `BEQ rel` | `0xF0` | Relative | Implemented |
| `BNE rel` | `0xD0` | Relative | Implemented |

### 8.2. Planowany phase 3 slice

| Instrukcja | Opcode | Tryb | Status |
|---|---:|---|---|
| `LDX #imm` | `0xA2` | Immediate | Planned |
| `DEX` | `0xCA` | Implied | Planned |
| `CPX #imm` | `0xE0` | Immediate | Planned |
| `STX abs` | `0x8E` | Absolute | Planned |

### 8.3. Instrukcje poza zakresem

Poza aktualnym zakresem są m.in.:

- `ADC`, `SBC`,
- `AND`, `ORA`, `EOR`,
- `ASL`, `LSR`, `ROL`, `ROR`,
- `JSR`, `RTS`, `RTI`,
- `PHA`, `PLA`, `PHP`, `PLP`,
- `LDY`, `STY`, `CPY`,
- `BIT`,
- `CLC`, `SEC`, `CLI`, `SEI`, `CLV`, `CLD`, `SED`,
- pełne warianty adresowania dla istniejących instrukcji.

---

## 9. Szczegółowa semantyka instrukcji

### 9.1. `LDA #imm`

Opcode:

```text
A9
```

Format:

```asm
LDA #$NN
```

Działanie:

```text
A = operand
Zero = A == 0
Negative = bit7(A) == 1
```

Przykład:

```asm
LDA #$00
```

Oczekiwany stan:

```text
A = 0x00
Zero = true
Negative = false
```

### 9.2. `TAX`

Opcode:

```text
AA
```

Format:

```asm
TAX
```

Działanie:

```text
X = A
Zero = X == 0
Negative = bit7(X) == 1
```

### 9.3. `INX`

Opcode:

```text
E8
```

Format:

```asm
INX
```

Działanie:

```text
X = (X + 1) & 0xFF
Zero = X == 0
Negative = bit7(X) == 1
```

Wartość `X` powinna zachowywać się jak bajt. Przejście z `0xFF` powinno dać `0x00`.

### 9.4. `STA abs`

Opcode:

```text
8D
```

Format:

```asm
STA $HHLL
```

Działanie:

```text
memory[address] = A
```

Instrukcja nie zmienia flag.

Przykład:

```asm
LDA #$2A
STA $2000
```

Oczekiwany efekt:

```text
memory[$2000] = 0x2A
```

### 9.5. `BRK`

Opcode:

```text
00
```

Format:

```asm
BRK
```

Działanie w aktualnym modelu:

```text
IsHalted = true
```

Aktualne `BRK` nie obsługuje stosu, wektora przerwania ani pełnej semantyki 6502. Jest traktowane jako instrukcja zatrzymania programu.

### 9.6. `CMP #imm`

Opcode:

```text
C9
```

Format:

```asm
CMP #$NN
```

Działanie aktualne:

```text
result = A - operand
Zero = A == operand
Negative = bit7(result) == 1
```

Planowane uzupełnienie w fazie 3:

```text
Carry = A >= operand
```

`CMP` nie zmienia wartości `A`.

### 9.7. `JMP abs`

Opcode:

```text
4C
```

Format:

```asm
JMP $HHLL
```

Działanie:

```text
PC = address
```

Adres jest pobierany jako little-endian.

### 9.8. `BEQ rel`

Opcode:

```text
F0
```

Format:

```asm
BEQ $NN
```

Działanie:

```text
if Zero == true:
    PC = PC + signed_offset
```

Offset jest liczony względem `PC` po pobraniu bajtu offsetu.

### 9.9. `BNE rel`

Opcode:

```text
D0
```

Format:

```asm
BNE $NN
```

Działanie:

```text
if Zero == false:
    PC = PC + signed_offset
```

Przykład offsetu ujemnego:

```asm
BNE $FE
```

`$FE` oznacza `-2` jako signed byte.

### 9.10. Planowane `LDX #imm`

Opcode:

```text
A2
```

Format:

```asm
LDX #$NN
```

Planowane działanie:

```text
X = operand
Zero = X == 0
Negative = bit7(X) == 1
```

### 9.11. Planowane `DEX`

Opcode:

```text
CA
```

Format:

```asm
DEX
```

Planowane działanie:

```text
X = (X - 1) & 0xFF
Zero = X == 0
Negative = bit7(X) == 1
```

### 9.12. Planowane `CPX #imm`

Opcode:

```text
E0
```

Format:

```asm
CPX #$NN
```

Planowane działanie:

```text
result = X - operand
Zero = X == operand
Negative = bit7(result) == 1
Carry = X >= operand
```

`CPX` nie zmienia wartości `X`.

### 9.13. Planowane `STX abs`

Opcode:

```text
8E
```

Format:

```asm
STX $HHLL
```

Planowane działanie:

```text
memory[address] = X
```

Instrukcja nie powinna zmieniać flag.

---

## 10. Assembler bootstrapowy

Assembler bootstrapowy służy do generowania bajtów dla aktualnie obsługiwanego slice CPU. Nie jest jeszcze pełnym assemblerem 6502.

### 10.1. Obsługiwane typy linii

Assembler powinien obsługiwać:

- puste linie,
- komentarze zaczynające się od `;`,
- mnemoniki z operandem immediate,
- mnemoniki implied bez operandu,
- mnemoniki absolute z operandem 16-bitowym,
- branch relative z ręcznie podanym bajtem offsetu.

### 10.2. Brak etykiet

Aktualnie assembler nie obsługuje etykiet.

Niepoprawne w aktualnym zakresie:

```asm
loop:
DEX
BNE loop
```

Poprawne w aktualnym zakresie:

```asm
DEX
CPX #$00
BNE $FB
```

### 10.3. Brak `.org`

Assembler nie obsługuje dyrektywy `.org`. Adres startowy programu jest przekazywany przez CPU lub CLI:

```powershell
dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/simple.asm --start 0x0600 --max-steps 100
```

### 10.4. Literały

Aktualnie preferowane są literały hex:

```asm
LDA #$01
STA $2000
BNE $FB
```

Jeżeli zostanie dodana obsługa decimal/binary literals, musi mieć osobny task i testy negatywne.

### 10.5. Ręczne branch offsety

Branch offset jest signed byte liczonym względem `PC` po pobraniu offsetu.

Dla programu:

```asm
LDX #$03
DEX
CPX #$00
BNE $FB
STX $2000
BRK
```

Offset `$FB` oznacza `-5` i powinien cofać wykonanie do instrukcji `DEX`.

---

## 11. CLI i śledzenie wykonania

CLI znajduje się w projekcie `pseudoCPU.Cli`.

### 11.1. Komenda `run-asm`

Przykład:

```powershell
dotnet run --project src/pseudoCPU.Cli -- run-asm --source examples/simple.asm --start 0x0600 --max-steps 100 --trace
```

Znaczenie opcji:

| Opcja | Znaczenie |
|---|---|
| `--source` | Ścieżka do pliku ASM. |
| `--start` | Adres startowy programu, np. `0x0600`. |
| `--max-steps` | Limit instrukcji do wykonania. |
| `--trace` | Włącza ślad wykonania per instrukcja. |

### 11.2. Komenda `run-bin`

Przykład:

```powershell
dotnet run --project src/pseudoCPU.Cli -- run-bin --source examples/simple.bin --start 0x0600 --max-steps 100 --trace
```

`run-bin` ładuje bajty bezpośrednio do pamięci i uruchamia CPU od wskazanego adresu.

### 11.3. Trace

Trace powinien pokazywać co najmniej:

- numer kroku,
- `PC` przed wykonaniem instrukcji,
- opcode,
- mnemonic,
- rejestry,
- flagi.

Przykładowy format:

```text
#0001 PC=0x0600 OPC=0xA9 LDA #$01 A=0x01 X=0x00 Z=false N=false
```

Po dodaniu `Carry` trace powinien zawierać także `C=true|false` albo równoważny zapis.

---

## 12. Strategia testowania

Projekt powinien stosować testy wąskie i testy końcowe.

### 12.1. Testy decoder

Każda nowa instrukcja musi mieć test mapowania opcode na enum.

Przykład zakresu:

```text
0xA9 -> LdaImmediate
0x4C -> JmpAbsolute
0xD0 -> BneRelative
```

### 12.2. Testy CPU

Każda nowa instrukcja musi mieć test wykonania CPU. Test powinien sprawdzać:

- rejestry po wykonaniu,
- `PC`,
- flagi,
- pamięć, jeśli instrukcja zapisuje lub czyta dane,
- przypadki brzegowe, jeśli instrukcja je posiada.

### 12.3. Testy assemblera

Każda nowa instrukcja wspierana przez assembler musi mieć test:

```text
ASM -> expected bytes
```

Dla błędnych operandów należy dodać test negatywny.

### 12.4. Testy end-to-end

Dla każdego pionowego slice warto mieć test:

```text
ASM -> bytes -> LoadProgram -> RunUntilHalt -> final CPU state
```

### 12.5. Testy CLI

CLI powinno mieć testy parsera i formattera trace. Pełne testy procesu `dotnet run` mogą być dodane później, jeżeli będą stabilne.

### 12.6. Komendy weryfikacyjne

Podstawowa weryfikacja:

```powershell
dotnet format pseudoCPU.sln --verify-no-changes
dotnet build pseudoCPU.sln
dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj
```

Dla tasków preferowane są wąskie filtry:

```powershell
dotnet test tests/pseudoCPU.Bootstrap.Tests/pseudoCPU.Bootstrap.Tests.csproj --filter "Category=ControlFlow|Category=OpcodeDecoder"
```

---

## 13. Zasady dodawania nowych instrukcji

Każda nowa instrukcja powinna przejść przez ten sam przepływ:

1. Dodać lub zaktualizować issue z jasnym zakresem.
2. Dodać opcode do enum/metadata.
3. Dodać mapowanie w decoderze.
4. Dodać semantykę wykonania w CPU.
5. Dodać testy jednostkowe CPU.
6. Dodać obsługę assemblera, jeśli instrukcja ma być pisana w ASM.
7. Dodać test ASM -> bytes.
8. Dodać test end-to-end, jeśli instrukcja jest częścią pionowego slice.
9. Dodać trace formatter, jeśli CLI ma ją czytelnie pokazywać.
10. Zaktualizować dokumentację i mapę slice CPU.

Nie należy dodawać instrukcji tylko dlatego, że są łatwe. Nowa grupa instrukcji powinna umożliwiać nowy scenariusz, np. pętlę, warunek, zapis pamięci, stos albo wywołanie podprogramu.

---

## 14. Model rozwoju procesora w fazach

### 14.1. Faza bootstrap

Cel: uruchomić pierwszy program.

Zakres:

- `LDA #imm`,
- `TAX`,
- `INX`,
- `STA abs`,
- `BRK`,
- prosty assembler,
- test ASM -> CPU.

### 14.2. Faza control-flow

Cel: umożliwić warunki i skoki.

Zakres:

- `JMP abs`,
- `CMP #imm`,
- `BEQ rel`,
- `BNE rel`,
- CLI runner,
- trace.

### 14.3. Faza status/counter loop

Cel: umożliwić licznikową pętlę programową.

Planowany zakres:

- `Carry`,
- pełniejsze `CMP #imm`,
- `LDX #imm`,
- `DEX`,
- `CPX #imm`,
- `STX abs`,
- end-to-end program z pętlą.

### 14.4. Potencjalna kolejna faza: etykiety assemblera

Cel: usunąć ręczne liczenie branch offsetów.

Możliwy zakres:

- etykiety,
- dwupassowy assembler,
- diagnostyka nieznanej etykiety,
- testy branch forward/backward.

### 14.5. Potencjalna kolejna faza: stos i podprogramy

Cel: umożliwić wywołania funkcji.

Możliwy zakres:

- `JSR`,
- `RTS`,
- `PHA`,
- `PLA`,
- `TXS`,
- `TSX`,
- prosty model stosu.

### 14.6. Potencjalna kolejna faza: MMIO

Cel: umożliwić komunikację z urządzeniami.

Możliwy zakres:

- memory bus,
- zakresy MMIO,
- callback read/write,
- urządzenie testowe,
- CLI dump pamięci lub portu.

---

## 15. Ograniczenia aktualnej implementacji

Aktualne ograniczenia są świadome:

- brak pełnej zgodności 6502,
- brak cycle accuracy,
- brak stosu,
- brak przerwań,
- brak rejestru `Y`,
- brak pełnego status register,
- brak etykiet w assemblerze,
- brak `.org`,
- brak zaawansowanych trybów adresowania,
- brak MMIO,
- brak binarnego formatu ładowania z nagłówkiem,
- brak disassemblera,
- brak debuggera interaktywnego.

Nie należy traktować tych braków jako błędów, dopóki nie ma issue, które przenosi je do zakresu implementacji.

---

## 16. Docelowy kierunek architektury

Docelowo projekt może zostać podzielony na bardziej formalne komponenty:

| Komponent | Rola |
|---|---|
| `CpuState` | Rejestry i flagi procesora. |
| `CpuFlags` | Jawny model statusu. |
| `MemoryBus` | Dostęp do RAM i MMIO. |
| `Instruction` | Metadane opcode, długość, tryb adresowania. |
| `OpcodeDecoder` | Mapowanie bajtów na instrukcje. |
| `ExecutionResult` | Wynik kroku lub przebiegu. |
| `TraceEntry` | Dane śladu wykonania. |
| `Assembler` | Parser i generator bajtów. |
| `CliRunner` | Warstwa uruchomieniowa CLI. |

Nie należy jednak wykonywać dużego refaktoru bez osobnego ADR i issue. Aktualny kod ma być rozwijany małymi krokami.

---

## 17. Przykładowe programy ASM

### 17.1. Prosty program zapisujący akumulator

```asm
LDA #$2A
STA $2000
BRK
```

Oczekiwany efekt:

```text
A = 0x2A
memory[$2000] = 0x2A
IsHalted = true
```

### 17.2. Program z warunkiem

```asm
LDA #$03
CMP #$03
BNE $02
STA $2000
BRK
```

Oczekiwany efekt:

```text
Zero = true
BNE not taken
memory[$2000] = 0x03
```

### 17.3. Planowany program z pętlą X

```asm
LDX #$03
DEX
CPX #$00
BNE $FB
STX $2000
BRK
```

Oczekiwany efekt po fazie 3:

```text
X = 0x00
memory[$2000] = 0x00
Zero = true
Negative = false
Carry = true
IsHalted = true
```

---

## 18. Checklisty dla agentów

### 18.1. Definition of Done

Każdy task implementacyjny powinien spełniać:

- kod jest sformatowany,
- dodano lub zaktualizowano testy,
- narrow test filter przechodzi,
- `dotnet build pseudoCPU.sln` przechodzi,
- `dotnet format pseudoCPU.sln --verify-no-changes` przechodzi,
- dokumentacja została zaktualizowana, jeśli zmienił się kontrakt,
- nie rozszerzono zakresu poza issue,
- wynik zapisano w komentarzu issue.

### 18.2. Scope Guard

Nie implementować bez jawnego zakresu w issue:

- etykiet assemblera,
- `.org`, `.byte`, `.word`,
- nowych trybów adresowania,
- cycle counting,
- przerwań,
- stosu,
- podprogramów,
- niepowiązanych grup opcode,
- nowych komend CLI.

### 18.3. QC gate po fazie

Po każdym epicu należy sprawdzić:

- czy taski spełniły acceptance criteria,
- czy testy pokrywają happy path,
- czy testy pokrywają error path,
- czy dokumentacja zgadza się z kodem,
- czy CLI/help zgadza się z implementacją,
- czy przykłady działają,
- czy nie dodano funkcji poza zakresem,
- czy następna faza nie bazuje na ukrytym długu.

---

## 19. Słownik pojęć

| Pojęcie | Znaczenie |
|---|---|
| Opcode | Bajt identyfikujący instrukcję procesora. |
| Operand | Dane instrukcji zapisane po opcode. |
| Immediate | Tryb, w którym operand jest wartością bezpośrednią. |
| Absolute | Tryb, w którym operand jest 16-bitowym adresem. |
| Relative | Tryb branch z signed byte offsetem. |
| Little-endian | Kolejność zapisu 16-bitowego adresu jako low byte, potem high byte. |
| `PC` | Program Counter, adres następnego bajtu do pobrania. |
| `A` | Akumulator. |
| `X` | Rejestr indeksowy/licznikowy. |
| `Zero` | Flaga wyniku zero. |
| `Negative` | Flaga bitu znaku/wysokiego bitu. |
| `Carry` | Flaga przeniesienia; w porównaniu oznacza `register >= operand`. |
| `BRK` | W aktualnym modelu instrukcja zatrzymująca CPU. |
| Slice | Mały pionowy wycinek funkcjonalności: CPU, decoder, assembler, testy, CLI. |
| QC gate | Kontrola jakości po zakończeniu fazy. |
| ADR | Architecture Decision Record, zapis decyzji architektonicznej. |
