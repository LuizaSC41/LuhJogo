using UnityEngine;

/// <summary>
/// Causa derrota quando o jogador encosta neste objeto.
///
/// Pode ser usado em:
/// - inimigos;
/// - espinhos;
/// - lava;
/// - serras;
/// - armadilhas.
/// </summary>
public class DanoAoJogador2D : MonoBehaviour
{
    [SerializeField]
    private string tagDoJogador = "Player";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(tagDoJogador))
        {
            return;
        }

        if (GerenciadorJogo.Instancia != null)
        {
            GerenciadorJogo.Instancia.DerrotarEReiniciar();
        }
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
    }
}