# CPU Slice Map

| Instruction | Opcode | CPU | Decoder | Assembler | Tests | CLI trace | Notes |
|---|---:|---|---|---|---|---|---|
| LDA #imm | A9 | done | done | done | done | done | updates Z/N |
| TAX | AA | done | done | done | done | done | transfers A to X |
| INX | E8 | done | done | done | done | done | updates Z/N |
| STA abs | 8D | done | done | done | done | done | no flag updates |
| BRK | 00 | done | done | done | done | done | halts CPU |
| CMP #imm | C9 | done | done | done | done | done | Z/N/C now |
| JMP abs | 4C | done | done | done | done | done | absolute jump |
| BEQ rel | F0 | done | done | done | done | done | branch on Zero |
| BNE rel | D0 | done | done | done | done | done | branch on !Zero |
| LDX #imm | A2 | done | done | planned | done | done | phase 3 |
| DEX | CA | done | done | planned | done | done | phase 3 |
| CPX #imm | E0 | done | done | planned | done | done | phase 3 |
| STX abs | 8E | done | done | planned | done | done | phase 3 |

Maintain this map after each opcode phase.
