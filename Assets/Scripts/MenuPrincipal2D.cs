using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla as ações disponíveis no menu principal.
/// </summary>
public class MenuPrincipal3D : MonoBehaviour
{
    [Header("Cenas")]

    [SerializeField]
    [Tooltip("Primeira fase aberta pelo botão Jogar.")]
    private string primeiraFase = "Fase_01_Plataforma2D";

    // Evita que vários cliques iniciem carregamentos repetidos.
    private bool _carregandoCena;

    /// <summary>
    /// Método público conectado ao botão Jogar.
    /// </summary>
    public void Jogar()
    {
        CarregarCena(primeiraFase);
    }

    /// <summary>
    /// Encerra a aplicação compilada.
    /// No Editor, apenas interrompe o modo Play.
    /// </summary>
    public void SairDoJogo()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Valida e carrega uma cena pelo nome.
    /// </summary>
    private void CarregarCena(string nomeCena)
    {
        if (_carregandoCena)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(nomeCena))
        {
            Debug.LogError("MenuPrincipal2D: o nome da primeira fase está vazio.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nomeCena))
        {
            Debug.LogError(
                "MenuPrincipal2D: a cena '" + nomeCena
                + "' não foi encontrada. Confira o nome e o Build Profile."
            );

            return;
        }

        _carregandoCena = true;
        SceneManager.LoadScene(nomeCena, LoadSceneMode.Single);
    }
}