# Jet: Conceito

← [design/context.md](context.md)

## Fantasia
Um piloto de jetpack que equilibra o tiroteio clássico do Risk of Rain 2 com **voo livre**.
No chão ele joga como um shooter; no ar ele vira um caça. A decisão central do jogador é
*quando* gastar combustível para reposicionar, perseguir ou fugir.

## Pilar de gameplay: Jetpack (passiva)
- 1º pulo (espaço): pulo normal.
- 2º pulo (espaço no ar): no lugar de um double jump, **ativa a jetpack**.
  1. Dá um **impulso inicial** leve (só na primeira ignição depois de sair do chão).
  2. Entra em **modo de voo**: a direção segue o **WASD** e a jetpack sempre puxa um pouco para cima.
     O mouse **gira o personagem e a mira**, independente da direção do voo: dá para voar para trás atirando para frente.
  3. O voo dura **enquanto o espaço estiver pressionado** e **houver combustível**.
- **Medidor de combustível** sempre visível **ao lado do personagem**, como o medidor de corrupção do Void Fiend.
- Ao soltar o espaço ou zerar o combustível, o voo termina e Jet volta a cair normalmente.
- Jet **atira voando**: o voo é mobilidade **e** combate.

Regras detalhadas em [kit.md](kit.md#passiva-jetpack).

## Visual

### Paleta
| Elemento | Cor |
|---|---|
| Armadura e capacete | **Amarelo queimado** |
| Jaqueta | Couro marrom escuro de aviador, gola e forro em pelúcia bege/creme |
| Detalhes | Cinza (encaixes, parafusos simples) |
| Visor | Escuro, quase preto, com emblema holográfico |
| Chamas da jetpack | A definir (a referência usa ciano; pode combinar com o amarelo via laranja/âmbar) |

### Corpo
- **Armadura espacial de placas**, linguagem de "soldado padrão do RoR2" (proporções e sobriedade do Commando),
  mas com design próprio. Cobre o torso por baixo da jaqueta e toda a parte inferior.
- **Jaqueta de aviador clássica** (estilo B-3: couro, gola larga felpuda, punhos e barra com pelúcia, cinto),
  usada **semiaberta**, mostrando as placas por baixo.

### Jetpack
- Sempre nas costas. Futurista, nada de jetpack de "dois cilindros".
- Duas carcaças metálicas que imitam os **élitros de um besouro** (a casca das asas), com o fogo saindo por baixo.
- Referência local (não versionada): `art/concept/refs/jetpack_beetle_reference.png`.

### Capacete
Esboço: [art/concept/helmet_sketch_side_front_back.png](../art/concept/helmet_sketch_side_front_back.png)
- Inspiração: **elmo alado de valquíria** + um pouco de **Daft Punk**.
- **Visor escuro e grande**, com espaço para o emblema holográfico no centro.
- **Protuberâncias laterais** (asas) que nascem na altura da orelha e se alongam para trás e para cima.
  Na vista frontal, formam um "V" acima da cabeça.
- **Queixo levemente protuberante.**
- Lateral: módulo circular cinza na altura da orelha, com detalhes de encaixes.
- Traseira: placa amarela inteira com frisos horizontais.
- Partes estruturais em amarelo; detalhes em cinza.

### Emblema do visor ✅
Esboço: [art/concept/visor_emblem.png](../art/concept/visor_emblem.png)
- **Três triângulos vazados** (só contorno), dispostos radialmente em volta de um centro vazio: dois em cima, um embaixo.
- Holográfico, **amarelo** sobre o visor escuro.
- Substitui a ideia inicial da triforce (descartada por ser marca da Nintendo).

## Inspirações (gênero, não cópia)
- Voo com jetpack em shooters de terceira pessoa (sensação, não design).
- Jaqueta B-3 de aviação real (peça de vestuário genérica).
- Elmos alados nórdicos (motivo histórico/mitológico).
