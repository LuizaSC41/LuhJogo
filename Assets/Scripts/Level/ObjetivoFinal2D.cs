using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Representa o final da fase.
/// Quando o jogador toca neste objeto, a fase é considerada concluída.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ObjetivoFinal2D : MonoBehaviour
{
    [SerializeField]
    private string tagDoJogador = "Player";

    private void Reset()
    {
        Collider2D colisor = GetComponent<Collider2D>();
        colisor.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagDoJogador))
        {
            return;
        }

        if (GerenciadorJogo.Instancia != null)
        {
            GerenciadorJogo.Instancia.VencerFase();
        }
        else
        {
            Debug.Log("Vitória! Mas não existe GerenciadorJogo na cena.");
        }
    }

    
}