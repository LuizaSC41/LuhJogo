using UnityEngine;

/// <summary>
/// Sistema simples de pontuação para o jogo de plataforma.
///
/// Este sistema usa uma instância estática para facilitar o acesso
/// por coletáveis, inimigos e outros scripts.
///
/// Em aula, isso ajuda os estudantes a entenderem comunicação entre scripts.
/// </summary>
public class PontuacaoPlataforma : MonoBehaviour
{
    public static PontuacaoPlataforma Instancia { get; private set; }

    [Header("Pontuação")]
    [SerializeField]
    private int pontuacaoAtual = 0;

    [SerializeField]
    private bool mostrarLogs = true;

    public int PontuacaoAtual => pontuacaoAtual;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
    }

    public void AdicionarPontos(int quantidade)
    {
        if (quantidade <= 0)
        {
            Debug.LogWarning("PontuacaoPlataforma: quantidade precisa ser positiva.");
            return;
        }

        pontuacaoAtual += quantidade;

        if (mostrarLogs)
        {
            Debug.Log("Pontuação atual: " + pontuacaoAtual);
        }
    }

    public void ZerarPontuacao()
    {
        pontuacaoAtual = 0;

        if (mostrarLogs)
        {
            Debug.Log("Pontuação zerada.");
        }
    }
}