# Contribuindo com o Player360

Obrigado pelo interesse em contribuir! Este documento explica como preparar o ambiente, propor mudanças e enviar pull requests.

## Preparando o ambiente

1. Instale o **Unity 6000.0.32f1 (Unity 6)** via Unity Hub, com o módulo **Android Build Support** (inclui SDK/NDK e OpenJDK).
2. Faça um fork do repositório e clone o seu fork:
   ```bash
   git clone https://github.com/<seu-usuario>/Player360.git
   ```
3. Abra a pasta do projeto no Unity Hub. Os pacotes (Meta XR SDK, XR Plugin Management, Input System etc.) são resolvidos automaticamente pelo Package Manager.
4. A cena principal é `Assets/_Core/Scenes/360Play.unity`.

Para detalhes de build e deploy no Meta Quest, veja a [Documentação Técnica](docs/DOCUMENTACAO-TECNICA.md).

## Rodando os testes

Os testes EditMode ficam em `Assets/Tests/EditMode` (assembly `EditModeTests`):

- No Unity: **Window > General > Test Runner > EditMode > Run All**.
- No CI: rodam automaticamente em todo push/PR para a `main` (workflow `ci.yml`).

PRs só são aceitos com os testes passando. Se a sua mudança adicionar comportamento testável fora do runtime XR, inclua testes EditMode novos.

## Fluxo de contribuição

1. Abra uma **issue** antes de começar mudanças grandes, para alinharmos a abordagem.
2. Crie uma branch a partir da `main` com um nome descritivo (ex.: `fix/fade-audio`, `feat/subtitle-support`).
3. Faça commits pequenos com mensagens claras, no imperativo (ex.: "Corrige fade de áudio na troca de vídeo").
4. Abra o pull request preenchendo o template. Descreva **o que** mudou e **por quê**, e como testou (idealmente em um headset Meta Quest).

## Diretrizes de código

- C# seguindo as convenções já usadas nos scripts de `Assets/_Core/Scripts` (nomes em inglês, `PascalCase` para tipos e métodos, `camelCase` para campos).
- Evite adicionar dependências novas sem discutir antes em uma issue.
- Assets binários (texturas, vídeos, etc.) só quando indispensáveis — o repositório é intencionalmente leve. **Nunca** commite vídeos de teste.
- Sempre commite os arquivos `.meta` junto com os assets correspondentes (o projeto usa serialização em texto e meta files visíveis).

## Limitações conhecidas do CI

- Testes **PlayMode não rodam no CI** — o Meta XR SDK não inicializa em modo headless. Mudanças de runtime devem ser validadas manualmente em um headset e descritas no PR.
- Builds no CI dependem de secrets de licença Unity que **não ficam disponíveis em PRs de forks**. Se o job de build não rodar no seu PR, não se preocupe: os mantenedores rodam o build ao revisar.

## Licença das contribuições

Ao contribuir, você concorda que suas contribuições serão licenciadas sob a [Licença Apache 2.0](LICENSE), conforme a seção 5 da licença.
