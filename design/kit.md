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
| Direção do voo | **WASD** (relativo à câmera), incluindo **voar para trás**. A mira **só gira o personagem**: dá para recuar voando enquanto atira para frente |
| Subida | Com a jetpack ligada, Jet **sempre sobe um pouco** |
| Inércia | A velocidade **acelera** até o alvo (não é instantânea) |
| Impulso inicial | Só na **primeira** ignição depois de sair do chão. Religar no ar **não** dá impulso (evita ganhar distância tocando o espaço várias vezes) |
| Combate | Jet **atira e usa skills durante o voo** |
| Fim | Soltar o espaço **ou** combustível = 0 |
| Religar | Pode religar no mesmo pulo, **quantas vezes quiser, enquanto houver combustível** |
| Recurso | Combustível, com medidor ao lado do personagem (estilo Void Fiend) |
| Recarga | Começa após **~1 s sem voar**; **mesma taxa no chão e no ar** |
| Velocidade | Horizontal (frente/lados) **escala 100% com velocidade de movimento** (Energy Drink, Goat Hoof etc.). A **subida é fixa**: itens **não** a aceleram |
| Hopoo Feather | **Não dá pulos extras.** Cada pena **acelera a recarga** do combustível (+20% por stack, linear) |
| Gasoline | Além do efeito normal (explosão de fogo ao matar), cada stack dá **+15% de capacidade total do tanque** (linear) |

---

## Primária: Hand Cannon / Canhão de Mão *(nome provisório; "Desert Eagle" é marca registrada)*
**Intenção:** tiro pesado e ritmado. Cada disparo importa, e o 3º tiro recompensa alinhar inimigos.

| Aspecto | Definição |
|---|---|
| Tipo | Pistola semiautomática (hitscan), **um pouco lenta**, com **bala grossa** (hitbox maior, sensação de alto calibre) |
| Dano | **350%** por tiro |
| Recuo | **Todo tiro** empurra o Jet para trás (como o tiro sem mira da Railgunner). "Uma deagle de verdade" |
| Coice | A mira/câmera **sobe forte** a cada tiro e volta sozinha (mais forte no 3º tiro) |
| 3º tiro | **Perfura** inimigos, causa **2× 250%** (dois acertos na mesma linha) e tem **recuo maior** |
| Kjaro/Runald | Nenhum acerto da primária chega a 400%, para **não ativar** as Bands (por isso o 3º tiro é 2×250% e não 1×500%) |
| Uso | Chão e ar (inclusive voando) |

> O recuo de tiro (self-knockback) também será usado na **Secundária**.

## Secundária: UES Safe Travels Supplies
**Intenção:** utilidade de área com um "presente" raro, um aceno à nave UES Safe Travels da lore do RoR2.
A chance é baixa de propósito: é um momento de surpresa, não uma fonte de economia.

| Aspecto | Definição |
|---|---|
| Ação | Dispara um **sinalizador em arco** (cai com a gravidade) com a outra mão |
| Recuo no chão | **Intenso**: empurrão para trás em relação à mira |
| Uso no ar: mira | **Segurar** a tecla entra em modo de mira: o Jet **cai bem devagar** (muita resistência à gravidade), o **combustível não recarrega**, o voo da jetpack fica pausado, a mira vira a **do Orbital Probe do Captain** e aparece um **círculo no chão do tamanho do raio da explosão** onde o sinalizador vai cair. Animação provisória: Suppressive Fire do Commando |
| Uso no ar: soltar | Ao **soltar**, o sinalizador sai (em arco, caindo exatamente no ponto mirado) e o Jet faz o **dash do Tactical Dive do Commando** (mesma animação e distância), **sempre para trás**, independente das teclas |
| Acerto direto | O inimigo atingido em cheio leva **2× 300%** (dois acertos, sem ativar Kjaro/Runald) + **Burn** |
| Área | A explosão causa **300%** (um acerto) + **Burn** aos **demais** inimigos no raio (o alvo direto não leva a área) |
| Poça de fogo | Ao cair, **incendeia o chão** como um mini Molotov: **200% de dano por segundo**, **proc coefficient 0,5** (o Burn só é aplicado em ~50% dos ticks) |
| Recarga | **5 s** |
| Projétil | **Um pouco maior** que o firebolt original (1,6×, visual e colisão). Disparo com som de **fogos de artifício** |
| Aviso de entrega | Quando o disparo **mata** (acerto direto, área, **poça de fogo** ou **Burn** aplicado por eles, inclusive stacks novos de Burn depois do primeiro), aparece no local o **feixe de marcação da utilitária do Captain**, com o **som de aviso** dele. Só a **primeira** morte de cada disparo conta |
| Queda do baú | Se o sorteio passar, no fim do feixe acontece o **impacto do Orbital Probe** (poeira, anéis, **tremor de tela**, som de impacto) + **som de queda da entrega do Captain**, e o baú surge **logo após o impacto, exatamente no chão onde o inimigo morreu**. O baú é **pago** (preço normal da fase) |
| Baú | O baú só cai no fim do feixe se o sorteio passar: **0,5%** qualquer tipo de baú (comum, grande, categoria, equipamento, adaptativo, camuflado, lunar, Void se a DLC estiver ativa) e **0,25%** baú **lendário** |
| Múltiplas mortes | Sorteio e feixe **uma vez por disparo**, não por inimigo morto |
| Placeholder | Projétil: firebolt da Artificer. Animação: FMJ do Commando (animação própria do recuo depois). Sons: vanilla (fogos, Captain) |

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
  dedicada "Jetpack" com `Idle`/`JetpackFlight`, separada da "Weapon", por isso atirar voando funciona naturalmente.
  No voo: gravidade desligada, velocidade acelera até `moveVector × velocidade + subida`; o corpo encara a mira.
  Movimento de player é autoritativo no cliente, então funciona em multiplayer sem sincronização extra.
- **Combustível:** componente no body (capacidade = base × bônus de Gasoline; recarga = base × bônus de Hopoo Feather).
  API para a utilitária consumir "todo o restante" e informar quanto foi consumido. Medidor no HUD = submódulo genérico do Framework.
- **Hopoo Feather:** o `CharacterMotor` conta pulos extras pela pena; o estado principal do Jet ignora esses pulos e
  usa a contagem de penas só para a velocidade de recarga do combustível.
- **Chama azul:** um único componente de VFX da jetpack com modos (normal/azul), compartilhado por Utilitária e Especial.
- **Primária:** contador de tiros no próprio componente do survivor (sobrevive entre estados) para o 3º tiro perfurante.
- **Secundária:** o baú é spawnado **no servidor** via `DirectorCore` + `InteractableSpawnCard` do baú sorteado.
- **Barrel Roll:** recast com janela, no padrão de skills de reativação; `DotController` para o Burn (inclusive no Jet).
- **Fogo Azul:** DoT customizado. Exige **R2API.Dot** (DotAPI), que será a **primeira dependência do R2API**
  (justificativa registrada em `src/JetProject/context.md` quando entrar).
- **Rasante:** detecção de colisão iminente com o chão via raycast na direção do impulso; transição para um estado
  `StrafingRun` com curva para cima.
