# Jet: Balanceamento

← [design/context.md](context.md) · Valores vivem em `src/JetProject/Characters/Jet/JetConfig.cs`
(e no config do BepInEx: `BepInEx/config/com.jetproject.JetProject.cfg` do perfil, editável sem recompilar).

**Ao mudar um valor padrão no código, incremente `DefaultsRevision` em `JetConfig`.** Senão, quem já tem o `.cfg`
continua com o valor antigo (o BepInEx persiste os valores). A revisão nova reseta o arquivo para os padrões.

## Valores atuais (revisão 5)
| Parâmetro | Valor | Nota |
|---|---|---|
| Velocidade horizontal de voo | 2,2 × velocidade de movimento | Commando anda a 7 m/s → ~15 m/s voando. Usuário aprovou o feeling |
| Subida com jetpack ligada | 10 m/s, **fixa** | Itens de velocidade não afetam a subida, só o horizontal |
| Aceleração do voo | 60 m/s² | ~0,25 s do zero à velocidade de voo |
| Impulso inicial | 1,8×, decai em 0,35 s | Só na 1ª ignição desde que saiu do chão |
| Tanque | 3,75 s × (1 + 15% por Gasoline) | Ex.: 2 Gasoline = 3,75 × 1,3 ≈ 4,9 s |
| Recarga com penas | 50%/s × (1 + 20% por Hopoo Feather) | **20% é provisório** (usuário não definiu). Ex.: 3 penas = 80%/s |
| Mínimo para ligar | 0,1 s de combustível | Evita ligar/desligar em "piscadas" |
| Atraso da recarga | 1 s | |
| Recarga | 50% do tanque/s (chão e ar), base sem itens | |
| Primária: dano | 350% | |
| Primária: intervalo | 0,5 s (escala com velocidade de ataque) | "Um pouco devagar" |
| Primária: reset do combo | 1,5 s sem atirar | Depois disso, a contagem do 3º tiro recomeça |
| Primária: espessura da bala | raio 0,4 m (perfurante: 0,8 m) | Commando usa 0,1 m |
| Primária: recuo de todo tiro | força 600 (~6 m/s no corpo de massa 100) | No chão só empurra na horizontal; no ar, em qualquer direção |
| Primária: recuo do 3º tiro | força 800 (~8 m/s) | |
| Primária: coice da arma (câmera) | 4 (3º tiro: 6,5) | Mira sobe 2,4° a 4° e volta sozinha. Nível do revólver da Bandit |
| Secundária: dano direto | 2× 300%, Burn | Só no inimigo atingido em cheio. Dois acertos < 400%: não ativa Kjaro's/Runald's Band |
| Secundária: dano em área | 300%, Burn | Nos demais inimigos do raio (alvo direto excluído; `AreaHitsDirectTarget` muda isso) |
| Secundária: recarga | 5 s | Decisão do usuário |
| Secundária: raio da explosão | 7 m | Provisório |
| Secundária: velocidade do projétil | 70 m/s, com gravidade | Arco |
| Secundária: tamanho do projétil | 1,6× (visual e colisão) | "Um pouco maior" |
| Secundária: recuo no chão | força 2500 (~25 m/s) | "Bem intenso" |
| Secundária: recuo no ar | Dash para trás do Commando (valores vanilla do Tactical Dive) | `AirBackDodge` liga/desliga (vale para o disparo sem mira) |
| Secundária: mira no ar | Queda máx. 3 m/s; alcance 150 m; sem recarga de combustível | `AirAim` liga/desliga |
| Secundária: poça de fogo | 200%/s, proc 0,5 (= 50% de chance de Burn por tick), 4 s, metade do tamanho da do Molotov | Provisórios: duração e tamanho |
| Secundária: volume do disparo | Som vanilla tocado 2× (≈ +6 dB) | `FireSoundLayers` |
| Primária: volume | +3 dB nos dois tiros (`wwise/sounds.json`) | "Um pouco" mais alto |
| Secundária: chance de baú | 0,5% (qualquer não lendário) + 0,25% (lendário), 1 sorteio por disparo | Ajustável ao vivo: `.chestluck` / `.legenchestluck` |
| Primária: dano do 3º tiro | 2 acertos × 250% (= 500%) | Cada acerto < 400%: não ativa Kjaro's/Runald's Band. Trava no código: máx. 399% por acerto |
| Primária: volume dos tiros | 0 dB (em `wwise/sounds.json`) | Sons próprios via Wwise, no bus Character do jogo |

