# CPU Slice Map

Bootstrap assembler accepts raw branch offsets only; labels and `.org` remain unsupported.

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
| LDX #imm | A2 | done | done | done | done | done | phase 3 |
| DEX | CA | done | done | done | done | done | phase 3 |
| CPX #imm | E0 | done | done | done | done | done | phase 3 |
| STX abs | 8E | done | done | done | done | done | phase 3 |
| JSR abs | 20 | done | done | done | done | done | pushes return address high then low |
| RTS | 60 | done | done | done | done | done | pops low then high and returns to next instruction |
| PHA | 48 | done | done | done | done | done | pushes A without flag updates |
| PLA | 68 | done | done | done | done | done | pulls into A and updates Z/N |
| PHP | 08 | done | done | done | done | done | pushes bootstrap status snapshot |
| PLP | 28 | done | done | done | done | done | restores Carry/Zero/Negative only |
| LDY #imm | A0 | done | done | done | done | done | updates Z/N |
| INY | C8 | done | done | done | done | done | updates Z/N |
| DEY | 88 | done | done | done | done | done | updates Z/N |
| CPY #imm | C0 | done | done | done | done | done | updates C/Z/N without mutating Y |
| STY abs | 8C | done | done | done | done | done | no flag updates |
| ADC #imm | 69 | done | done | done | done | done | phase 7 immediate add, updates C/V/Z/N |
| SBC #imm | E9 | done | done | done | done | done | phase 7 immediate subtract, updates C/V/Z/N |
| BIT zp | 24 | done | done | done | done | done | phase 7 zero-page only, updates Z/N/V |
| ASL A | 0A | done | done | done | done | done | accumulator shift left, updates C/Z/N |
| LSR A | 4A | done | done | done | done | done | accumulator shift right, updates C/Z/N |
| ROL A | 2A | done | done | done | done | done | accumulator rotate left, updates C/Z/N |
| ROR A | 6A | done | done | done | done | done | accumulator rotate right, updates C/Z/N |
| CLC | 18 | done | done | done | done | done | clears Carry |
| SEC | 38 | done | done | done | done | done | sets Carry |
| CLI | 58 | done | done | done | done | done | clears Interrupt Disable |
| SEI | 78 | done | done | done | done | done | sets Interrupt Disable |
| CLD | D8 | done | done | done | done | done | clears Decimal |
| SED | F8 | done | done | done | done | done | sets Decimal |
| CLV | B8 | done | done | done | done | done | clears Overflow |
| BCC | 90 | done | done | done | done | done | branch on clear Carry |
| BCS | B0 | done | done | done | done | done | branch on set Carry |
| BMI | 30 | done | done | done | done | done | branch on Negative |
| BPL | 10 | done | done | done | done | done | branch on positive |
| BVC | 50 | done | done | done | done | done | branch on clear Overflow |
| BVS | 70 | done | done | done | done | done | branch on set Overflow |

Maintain this map after each opcode phase.
