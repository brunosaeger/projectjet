# Survivor: Jet

← [Characters](../context.md) · Design: [concept](../../../../design/concept.md) · [kit](../../../../design/kit.md)

## Estrutura
| Arquivo | Papel |
|---|---|
| `JetCharacter.cs` | Monta tudo: body, máquinas, componentes, skills, SurvivorDef |
| `JetConfig.cs` | **Todos** os números de balanceamento (config do BepInEx, editáveis sem recompilar) |
| `JetTokens.cs` | Constantes dos tokens (textos em `Resources/Language/*/Jet.json`) |
| `JetStateMachines.cs` | Nomes das máquinas: `Body`, `Weapon`, `Jetpack` |
| `JetAssets.cs` | Assets do jogo usados pelos estados (tracers), carregados uma vez no registro |
| `JetSounds.cs` | Nomes dos eventos Wwise do Jet (`Play_Jet_...`), definidos em `wwise/sounds.json` |
| `JetDevCommands.cs` | Comandos de chat de teste: `.chestluck`, `.legenchestluck`, `.givegas`, `.givehopoo`, `.giveenergy`, `.givegoat` |
| `Components/JetpackFuel.cs` | Tanque de combustível (segundos de voo); fonte do medidor do HUD |
| `Components/JetpackGroundSmoke.cs` | Visual local: fumaça no chão sob o Jet enquanto voa perto do solo (loop rápido) |
| `Effects/JetpackGroundSmokeEffect.cs` | Cópia da Smokebomb da Bandit: menor, tremor fraco, sem som |
| `Components/JetpackThrusters.cs` | Visual local nos pontos `LeftJet`/`RightJet`: jatos em loop regular + chama em ritmo/tamanho aleatórios (pisca) |
| `Effects/JetpackThrusterEffect.cs` | Placeholder dos propulsores a partir do `CommandoDashJets`, dividido em `Jets` (sem a chama) e `Flame` (só a `Flare`) |
| `Components/SupplyFlareMarker.cs` | Marca projéteis da secundária (sinalizador e poça) e os liga ao `SupplyShot` do disparo |
| `Components/SupplyFlareFireZoneSpawner.cs` | No sinalizador: ao detonar, cria a poça de fogo no chão (mesmo disparo) |
| `Projectiles/SupplyFireZoneProjectile.cs` | Poça de fogo (clone menor da zona do Molotov), 200%/s, proc 0,5 |
| `Systems/SupplyShot.cs` | Um disparo da secundária: sobrevive ao projétil; 1 sorteio de baú |
| `Components/SupplyBeacon.cs` | Marcador de entrega (feixe do Captain): solta o baú no fim, se sorteado |
| `Projectiles/SupplyFlareProjectile.cs` | Sinalizador: arco + explosão em área + Burn (base: firebolt da Artificer) |
| `Projectiles/SupplyBeaconProjectile.cs` | Marcador sem dano (base: Orbital Probe do Captain) |
| `Systems/SupplyDropSystem.cs` | No servidor: morte pelo sinalizador → marcador + sorteio do baú |
| `States/IPausesJetpack.cs` | Marcador: estados que pausam o voo da jetpack enquanto ativos |
| `States/Weapon/SupplyFlareEntry.cs` | Ativação da secundária: chão → `FireSupplyFlare`; ar → `AimSupplyFlare` |
| `States/Weapon/FireSupplyFlare.cs` | Secundária sem mira: dispara na hora; empurrão (ou dash, se `AirAim` desligado e no ar) |
| `States/Weapon/AimSupplyFlare.cs` | Secundária no ar: mira (base `AimThrowableBase` do jogo), queda lenta, sem recarga; solta → tiro + dash |
| `States/Movement/JetDodge.cs` | Dash/rolagem numa direção escolhida pelo código (subclasse do `DodgeState` do Commando, números vanilla copiados do catálogo), com multiplicador de distância, arco opcional (`UpwardSpeed`) e modo 3D (`FollowFullDirection`). Usado pela M2 (3D, oposto à mira), rolagens e quique |
| `States/IJetpackThrustVisual.cs` | Marcador: estado da Body em que a jetpack aparece propulsionando (com chama azul opcional) |
| `States/Utility/BarrelRollEntry.cs` | Utilitária: explosão de saída + escolhe rolagem (A/D/S) ou Ace the Skies (W) |
| `States/Utility/AceTheSkiesBase.cs` | Voo em alta velocidade para frente (mira + A/D), sem outras skills |
| `States/Utility/AceTheSkies.cs` | Fase normal (gasto normal); 1,5 s segurando → queima |
| `States/Utility/AceTheSkiesBurn.cs` | Queima (azul, gasto no ritmo do voo): explosão 1200–3600%, lockout de 10 s, Burn em si, quique no chão |
| `Movement/FlightMotor.cs` | Voo direto no motor (voo/antigravidade, moveDirection, velocidade de cruzeiro), compartilhado por voo e Ace |
| `Components/JetpackVisualState.cs` | Pergunta única dos visuais: propulsionando? chama azul? |
| `States/JetCharacterMain.cs` | Estado principal: pulo no chão normal, pulo no ar liga a jetpack |
| `States/Jetpack/JetpackFlight.cs` | Voo livre guiado pela mira |
| `States/Weapon/FireHandCannon.cs` | Primária: pistola pesada, 3º tiro perfura |
| `Skills/JetSkills.cs` | SkillDefs e famílias |

## Máquinas de estado
| Máquina | Estados | Por quê |
|---|---|---|
| `Body` | `JetCharacterMain` (main), `JetDodge`, `BarrelRollEntry`, `AceTheSkies`, `AceTheSkiesBurn` | Movimento e input de pulo; dashes/voos que controlam a velocidade (fora do main, a Body não processa skills: impede atirar durante rolagens e Ace) |
| `Weapon` | skills (vanilla) | Atirar |
| `Jetpack` | `Idle` ↔ `JetpackFlight` | Separada, para voar **e** atirar ao mesmo tempo |

