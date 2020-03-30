using System.ComponentModel;


namespace Envios.Especiais.Domain.Enums
{
    public enum StatusLog
    {
        [Description("ENVIANDO EMAIL")]
        ENVIANDO,
        [Description("INSERINDO LOG")]
        INSERINDOLOG,
        [Description("ATUALIZANDO A BASE")]
        ATUALIZANDOBASE,
        [Description("ENVIADO")]
        ENVIADO,
        [Description("NÃO ENVIADO")]
        NAOENVIADO,
        [Description("ERRO")]
        ERRO,
        [Description("REENVIANDO EMAIL")]
        REENVIANDO,
        [Description("EMAIL RENVIADO")]
        REENVIADO,
    }
}
