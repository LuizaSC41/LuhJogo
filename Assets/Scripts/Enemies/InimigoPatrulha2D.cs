using UnityEngine;

/// <summary>
/// Inimigo simples que patrulha entre dois pontos.
///
/// Este script usa Rigidbody2D kinematic para movimentação controlada.
/// É uma boa introdução para IA simples em jogos de plataforma.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class InimigoPatrulha2D : MonoBehaviour
{
    [Header("Pontos de patrulha")]
    [SerializeField]
    private Transform pontoEsquerda;

    [SerializeField]
    private Transform pontoDireita;

    [Header("Movimento")]
    [SerializeField]
    private float velocidade = 2f;

    private Rigidbody2D _rb;
    private int _direcao = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (pontoEsquerda == null || pontoDireita == null)
        {
            return;
        }

        Vector2 novaPosicao = _rb.position + Vector2.right * _direcao * velocidade * Time.fixedDeltaTime;
        _rb.MovePosition(novaPosicao);

        if (_direcao > 0 && transform.position.x >= pontoDireita.position.x)
        {
            InverterDirecao();
        }
        else if (_direcao < 0 && transform.position.x <= pontoEsquerda.position.x)
        {
            InverterDirecao();
        }
    }

    private void InverterDirecao()
    {
        _direcao *= -1;

        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * _direcao;
        transform.localScale = escala;
    }
}