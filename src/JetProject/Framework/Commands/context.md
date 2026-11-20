# Submódulo: Framework/Commands

Comandos de desenvolvimento pelo **chat do jogo**, com prefixo `.`. ← [Framework](../context.md)

## Uso no jogo
Digite no chat (Enter): `.help` lista os comandos. Ex.: `.chestluck 0.5`.
A mensagem do comando não aparece para os outros; a resposta aparece no chat como `[JetProject] ...`.

## Como funciona
- `ChatCommandsModule` aplica um **Harmony prefix** em `Chat.SendBroadcastChat(ChatMessageBase, int)` (no servidor).
  Toda mensagem de jogador passa por ali (o comando `say` do cliente é retransmitido pelo servidor).
  Mensagens `UserChatMessage` que começam com `.` são tratadas e engolidas.
- `ChatCommandRegistry` (serviço): `Register(nome, uso, handler)`. O handler recebe um `ChatCommandContext`
  (`Args`, `User`, `Master`, `Body`, `SenderName`, `IntArg(i, padrão)`) e devolve a resposta.
- `ItemCommands.RegisterGive(registry, nome, nomeExibido, () => ItemDef)`: comando ".nome [qtd]" que dá o item a quem
  digitou (negativo retira). O ItemDef é passado como **função** porque os itens do jogo só existem depois que o
  conteúdo carrega (depois do registro dos comandos).
- Executa **no host**. Em multiplayer, qualquer jogador pode digitar (ambiente de teste); itens vão para quem digitou.

## Regras
- Comandos de cada survivor ficam no módulo dele (ex.: `Characters/Jet/JetDevCommands.cs`), não aqui.
- Comandos que mudam balanceamento alteram o `ConfigEntry` (o valor é salvo no `.cfg`).
- Números: use `ChatCommandRegistry.ParseFloat` (aceita `0.5` e `0,5`).
- Antes de publicar o mod, decidir se os comandos ficam (talvez atrás de uma opção de config).
