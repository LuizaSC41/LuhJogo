using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gerenciador central de entrada para um jogo de plataforma 2D.
///
/// Este script usa o Unity Input System e foi pensado para PC/WebGL.
/// Ele lê ações como Movimento, Pular e Reiniciar a partir do arquivo
/// ControlesPlataforma.inputactions.
///
/// Ideia didática:
/// - este script lê o input;
/// - o personagem usa os valores lidos;
/// - outros sistemas podem escutar eventos;
/// - isso evita espalhar leitura de teclado por vários scripts.
///
/// Para funcionar:
/// - deve existir um asset chamado ControlesPlataforma;
/// - a opção Generate C# Class precisa estar marcada;
/// - a classe gerada precisa se chamar ControlesPlataforma;
/// - este script deve estar em um objeto da cena, por exemplo Gerenciador_Input.
/// </summary>
public class InputHandlerPlataforma : MonoBehaviour
{
    /// <summary>
    /// Instância simples para facilitar o acesso por outros scripts.
    ///
    /// Em projetos pequenos e didáticos, isso é aceitável.
    /// Em projetos maiores, poderíamos usar injeção de dependência,
    /// ScriptableObjects ou sistemas de eventos mais robustos.
    /// </summary>
    public static InputHandlerPlataforma Instancia { get; private set; }

    /// <summary>
    /// Direção atual do movimento.
    ///
    /// Em um plataforma 2D, normalmente usamos:
    /// - X negativo para esquerda;
    /// - X positivo para direita;
    /// - Y pode ser útil no futuro, mas neste guia o pulo é uma ação separada.
    /// </summary>
    public Vector2 Movimento { get; private set; }

    /// <summary>
    /// Indica se o botão de pulo está pressionado neste momento.
    /// Isso será usado para controlar pulo variável.
    /// </summary>
    public bool PuloPressionado { get; private set; }

    /// <summary>
    /// Evento chamado quando o jogador aperta o botão de pulo.
    /// Útil para implementar jump buffer.
    /// </summary>
    public static event Action OnPuloIniciado;

    /// <summary>
    /// Evento chamado quando o jogador solta o botão de pulo.
    /// Útil para cortar o pulo e criar pulo variável.
    /// </summary>
    public static event Action OnPuloFinalizado;

    /// <summary>
    /// Evento chamado quando o jogador pede para reiniciar a fase.
    /// </summary>
    public static event Action OnReiniciar;

    /// <summary>
    /// Classe gerada automaticamente pela Unity a partir do asset
    /// ControlesPlataforma.inputactions.
    /// </summary>
    private ControlesPlataforma _controles;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Debug.LogWarning("InputHandlerPlataforma: já existe outro gerenciador de input na cena. Este será destruído.");
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        _controles = new ControlesPlataforma();

        // Movimento é uma ação contínua. Sempre que o valor mudar,
        // guardamos o novo Vector2.
        _controles.Jogador.Movimento.performed += contexto =>
        {
            Movimento = contexto.ReadValue<Vector2>();
        };

        // Quando o movimento for cancelado, significa que o jogador soltou as teclas.
        _controles.Jogador.Movimento.canceled += contexto =>
        {
            Movimento = Vector2.zero;
        };

        // Quando o jogador aperta o botão de pulo.
        _controles.Jogador.Pular.started += contexto =>
        {
            PuloPressionado = true;
            OnPuloIniciado?.Invoke();
        };

        // Quando o jogador solta o botão de pulo.
        _controles.Jogador.Pular.canceled += contexto =>
        {
            PuloPressionado = false;
            OnPuloFinalizado?.Invoke();
        };

        // Reiniciar é uma ação simples de botão.
        _controles.Jogador.Reiniciar.performed += contexto =>
        {
            OnReiniciar?.Invoke();
        };
    }

    private void OnEnable()
    {
        if (_controles != null)
        {
            _controles.Jogador.Enable();
        }
    }

    private void OnDisable()
    {
        if (_controles != null)
        {
            _controles.Jogador.Disable();
        }
    }
}