# Changelog

Todas as mudanças relevantes deste projeto são documentadas neste arquivo.

O formato segue o [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e o projeto adota [Versionamento Semântico](https://semver.org/lang/pt-BR/).

## [Não lançado]

### Adicionado

- Licença Apache 2.0 e arquivo NOTICE
- Arquivos de comunidade: CONTRIBUTING, CODE_OF_CONDUCT, SECURITY, templates de issue e PR
- Novo ícone do aplicativo e nova splash screen de VR, sem marca registrada

### Alterado

- Assets de identidade visual renomeados (`AppIcon.jpg`, `SplashLogo.png`)

## [1.0.0] — histórico

Versão inicial do player: playlist com loop, detecção de formato pelo nome do arquivo (`360M`, `360S`, `180M`, `180S`), renderização via skybox, fades de imagem/áudio, reinício automático por HMD com rescan da pasta, `config.json`, tela de aviso e log de uso (`usage_log.txt`). CI/CD com GameCI (testes, build Android e releases).
