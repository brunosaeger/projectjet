# Submódulo: Framework/UI

Elementos de HUD genéricos. ← [Framework](../context.md)

## Medidor de recurso
| Tipo | Papel |
|---|---|
| `IResourceMeterSource` | Contrato: `Fraction` (0 a 1), `MeterColor`, `MeterVisible` |
| `ResourceMeterHud` | Componente no **body**: acha o HUD que observa este body e desenha uma barra vertical ao lado do crosshair |

Uso: adicionar ao prefab do body um componente que implemente `IResourceMeterSource` **e** o `ResourceMeterHud`.
Só aparece no HUD de quem está controlando/espectando aquele body (cada jogador vê o seu).

## Regras
- UI construída por código enquanto não há AssetBundle. Quando houver arte de UI, o visual migra para prefab,
  mas o contrato `IResourceMeterSource` continua o mesmo.
- Nada aqui conhece um survivor específico.
- Hoje suporta um medidor por body. Se precisar de mais, evoluir para uma lista de fontes com posições.
