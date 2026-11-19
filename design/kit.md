# Jet: Kit de habilidades

← [design/context.md](context.md) · [concept.md](concept.md)

**Tema do kit:** voo + fogo. A jetpack é mobilidade, recurso **e** arma. A chama azul é o "modo poderoso", visual
compartilhado por Utilitária, Especial e pelo debuff de fogo azul.

## Passiva: Jetpack
**Intenção:** dar ao Jet uma mobilidade vertical e livre que nenhum survivor vanilla tem, mas limitada por um recurso,
para que voar seja uma decisão e não um estado permanente.

| Aspecto | Definição |
|---|---|
| Ativação | 2º toque no espaço, estando no ar (substitui o double jump) |
| Impulso inicial | Leve, na direção da mira |
| Voo | Velocidade constante na direção da **mira** (3D livre); dura enquanto o espaço estiver segurado |
| Direcionais no voo | **W/A/D** desviam um pouco a trajetória (a mira domina). **S não freia**: Jet não para até soltar o espaço |
| Combate | Jet **atira e usa skills durante o voo** |
| Fim | Soltar o espaço **ou** combustível = 0 |
| Religar | Pode religar no mesmo pulo, **quantas vezes quiser, enquanto houver combustível** |
| Recurso | Combustível, com medidor ao lado do personagem (estilo Void Fiend) |
| Recarga | Começa após **~1 s sem voar**; rápida no chão, mais lenta no ar |
| Velocidade | **Escala 100% com velocidade de movimento** (Energy Drink, Goat Hoof etc.) |
| Hopoo Feather | **Não dá pulos extras.** Cada pena **aumenta a capacidade do tanque** (mais tempo de voo) |

---

## Primária: Desert Eagle *(nome provisório)*
**Intenção:** tiro pesado e ritmado. Cada disparo importa, e o 3º tiro recompensa alinhar inimigos.

| Aspecto | Definição |
|---|---|
| Tipo | Pistola semiautomática (hitscan), **um pouco lenta** |
| Dano | **350%** por tiro |
| Ritmo | A cada **3º tiro**, o disparo **perfura** inimigos |
| Uso | Chão e ar (inclusive voando) |

## Secundária: UES Safe Travels Supplies
**Intenção:** utilidade de área com um "presente" raro, um aceno à nave UES Safe Travels da lore do RoR2.
A chance é baixa de propósito: é um momento de surpresa, não uma fonte de economia.

| Aspecto | Definição |
|---|---|
| Ação | Dispara um **sinalizador** com a outra mão |
| Efeito | **Dano em área** + aplica **Burn** |
| Bônus | Se o sinalizador **mata** um inimigo: **0,5%** de chance de surgir um **baú** |
| Múltiplas mortes | A chance é rolada **uma vez por disparo**, não por inimigo morto |
| Baú | Qualquer tipo de baú, **exceto lendário** |

## Utilitária: Do a Barrel Roll!
**Intenção:** dash ofensivo que transforma a jetpack em arma. Recompensa posicionamento com os direcionais e oferece
um "all-in" que troca combustível (e um pouco de vida) por dano massivo.

| Aspecto | Definição |
|---|---|
| Restrição | **Só no ar** (não funciona no chão) |
| Direção | Direcionais apertados no momento do Shift; **sem direcional = sobe na vertical** |

**1ª ativação: Burst**
- **Não gasta combustível.**
- A chama da jetpack fica **azul**.
- Explosão que **impulsiona** Jet e deixa um **rastro de fogo para trás** (o dano fica no ponto de partida e no caminho).
- **600% de dano** + **Burn**.
- Abre uma **janela de 2 a 3 s** para o recast (ritmo parecido com os dashes encadeados do Mercenário).
  Se a janela acabar sem recast, a skill entra em **cooldown**.

**2ª ativação (dentro da janela): Overburn**
- Consome **todo o combustível restante**. Funciona **mesmo com o tanque vazio**.
- Impulsiona Jet **ainda mais longe** que o Burst.
- **1200% de dano base, escalando com o combustível consumido** (tanque vazio = base; tanque cheio = bônus máximo).
- Aplica **1 stack de Burn no próprio Jet**.

