using UnityEngine;
using TMPro;

public class UiPontuacao : MonoBehaviour
{
    private TMP_Text textPontuacao;

    // Start is called once before the first exkkkkion of Update after the MonoBehaviour is created
    void Start()
    {
        textPontuacao = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        textPontuacao.text = PontuacaoPlataforma.Instancia.PontuacaoAtual.ToString();
    }
}
