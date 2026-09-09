using UnityEngine;

/// <summary>
/// Script para itens coletáveis em jogos 2D.
///
/// Exemplo de uso:
/// - moeda;
/// - chave;
/// - estrela;
/// - cristal;
/// - item de missão.
///
/// O objeto precisa ter um Collider2D marcado como Is Trigger.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Coletavel2D : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField]
    [Tooltip("Quantidade de pontos concedida ao coletar este item.")]
    private int pontos = 1;

    [SerializeField]
    [Tooltip("Tag do objeto que pode coletar este item.")]
    private string tagDoJogador = "Player";

    [SerializeField]
    [Tooltip("Se verdadeiro, destrói o coletável após a coleta.")]
    private bool destruirAoColetar = true;

    private void Reset()
    {
        // Reset é chamado quando o componente é adicionado.
        // Ajuda a configurar automaticamente o Collider como Trigger.
        Collider2D colisor = GetComponent<Collider2D>();
        colisor.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagDoJogador))
        {
            return;
        }

        Debug.Log("Coletou item: " + gameObject.name + " | Pontos: " + pontos);

        if (PontuacaoPlataforma.Instancia != null)
        {
            PontuacaoPlataforma.Instancia.AdicionarPontos(pontos);
        }

        if (destruirAoColetar)
        {
            Destroy(gameObject);
        }
    }
}