## Especial: *(nome a definir)*
**Intenção:** o ataque aéreo definitivo, um rasante de caça. Premia jogar no ar e "preparar" inimigos com Burn
(Secundária e Utilitária) para depois convertê-los em fogo azul.

| Aspecto | Definição |
|---|---|
| Restrição | **Só no ar** |
| Ação | Impulso **curto e rápido para frente**, com a jetpack em **chama azul** |
| Dano | O **rastro azul** causa **500% por tick**, constante até o fim do impulso |
| Rasante | Se o impulso for **colidir com o chão**, Jet faz um **rasante**: raspa o chão e **curva de volta para cima** |
| Fogo azul | Durante o rasante, inimigos com **Burn** têm todos os stacks **convertidos** em **Fogo Azul** |
| Fogo Azul (debuff) | DoT próprio: **mais dano e mais duração** que o Burn |

---

## Perguntas em aberto
**Primária**
1. **Nome final** da pistola. "Desert Eagle" é uma marca registrada de arma real; para um mod público,
   o ideal é um nome próprio (ex.: modelo fictício de uma fabricante da lore). O visual pode ser o de uma pistola pesada.
2. **Cadência** aproximada: 1 tiro a cada ~0,5 s? E tem queda de dano com a distância?

**Secundária**
3. O sinalizador é **projétil com arco** (cai como granada) ou **reto** (sobe e explode)? Explode ao tocar o inimigo?
4. **Cooldown e cargas.**
5. "Qualquer baú exceto lendário": entram baús de **equipamento**, **multishop**, **baús de categoria** (dano/cura/utilidade),
   **Lunar Pod** e **Void Cradle**? Sugestão: só baús de item comum, de equipamento e de categoria.

**Especial**
6. **Taxa de tick** do rastro. 500% por tick é muito forte se o tick for rápido; sugestão: ~4 ticks/s por inimigo.
7. O impulso segue a **mira** (pode mirar para baixo e forçar o rasante) ou é sempre na horizontal?
8. **Duração** do impulso, **cooldown**, e se gasta combustível.

## Notas de implementação (para o código, não são decisões de design)
- **Passiva:** estado principal do body (subclasse de `GenericCharacterMain`) intercepta o pulo no ar; state machine
  dedicada "Jetpack" com `Idle`/`Boost`/`Flight`, separada da "Weapon", por isso atirar voando funciona naturalmente.
  No voo: gravidade desligada, velocidade = `aimDirection` × `moveSpeed`, mais um desvio de `moveVector` sem a
  componente "para trás". Movimento de player é autoritativo no cliente, então funciona em multiplayer sem sincronização extra.
- **Combustível:** componente no body (capacidade = base + bônus por Hopoo Feather). API para a utilitária
  consumir "todo o restante" e informar quanto foi consumido. Medidor no HUD = submódulo genérico do Framework.
- **Hopoo Feather:** o `CharacterMotor` conta pulos extras pela pena; o estado principal do Jet ignora esses pulos e
  usa a contagem de penas só para a capacidade do tanque.
- **Chama azul:** um único componente de VFX da jetpack com modos (normal/azul), compartilhado por Utilitária e Especial.
- **Primária:** contador de tiros no próprio componente do survivor (sobrevive entre estados) para o 3º tiro perfurante.
- **Secundária:** o baú é spawnado **no servidor** via `DirectorCore` + `InteractableSpawnCard` do baú sorteado.
- **Barrel Roll:** recast com janela, no padrão de skills de reativação; `DotController` para o Burn (inclusive no Jet).
- **Fogo Azul:** DoT customizado. Exige **R2API.Dot** (DotAPI), que será a **primeira dependência do R2API**
  (justificativa registrada em `src/JetProject/context.md` quando entrar).
- **Rasante:** detecção de colisão iminente com o chão via raycast na direção do impulso; transição para um estado
  `StrafingRun` com curva para cima.
