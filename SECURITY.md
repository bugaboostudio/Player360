# Política de Segurança

## Versões suportadas

Apenas a versão mais recente publicada nas [releases](https://github.com/bugaboostudio/Player360/releases) recebe correções de segurança.

## Reportando uma vulnerabilidade

Se você encontrar uma vulnerabilidade de segurança, **não abra uma issue pública**. Em vez disso:

1. Use o recurso de [divulgação privada do GitHub](https://github.com/bugaboostudio/Player360/security/advisories/new) (**Security > Report a vulnerability**), que mantém o relato visível apenas para os mantenedores.
2. Descreva o problema com o máximo de detalhes possível: passos para reproduzir, versão/commit afetado e impacto estimado.

Responderemos o mais rápido possível, normalmente em até 7 dias. Pedimos que você não divulgue a vulnerabilidade publicamente até que uma correção esteja disponível.

## Escopo

O Player360 é um aplicativo offline que reproduz vídeos locais do dispositivo. Ainda assim, consideramos relevantes, por exemplo:

- Falhas no parsing do `config.json` ou de nomes de arquivo que causem comportamento inesperado
- Escrita de logs (`usage_log.txt`) fora da pasta esperada
- Problemas na cadeia de build/CI (workflows do GitHub Actions)
