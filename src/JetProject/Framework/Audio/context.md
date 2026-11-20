# Submódulo: Framework/Audio

Carrega os soundbanks Wwise do mod. ← [Framework](../context.md) · Projeto Wwise: [wwise/context.md](../../../../wwise/context.md)

## Como funciona
`AudioModule` registra no Wwise do jogo cada `.bnk` de `Resources/SoundBanks` via `R2API.SoundAPI.SoundBanks.Add`.
Depois disso, os eventos tocam com o método nativo do jogo:
```csharp
Util.PlaySound(JetSounds.HandCannonFire, gameObject);   // "Play_Jet_HandCannon_Fire"
```

## Sons que não podem ser cortados
O Wwise corta o som quando o GameObject emissor é destruído (ex.: projétil no impacto).
`DetachedSound.Play(evento, posição, lifetime, layers)` toca num emissor próprio que vive `lifetime` s.
`layers` > 1 posta o evento várias vezes juntas: é o jeito de **aumentar o volume de um som vanilla** (2 ≈ +6 dB).

## Sons decididos só no servidor
`NetworkSounds.Create(content, "Play_...")` registra um `NetworkSoundEventDef`; `NetworkSounds.PlayAt(def, posição)`
toca para todos os clientes (via `PointSoundManager.EmitSoundServer`). Use quando só o servidor sabe que o som deve tocar
(ex.: o baú caiu).

## Regras
- Código que já roda em **todos os clientes** (OnEnter de EntityState, Start de projétil) toca com `Util.PlaySound`.
  Código só do servidor usa `NetworkSounds`.
- Eventos vanilla podem ser reaproveitados pelo nome (lista: `.txt` dos soundbanks em `StreamingAssets/Audio/GeneratedSoundBanks/Windows`).
- Sons de explosão de projétil vêm do prefab do efeito (`explosionEffect`); `explosionSoundString` é obsoleto e não toca em rede.
- Nomes de eventos ficam em constantes por survivor (`<Survivor>Sounds.cs`), nunca strings soltas nos estados.
- O `.bnk` é artefato gerado por `tools/audio/build-soundbank.ps1`. Não edite à mão.

## Histórico
A primeira tentativa (2026-11-20) tocava WAVs pelo áudio do Unity. Foi removida porque o RoR2 desativa o áudio do Unity.