## Histórico
| Data | Rev. | Mudança | Motivo |
|---|---|---|---|
| 2026-11-19 | 1 | Valores iniciais do protótipo | Primeiro teste jogável |
| 2026-11-20 | 2 | Voo guiado por **WASD** (não mais pela mira), subida constante, aceleração (inércia) | Poder voar para trás atirando para frente |
| 2026-11-20 | 2 | Impulso inicial só na 1ª ignição após sair do chão | Tocar o espaço repetidamente ia mais longe gastando menos que segurar |
| 2026-11-20 | 2 | Recarga no ar = no chão (era 15%/s no ar) | Decisão do usuário |
| 2026-11-20 | 2 | Tanque 3 s → 3,75 s (+25%) | Decisão do usuário |
| 2026-11-20 | 2 | Bala mais grossa (0,15 → 0,4 m; perfurante 0,5 → 0,8 m) + tracers mais pesados | Sensação de alto calibre |
| 2026-11-20 | 2 | 3º tiro empurra o Jet para trás | Decisão do usuário (estilo Railgunner) |
| 2026-11-20 | 3 | Subida 4 → 10 m/s | Usuário: vertical "bem maior", horizontal como está |
| 2026-11-20 | 3 | Recuo 1000 → 600, e sem descolar do chão | Mirando para baixo, o recuo fazia o Jet dar um "pulinho" brusco |
| 2026-11-20 | 4 | Recuo em **todos** os tiros (600); 3º tiro 600 → 800 | "Será realmente uma deagle" |
| 2026-11-20 | 4 | 3º tiro: 350% → 2× 250% | Mais dano sem ativar Kjaro's Band (≥ 400% por acerto) |
| 2026-11-20 | 4 | Som do 3º tiro trocado (`newdeagle.mp3`) | Decisão do usuário |
| 2026-11-20 | 5 | Coice da arma 1,5 → 4 (3º tiro 2,5 → 6,5) | "Recuo da própria arma", sensação de deagle |
| 2026-11-20 | 5 | Secundária implementada (valores acima) | Primeira versão jogável |
| 2026-11-20 | 5 | Secundária: 300% → 2× 300% | Decisão do usuário |
| 2026-11-20 | 5 | Secundária: 2× 300% só no acerto direto; área 300%; baú só por morte no acerto direto | Decisão do usuário |
| 2026-11-20 | 5 | Baú volta a contar mortes pela área (só a 1ª morte de cada disparo) | "Estava muito difícil garantir a morte" |
| 2026-11-20 | 5 | Projétil 1,6× maior; sons: fogos no disparo, aviso e queda do Captain no feixe | Decisão do usuário |
| 2026-11-20 | 5 | No ar, a secundária faz o dash para trás do Commando em vez do empurrão | Decisão do usuário |
| 2026-11-20 | 5 | No ar, a secundária vira mira (segurar) com queda lenta; dispara + dash ao soltar | Decisão do usuário |
| 2026-11-20 | 5 | Queda do baú: impacto completo do Orbital Probe (poeira, tremor, som) e baú exatamente no ponto da morte | Decisão do usuário |
| 2026-11-20 | 5 | Gasoline: +15% de capacidade total do tanque por stack | Decisão do usuário |
| 2026-11-20 | 5 | Hopoo Feather: deixa de dar capacidade (+1 s) e passa a acelerar a recarga (+20%/stack) | Decisão do usuário: pena = recarga, Gasoline = capacidade |
| 2026-11-20 | 5 | Poça de fogo no impacto; mortes por poça e Burn do disparo contam para o baú; baús pagos; volumes +3 dB (M1) e 2× (M2) | Decisão do usuário |
