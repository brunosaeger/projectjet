namespace JetProject.Characters.Jet.States
{
    /// <summary>
    /// Marca um estado (em qualquer máquina) que PAUSA o voo da jetpack enquanto estiver ativo:
    /// o voo não gasta combustível, não controla a velocidade e devolve a gravidade ao personagem.
    /// Ao sair do estado, se o pulo ainda estiver pressionado, o voo retoma.
    /// Estados da máquina Body que não são o JetCharacterMain já pausam o voo automaticamente.
    /// </summary>
    public interface IPausesJetpack
    {
    }
}
