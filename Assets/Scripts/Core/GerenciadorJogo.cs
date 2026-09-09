using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerenciador simples para protótipos de plataforma.
///
/// Responsabilidades:
/// - reiniciar a fase;
/// - encerrar a fase com vitória;
/// - responder à ação Reiniciar do Input System.
///
/// Para um projeto didático, este script é suficiente.
/// Em projetos maiores, poderíamos separar melhor estados de jogo,
/// interface, áudio, salvamento e progressão.
/// </summary>
public class GerenciadorJogo : MonoBehaviour
{
    public static GerenciadorJogo Instancia { get; private set; }

    [Header("Configuração")]
    [SerializeField]
    [Tooltip("Se verdadeiro, mostra mensagens no Console para fins didáticos.")]


    private bool mostrarLogs = true;

        [SerializeField]
    private string ProximaFase = "Fase2";

    bool _carregandoCena;

    private bool _jogoFinalizado;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
    }

    private void OnEnable()
    {
        InputHandlerPlataforma.OnReiniciar += ReiniciarFase;
    }

    private void OnDisable()
    {
        InputHandlerPlataforma.OnReiniciar -= ReiniciarFase;
    }

    /// <summary>
    /// Reinicia a cena atual.
    /// </summary>
    public void ReiniciarFase()
    {
        Scene cenaAtual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(cenaAtual.buildIndex);
    }

    /// <summary>
    /// Finaliza a fase com vitória.
    /// Neste guia, vamos apenas registrar no Console.
    /// Depois, podemos trocar por uma tela de vitória.
    /// </summary>
    public void VencerFase()
    {
        if (_jogoFinalizado)
        {
            return;
        }

        _jogoFinalizado = true;

        if (mostrarLogs)
        {
            Debug.Log("Vitória! O jogador concluiu a fase.");
            CarregarCena(ProximaFase);
        }
         
        
    }

    /// <summary>
    /// Finaliza a fase com derrota e reinicia.
    /// </summary>
    private bool CarregarCena(string nomeCena)
    {
        if (_carregandoCena)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(nomeCena))
        {
            Debug.LogError("GerenciadorJogo3D: o nome da cena está vazio.");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(nomeCena))
        {
            Debug.LogError(
                "GerenciadorJogo3D: a cena '" + nomeCena
                + "' não foi encontrada. Confira o nome e o Build Profile."
            );

            return false;
        }

        _carregandoCena = true;

        // Single substitui a cena atual pela nova cena.
        SceneManager.LoadScene(nomeCena, LoadSceneMode.Single);
        Debug.Log("Vitória! Mas não existe GerenciadorJogo na cena.");

        return true;
    }
    public void DerrotarEReiniciar()
    {
        if (mostrarLogs)
        {
            Debug.Log("Derrota! Reiniciando a fase.");
        }

        ReiniciarFase();
    }
}