using UnityEngine;

/// <summary>
/// Controlador de personagem para jogo de plataforma 2D.
///
/// Este script usa Rigidbody2D e Unity Input System através do
/// InputHandlerPlataforma.
///
/// Recursos implementados:
/// - movimento lateral;
/// - pulo;
/// - detecção de chão;
/// - coyote time;
/// - jump buffer;
/// - pulo variável;
/// - inversão visual do personagem ao mudar de direção.
///
/// Para funcionar:
/// - este script deve estar no objeto Jogador;
/// - o Jogador precisa ter Rigidbody2D;
/// - o Jogador precisa ter Collider2D;
/// - deve existir um objeto Gerenciador_Input na cena;
/// - o campo Ponto De Verificacao Do Chao precisa ser configurado;
/// - a Layer do chão precisa estar marcada no campo Camada Do Chao.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ControladorPlataforma2D : MonoBehaviour
{
    [Header("Movimento lateral")]
    [SerializeField]
    [Tooltip("Velocidade horizontal máxima do personagem.")]
    private float velocidadeMovimento = 7f;

    [SerializeField]
    [Tooltip("Suavização da aceleração. Valores menores deixam mais responsivo.")]
    private float suavizacaoMovimento = 0.05f;

    [Header("Pulo")]
    [SerializeField]
    [Tooltip("Força vertical aplicada quando o personagem pula.")]
    private float forcaPulo = 13f;

    [SerializeField]
    [Tooltip("Multiplicador aplicado quando o jogador solta o botão de pulo antes do ápice.")]
    private float multiplicadorCortePulo = 0.5f;

    [Header("Tolerâncias de jogabilidade")]
    [SerializeField]
    [Tooltip("Tempo em segundos em que ainda é possível pular após sair do chão.")]
    private float tempoCoyote = 0.12f;

    [SerializeField]
    [Tooltip("Tempo em segundos em que o comando de pulo fica guardado antes de tocar no chão.")]
    private float tempoBufferPulo = 0.12f;

    [Header("Verificação de chão")]
    [SerializeField]
    [Tooltip("Ponto usado para verificar se o personagem está tocando o chão.")]
    private Transform pontoDeVerificacaoDoChao;

    [SerializeField]
    [Tooltip("Raio da verificação de chão. Valores pequenos evitam detecções exageradas.")]
    private float raioVerificacaoChao = 0.18f;

    [SerializeField]
    [Tooltip("Layer que representa chão e plataformas.")]
    private LayerMask camadaDoChao;

    [Header("Visual")]
    [SerializeField]
    [Tooltip("Se verdadeiro, inverte o personagem de acordo com a direção do movimento.")]
    private bool inverterVisualAoMover = true;

    private Rigidbody2D _rb;
    private InputHandlerPlataforma _input;

    private bool _estaNoChao;
    public bool EstaNoChao => _estaNoChao;
    private float _tempoDesdeUltimoChao;
    private float _tempoDesdeUltimoPuloApertado;
    private float _velocidadeSuavizacao;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = InputHandlerPlataforma.Instancia;

        // Segurança para evitar que o personagem gire ao colidir.
        _rb.freezeRotation = true;
    }

    private void Start()
    {
        // Caso o InputHandler ainda não estivesse pronto no Awake,
        // tentamos buscar novamente no Start.
        if (_input == null)
        {
            _input = InputHandlerPlataforma.Instancia;
        }

        if (pontoDeVerificacaoDoChao == null)
        {
            Debug.LogWarning("ControladorPlataforma2D: configure o Ponto De Verificacao Do Chao no Inspector.");
        }
    }

    private void OnEnable()
    {
        InputHandlerPlataforma.OnPuloIniciado += RegistrarPuloApertado;
        InputHandlerPlataforma.OnPuloFinalizado += CortarPuloSeNecessario;
    }

    private void OnDisable()
    {
        InputHandlerPlataforma.OnPuloIniciado -= RegistrarPuloApertado;
        InputHandlerPlataforma.OnPuloFinalizado -= CortarPuloSeNecessario;
    }

    private void Update()
    {
        VerificarChao();
        AtualizarTemporizadores();
        AtualizarDirecaoVisual();
    }

    private void FixedUpdate()
    {
        AplicarMovimentoHorizontal();
        TentarExecutarPulo();
    }

    /// <summary>
    /// Guarda o momento em que o jogador apertou o botão de pulo.
    /// Isso permite implementar jump buffer.
    /// </summary>
    private void RegistrarPuloApertado()
    {
        _tempoDesdeUltimoPuloApertado = 0f;
    }

    /// <summary>
    /// Se o jogador soltar o botão de pulo enquanto ainda está subindo,
    /// reduzimos a velocidade vertical.
    ///
    /// Isso cria um pulo variável:
    /// - segurou mais: pula mais alto;
    /// - soltou rápido: pulo menor.
    /// </summary>
    private void CortarPuloSeNecessario()
    {
        if (_rb.linearVelocity.y > 0f)
        {
            _rb.linearVelocity = new Vector2(
                _rb.linearVelocity.x,
                _rb.linearVelocity.y * multiplicadorCortePulo
            );
        }
    }

    /// <summary>
    /// Verifica se existe chão abaixo do personagem.
    /// Usamos Physics2D.OverlapCircle para procurar colliders na Layer do chão.
    /// </summary>
    private void VerificarChao()
    {
        if (pontoDeVerificacaoDoChao == null)
        {
            _estaNoChao = false;
            return;
        }

        _estaNoChao = Physics2D.OverlapCircle(
            pontoDeVerificacaoDoChao.position,
            raioVerificacaoChao,
            camadaDoChao
        );

        if (_estaNoChao)
        {
            _tempoDesdeUltimoChao = 0f;
        }
    }

    /// <summary>
    /// Atualiza os contadores usados pelo coyote time e pelo jump buffer.
    /// </summary>
    private void AtualizarTemporizadores()
    {
        _tempoDesdeUltimoChao += Time.deltaTime;
        _tempoDesdeUltimoPuloApertado += Time.deltaTime;
    }

    /// <summary>
    /// Aplica o movimento lateral no Rigidbody2D.
    ///
    /// Importante:
    /// Não alteramos a velocidade Y aqui, pois ela pertence ao pulo e à gravidade.
    /// Alteramos apenas a velocidade X.
    /// </summary>
    private void AplicarMovimentoHorizontal()
    {
        if (_input == null)
        {
            return;
        }

        float entradaHorizontal = _input.Movimento.x;
        float velocidadeAlvoX = entradaHorizontal * velocidadeMovimento;

        float novaVelocidadeX = Mathf.SmoothDamp(
            _rb.linearVelocity.x,
            velocidadeAlvoX,
            ref _velocidadeSuavizacao,
            suavizacaoMovimento
        );

        _rb.linearVelocity = new Vector2(novaVelocidadeX, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Tenta executar o pulo considerando:
    /// - se o botão de pulo foi apertado recentemente;
    /// - se o personagem está no chão ou dentro do coyote time.
    /// </summary>
    private void TentarExecutarPulo()
    {
        bool puloFoiSolicitado = _tempoDesdeUltimoPuloApertado <= tempoBufferPulo;
        bool podePular = _estaNoChao || _tempoDesdeUltimoChao <= tempoCoyote;

        if (puloFoiSolicitado && podePular)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, forcaPulo);

            // Consumimos o comando de pulo para evitar múltiplos pulos com um único aperto.
            _tempoDesdeUltimoPuloApertado = tempoBufferPulo + 1f;

            // Também consumimos o coyote time.
            _tempoDesdeUltimoChao = tempoCoyote + 1f;
        }
    }

    /// <summary>
    /// Inverte visualmente o personagem conforme a direção do movimento.
    /// Isso não altera a física, apenas a escala visual.
    /// </summary>
    private void AtualizarDirecaoVisual()
    {
        if (!inverterVisualAoMover || _input == null)
        {
            return;
        }

        float direcaoX = _input.Movimento.x;

        if (Mathf.Abs(direcaoX) < 0.01f)
        {
            return;
        }

        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * Mathf.Sign(direcaoX);
        transform.localScale = escala;
    }

    /// <summary>
    /// Desenha no editor o círculo de verificação de chão.
    /// Isso ajuda muito a depurar problemas de pulo.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (pontoDeVerificacaoDoChao == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(pontoDeVerificacaoDoChao.position, raioVerificacaoChao);
    }
}