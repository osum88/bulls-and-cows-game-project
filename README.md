# Bulls and Cows Game

Tato webová aplikace implementuje klasickou logickou hru "Bulls and Cows", kde hráči hádají tajný kód, a systém jim poskytuje nápovědy v podobě "býků" (správná číslice na správném místě) a "krav" (správná číslice na špatném místě).

## Funkcionality
- **Výběr obtížnosti:** Možnost zvolit si obtížnost hry (easy - neomezeně pokusů, normal - 15 pokusů, hard - 5 pokusů).

- **Správa uživatelských účtů:** Registrace, přihlášení pomocí ASP.NET Core Identity.

- **Ukládání statistik her:** Každá dokončená hra je uložena do databáze, včetně času spuštění, výsledku, uhodnutého kódu a počtu pokusů.

- **Historie pokusů:** Ukládání detailů o každém jednotlivém pokusu v průběhu hry.

- **Zobrazení statistik:** Uživatelé mohou prohlížet své herní statistiky s možnostmi filtrování a řazení.

- **Export statistik do XML:** Uživatelé mohou stáhnout své filtrované a seřazené herní statistiky ve formátu XML.

## Hraní hry:
1. Zvolte si obtížnost.

2. Zadejte svůj odhad kódu do vstupního pole.

3. Stiskněte tlačítko pro odeslání odhadu.

4. Zobrazí se vám nápověda v tabulce (Bulls & Cows).

5. Pokračujte v hádání, dokud kód neuhodnete, nebo dokud se nerozhodnete vzdát.

## Statistiky
- Sekce statistik je dostupná pro přihlášené uživatele. 
- Zde můžete:

  - Procházet historii svých odehraných her.

  - Filtrovat hry podle roku, měsíce, dne, stavu (vyhráno/prohráno) a obtížnosti.

  - Řadit výsledky podle různých kritérií (čas, trvání, počet pokusů, atd.).

  - Stáhnout statistiky do XML pomocí tlačítka "Download". Tento export bude obsahovat data podle aktuálně nastavených filtrů a řazení, včetně detailů o všech pokusech v rámci každé hry.

