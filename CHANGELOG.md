# Changelog

Todas as mudanças relevantes deste projeto são documentadas neste arquivo.

O formato segue o [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e o projeto adota [Versionamento Semântico](https://semver.org/lang/pt-BR/).

A cada deploy na `main`, o CI cria automaticamente uma nova tag `vX.Y.Z` (incrementando o patch) e publica uma GitHub Release com o APK anexado.

## [Não lançado]

## [1.0.0] — 2026-07-24

Primeiro release público do projeto.

### Adicionado

- Player 360°/180°: playlist com loop, detecção de formato pelo nome do arquivo (`360M`, `360S`, `180M`, `180S`), renderização via skybox, fades de imagem/áudio, reinício automático por HMD com rescan da pasta, `config.json`, tela de aviso e log de uso (`usage_log.txt`)
- CI/CD com GameCI: testes EditMode, build Android e releases automáticos a cada deploy na `main`
- Licença Apache 2.0 e arquivo NOTICE
- Arquivos de comunidade: CONTRIBUTING, CODE_OF_CONDUCT, SECURITY, templates de issue e PR
- Novo ícone do aplicativo e nova splash screen de VR, sem marca registrada

### Alterado

- Assets de identidade visual renomeados (`AppIcon.jpg`, `SplashLogo.png`)
- Créditos atualizados: projeto criado pela Bugaboo Studio e mantido pela XRBR — Associação Brasileira de Realidade Estendida
