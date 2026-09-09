using UnityEngine;

/// <summary>
/// Faz a câmera seguir um alvo em um jogo 2D.
///
/// Este script deve ser colocado na Main Camera.
/// O alvo normalmente será o objeto Jogador.
///
/// A câmera mantém sua posição Z original, pois em jogos 2D
/// a câmera costuma ficar em Z = -10 olhando para o plano XY.
/// </summary>
public class CameraSegue2D : MonoBehaviour
{
    [Header("Alvo")]
    [SerializeField]
    [Tooltip("Objeto que a câmera deve seguir. Normalmente é o jogador.")]
    private Transform alvo;

    [Header("Configuração")]
    [SerializeField]
    [Tooltip("Velocidade de suavização da câmera. Valores menores deixam mais suave.")]
    private float suavizacao = 0.15f;

    [SerializeField]
    [Tooltip("Deslocamento da câmera em relação ao alvo.")]
    private Vector3 deslocamento = new Vector3(0f, 1f, -10f);

    [Header("Limites opcionais")]
    [SerializeField]
    [Tooltip("Se verdadeiro, limita a câmera dentro dos valores mínimo e máximo.")]
    private bool usarLimites = false;

    [SerializeField]
    private Vector2 limiteMinimo;

    [SerializeField]
    private Vector2 limiteMaximo;

    private Vector3 _velocidadeAtual;

    private void LateUpdate()
    {
        if (alvo == null)
        {
            return;
        }

        Vector3 posicaoDesejada = alvo.position + deslocamento;

        if (usarLimites)
        {
            posicaoDesejada.x = Mathf.Clamp(posicaoDesejada.x, limiteMinimo.x, limiteMaximo.x);
            posicaoDesejada.y = Mathf.Clamp(posicaoDesejada.y, limiteMinimo.y, limiteMaximo.y);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            posicaoDesejada,
            ref _velocidadeAtual,
            suavizacao
        );
    }
}