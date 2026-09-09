using UnityEngine;

/// <summary>
/// Atualiza as animações visuais do personagem de plataforma 2D.
///
/// Este script não movimenta o personagem.
/// Ele apenas observa o Rigidbody2D e o ControladorPlataforma2D
/// para informar ao Animator o estado atual do jogador.
///
/// Responsabilidades:
/// - informar a velocidade horizontal;
/// - informar a velocidade vertical;
/// - informar se o personagem está no chão;
/// - virar o sprite para esquerda ou direita.
///
/// O script deve ser colocado no objeto filho Visual.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimacaoJogador2D : MonoBehaviour
{
    [Header("Referências")]

    [SerializeField]
    [Tooltip("Rigidbody2D localizado no objeto pai Jogador.")]
    private Rigidbody2D rigidbodyDoJogador;

    [SerializeField]
    [Tooltip("ControladorPlataforma2D localizado no objeto pai Jogador.")]
    private ControladorPlataforma2D controladorDoJogador;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    // Identificadores dos parâmetros do Animator.
    //
    // Usar StringToHash evita procurar os parâmetros pelo nome
    // várias vezes durante a execução.
    private static readonly int VelocidadeXHash =
        Animator.StringToHash("VelocidadeX");

    private static readonly int VelocidadeYHash =
        Animator.StringToHash("VelocidadeY");

    private static readonly int EstaNoChaoHash =
        Animator.StringToHash("EstaNoChao");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Se as referências não foram configuradas manualmente,
        // tentamos encontrá-las no objeto pai.
        if (rigidbodyDoJogador == null)
        {
            rigidbodyDoJogador = GetComponentInParent<Rigidbody2D>();
        }

        if (controladorDoJogador == null)
        {
            controladorDoJogador =
                GetComponentInParent<ControladorPlataforma2D>();
        }
    }

    private void Update()
    {
        AtualizarParametrosDoAnimator();
        AtualizarDirecaoDoSprite();
    }

    /// <summary>
    /// Envia ao Animator os dados atuais da física do personagem.
    /// </summary>
    private void AtualizarParametrosDoAnimator()
    {
        if (rigidbodyDoJogador == null || controladorDoJogador == null)
        {
            return;
        }

        // Mathf.Abs transforma valores negativos em positivos.
        //
        // Exemplo:
        // velocidade -5 para esquerda vira 5.
        // velocidade  5 para direita continua 5.
        //
        // Para a animação de corrida, interessa saber a intensidade
        // do movimento, não o lado.
        float velocidadeHorizontal =
            Mathf.Abs(rigidbodyDoJogador.linearVelocity.x);

        float velocidadeVertical =
            rigidbodyDoJogador.linearVelocity.y;

        _animator.SetFloat(
            VelocidadeXHash,
            velocidadeHorizontal
        );

        _animator.SetFloat(
            VelocidadeYHash,
            velocidadeVertical
        );

        _animator.SetBool(
            EstaNoChaoHash,
            controladorDoJogador.EstaNoChao
        );
    }

    /// <summary>
    /// Vira apenas o sprite, sem modificar o Rigidbody2D,
    /// o Collider2D ou o objeto principal do jogador.
    /// </summary>
    private void AtualizarDirecaoDoSprite()
    {
        if (rigidbodyDoJogador == null)
        {
            return;
        }

        float velocidadeX = rigidbodyDoJogador.linearVelocity.x;

        // Usamos uma pequena tolerância para evitar que o sprite
        // fique alternando de lado quando a velocidade estiver
        // muito próxima de zero.
        if (velocidadeX > 0.05f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (velocidadeX < -0.05f)
        {
            _spriteRenderer.flipX = true;
        }
    }
}