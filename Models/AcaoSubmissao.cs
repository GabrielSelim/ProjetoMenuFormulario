namespace FormEngineAPI.Models
{
    /// <summary>
    /// Ações possíveis para o histórico de submissões
    /// </summary>
    public enum AcaoSubmissao
    {
        /// <summary>
        /// Submissão foi criada
        /// </summary>
        Criado = 0,

        /// <summary>
        /// Submissão foi atualizada
        /// </summary>
        Atualizado = 1,

        /// <summary>
        /// Submissão foi enviada para análise
        /// </summary>
        Enviado = 2,

        /// <summary>
        /// Submissão foi aprovada
        /// </summary>
        Aprovado = 3,

        /// <summary>
        /// Submissão foi rejeitada
        /// </summary>
        Rejeitado = 4,

        /// <summary>
        /// Submissão foi cancelada
        /// </summary>
        Cancelado = 5,

        /// <summary>
        /// Submissão foi excluída
        /// </summary>
        Excluido = 6
    }
}