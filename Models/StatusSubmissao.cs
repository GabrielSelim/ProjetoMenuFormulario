namespace FormEngineAPI.Models
{
    /// <summary>
    /// Status possíveis para uma submissão de formulário
    /// </summary>
    public enum StatusSubmissao
    {
        /// <summary>
        /// Submissão em rascunho - ainda não foi enviada
        /// </summary>
        Rascunho = 0,

        /// <summary>
        /// Submissão enviada aguardando análise
        /// </summary>
        Enviado = 1,

        /// <summary>
        /// Submissão em processo de análise
        /// </summary>
        EmAnalise = 2,

        /// <summary>
        /// Submissão aprovada
        /// </summary>
        Aprovado = 3,

        /// <summary>
        /// Submissão rejeitada
        /// </summary>
        Rejeitado = 4,

        /// <summary>
        /// Submissão cancelada
        /// </summary>
        Cancelado = 5
    }
}