## Como o voo funciona
1. `JetCharacterMain.ProcessJump`: no ar, com combustível ≥ mínimo → `Jetpack` vai para `JetpackFlight`.
   `WithBoost` só é `true` na primeira ignição desde que tocou o chão (`_boostAvailable`).
2. `JetpackFlight` concede **voo + antigravidade** ao `CharacterMotor`. A cada tick (só na autoridade), a velocidade
   **acelera** (`FlightAcceleration`) até o alvo `WASD × velocidade de voo + subida (ClimbSpeed)`.
   Com boost, a velocidade é aplicada de imediato na ignição e o alvo é multiplicado enquanto o boost decai.
3. `JetCharacterMain.HandleMovements` repassa `MotorMoveDirection` (= velocidade atual / walkSpeed) ao motor,
   para o `CharacterMotor.PreMove` não brigar com a nossa aceleração, e vira o corpo para a **mira**.
4. Soltar o pulo ou zerar o combustível → volta para `Idle` e remove voo/antigravidade. A velocidade é mantida (momentum).
5. **Pausa:** o voo pausa se a máquina `Body` não estiver no `JetCharacterMain` (ex.: dash) **ou** se a `Body`/`Weapon`
   estiver num estado `IPausesJetpack` (ex.: mira da secundária). Pausado, o voo não gasta combustível, não controla
   o movimento (`IsPaused`) e **devolve a gravidade**; retoma sozinho se o pulo continuar pressionado.
6. **Sons:** `JetpackFlight` toca ignição + loop no `OnEnter` e o `Stop_` do loop no `OnExit` (todos os clientes).
   O aviso de combustível baixo vem do evento `JetpackFuel.LowFuelReached` (só na autoridade, então só o dono ouve).
7. **Visuais:** `JetpackFlight.IsActive(máquina)` é a pergunta única "a jetpack está voando?" usada pela fumaça
   de chão e pelos jatos (componentes locais, rodam em todos os clientes).
8. **Recarga bloqueada:** `JetpackFuel.BlockRegen()`/`UnblockRegen()` (contador). Usado pela mira da secundária.

## Estado atual (protótipo)
- ✅ Passiva Jetpack completa (voo, impulso, combustível, recarga, medidor no HUD).
  Itens: Hopoo Feather acelera a recarga (sem pulos extras); Gasoline aumenta a capacidade.
- ✅ Primária (nome provisório "Hand Cannon").
- ✅ Secundária (UES Safe Travels Supplies) com marcador de entrega e baús.
- ✅ Utilitária (DO A BARREL ROLL! // ACE THE SKIES!).
- ⏳ Especial: ainda a do Commando.

## Fluxo do baú (secundária)
1. O sinalizador nasce com um `SupplyShot` novo (no `SupplyFlareMarker`). Ao detonar, o `SupplyFlareFireZoneSpawner`
   cria a poça passando o MESMO shot por `SupplyFlareMarker.PendingShot` (spawn síncrono no servidor).
2. `SupplyDropSystem.OnServerDamageDealt`: todo dano de projétil marcado **marca a vítima** com o shot; na poça,
   sorteia o Burn (chance = proc coefficient) via `BurnHelper` (a poça não usa IgniteOnHit, que sempre aplica).
3. `SupplyDropSystem.OnCharacterDeathServer`: a morte é do disparo se o inflictor é um projétil marcado, **ou** se foi
   dano de Burn numa vítima marcada. Primeira morte do shot → `ChestRolled`, sorteia o baú e dispara o
   `SupplyBeaconProjectile` (payload por `SupplyBeacon.PendingPayload`).
4. O marcador nasce no **chão abaixo** do ponto da morte (`GroundFinder`). O feixe aparece para todos (som de aviso
   no Start) → no fim, `SupplyBeacon` spawna o baú (pago) **exatamente** ali (`InteractableSpawning.TrySpawnAt`, modo
   Direct), com o `impactEffect` original do Orbital Probe (escala 8: poeira, anéis, tremor), os sons
   `Play_captain_shift_impact` + `Play_captain_R_impact` (`NetworkSounds`, porque só o servidor sabe) e aviso no chat.
   Detalhes do prefab original inspecionados com `tools/assets/inspect-bundle.ps1` (ver comentário em `SupplyBeaconProjectile`).

Limitação conhecida: a marcação é "último disparo que atingiu a vítima". Se a vítima também queimar por outra fonte
do mesmo Jet, a morte por Burn ainda conta para o disparo marcado.
- ✅ Sons próprios da primária (Wwise): tiro normal e 3º tiro.
- ⏳ Modelo/animações/VFX: do Commando (clone). Ícones: nenhum.

## Regras específicas
- Números novos entram em `JetConfig` (nunca constantes soltas nos states), com valor padrão anotado em `design/balance.md`.
- Mudou um valor padrão? Incremente `JetConfig.DefaultsRevision`.
- Estados não carregam assets: pegam as referências prontas de `JetAssets`.
- Subclasse de estado vanilla: os campos `[SerializeField]` dele vêm de EntityStateConfiguration e **não** são
  aplicados à subclasse; copie de `EntityStateCatalog.InstantiateState(typeof(Original))` (ver `JetDodge`).
- Animações placeholder só podem ser do **animador do Commando** (o body é clone dele). Animações de outros
  survivors (ex.: dash do Mercenário) não existem nele; virão com o modelo/animador próprio.
- O voo é calculado **só na autoridade** (cliente dono). Não mover isso para o servidor.
