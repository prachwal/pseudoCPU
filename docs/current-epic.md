# Current Epic State

Ten plik przechowuje stan biezacego epica i informacje fazowe, ktorych nie nalezy dopisywac do `AGENTS.md`.

## Active Epic
- GitHub issue: #25
- Status: active
- Owner agent: `issue-planner`
- Quality gate agent: `epic-qc`

## Current Scope
- Szczegoly zakresu epica nalezy aktualizowac tutaj podczas planowania i synchronizacji z issue.
- Kazda zmiana zakresu musi wskazywac numer issue albo komentarz issue, z ktorego wynika.

## Phase Notes
- Biezace notatki fazowe, decyzje implementacyjne, ryzyka i zaleznosci zapisywac w tej sekcji.
- Po zamknieciu epica przeniesc trwale decyzje do ADR albo dokumentacji domenowej, a ten plik zaktualizowac pod nastepny epic.

## QC Feedback Loop
1. `issue-planner` utrzymuje strukture epic -> taski.
2. `issue-executor` realizuje taski i zapisuje wyniki w issue.
3. `epic-qc` weryfikuje epica po implementacji silniejszym modelem.
4. Jezeli `epic-qc` wykryje brakujacy zakres, regresje lub ryzyko, tworzy follow-up issue z body zapisanym w pliku i linkuje je z epica.
5. Petla wraca do planner/executor do czasu braku blokujacych ustalen QC.

## Verification Snapshot
- Narrow test command: TBD
- Build command: `dotnet build pseudoCPU.sln`
- Format command: `dotnet format pseudoCPU.sln --verify-no-changes`
- Latest QC result: TBD

## Follow-up Issues
- #17 - reference pattern for prior follow-up/task issue.
- #25 - current epic reference.
- New follow-up issues must be added here after `epic-qc` verification.
