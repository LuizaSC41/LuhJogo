using UnityEngine;

/// <summary>
/// Área que causa derrota quando o jogador entra nela.
///
/// Uso comum:
/// - buraco abaixo da fase;
/// - lava;
/// - espinhos;
/// - água venenosa;
/// - armadilha.
///
/// O objeto precisa ter Collider2D marcado como Is Trigger.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ZonaDeMorte2D : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Tag do jogador que será afetado por esta zona.")]
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
            GerenciadorJogo.Instancia.DerrotarEReiniciar();
        }
        else
        {
            Debug.LogWarning("ZonaDeMorte2D: GerenciadorJogo não encontrado na cena.");
        }
    }
}