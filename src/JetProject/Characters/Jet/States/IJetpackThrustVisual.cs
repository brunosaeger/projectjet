namespace JetProject.Characters.Jet.States
{
    /// <summary>
    /// Estado (na máquina Body) em que a jetpack está visivelmente propulsionando, mesmo fora do voo normal
    /// (ex.: Ace the Skies). Os visuais (jatos, chama, fumaça) leem isto via JetpackVisualState.
    /// O tipo do estado é sincronizado pela rede, então todos os clientes veem igual.
    /// </summary>
    public interface IJetpackThrustVisual
    {
        /// <summary>Chama azul (ex.: queima do Ace the Skies) em vez da vermelha.</summary>
        bool BlueFlame { get; }
    }
}
