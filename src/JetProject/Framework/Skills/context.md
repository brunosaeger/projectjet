# Submódulo: Framework/Skills

Criação de SkillDefs e SkillFamilies. ← [Framework](../context.md)

## API (`SkillFactory`)
| Método | Uso |
|---|---|
| `CreateSkill<T>(nome, estado, máquina, configure)` | Cria `SkillDef` ou subclasse (`SteppedSkillDef`...) já com estado de ativação e máquina |
| `CreateFamily(nome, params SkillDef[])` | Cria a família com as variantes (o loadout mostra uma opção por variante) |
| `AssignFamily(slot, família)` | Coloca a família num `GenericSkill` do body |

## Outros tipos
| Tipo | Uso |
|---|---|
| `AirborneSkillDef` | SkillDef que só fica pronta **no ar** |
| `SkillLockout.EmptyAndDelay(slot, s)` | Zera as cargas e a primeira só volta após `s` segundos (usa o cronômetro nativo: o ícone mostra o tempo certo) |

## Regras
- A fábrica **não registra** conteúdo. Quem cria registra no `ContentRegistry`: skill, família **e** o EntityState.
- Escolha o tipo de SkillDef que já resolve o problema antes de escrever lógica própria:
  - `SteppedSkillDef`: combos/sequências (o estado recebe o passo via `IStepSetter`).
  - Skills com reativação (recast), quando chegarmos à Utilitária: avaliar os tipos vanilla antes de criar um.
