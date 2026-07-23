# Player360 — Buga Player 360

[![CI](https://github.com/bugaboostudio/Player360/actions/workflows/ci.yml/badge.svg)](https://github.com/bugaboostudio/Player360/actions/workflows/ci.yml)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-6000.0.32f1-black.svg?logo=unity)](https://unity.com)

Player de vídeos imersivos **360° e 180° (mono e estéreo)** desenvolvido em Unity, com foco em headsets **Meta Quest**. Criado originalmente pela **[Bugaboo Studio](https://bugaboostudio.com)** e mantido pela **[XRBR](https://xrbr.com.br)** — Associação Brasileira de Realidade Estendida. O aplicativo carrega automaticamente vídeos armazenados no dispositivo e os exibe como skybox imersivo, sem necessidade de interação do usuário.

## Visão geral

O projeto funciona como um player "kiosk": ao iniciar, ele procura o primeiro vídeo `.mp4` na pasta local do dispositivo e o reproduz no formato configurado. É ideal para ativações, showrooms e experiências guiadas em VR, onde o headset é entregue ao usuário já com o conteúdo rodando.

### Principais recursos

- **Suporte a 4 formatos de vídeo imersivo:**
  - 360° Mono
  - 360° Estéreo (over-under)
  - 180° Mono
  - 180° Estéreo (side-by-side)
- **Detecção automática do formato pelo nome do arquivo** (sufixos `360M`, `360S`, `180M`, `180S`) — um único APK atende todos os formatos.
- **Playlist com loop**: reproduz todos os vídeos da pasta em ordem alfabética; com um vídeo só, faz loop contínuo.
- **Renderização via skybox** com o shader nativo `Skybox/Panoramic`, configurado em runtime.
- **Fade de entrada/saída** (imagem e áudio) no início, no fim e na troca de vídeos.
- **Reinício automático por HMD**: ao tirar o headset por mais de 3 segundos e recolocá-lo, a experiência reinicia do zero e **reescaneia a pasta** — dá para trocar o vídeo via USB sem fechar o app.
- **Configuração por `config.json`** na própria pasta de vídeos (volume, loop, fade, formato) — sem rebuild.
- **Tela de aviso** quando não há vídeo na pasta ou o arquivo não pode ser reproduzido.
- **Log de uso** em `usage_log.txt` na pasta de vídeos: sessões (colocar/tirar o headset), duração, vídeos carregados e erros.

## Requisitos

| Item | Versão |
|---|---|
| Unity | 6000.0.32f1 (Unity 6) |
| Meta XR SDK (`com.meta.xr.sdk.all`) | 71.0.0 |
| XR Plugin Management | 4.5.0 |
| Oculus XR Plugin | 4.4.0 |
| Input System | 1.11.2 |
| Plataforma alvo | Android (Meta Quest) |

Identificador do aplicativo Android: `com.BugabooStudio.BugaPlayer180Stereo`

## Estrutura do projeto

```
Assets/
├── _Core/
│   ├── Scenes/
│   │   └── 360Play.unity          # Cena principal do player
│   ├── Scripts/
│   │   ├── BugaVideoPlayer.cs     # Playlist, detecção de formato, skybox e fades
│   │   ├── HMDController.cs       # Reinício/rescan ao recolocar o headset + log de sessão
│   │   ├── PlayerConfig.cs        # Leitura/criação do config.json da pasta de vídeos
│   │   ├── UsageLogger.cs         # Log de uso em usage_log.txt
│   │   └── FallbackMessage.cs     # Mensagem de aviso criada em runtime
│   ├── Materials/                 # Materiais de skybox (360/180, mono/estéreo)
│   ├── Editor/
│   │   └── CreateDirectoryStructure.cs  # Utilitário para criar a estrutura de pastas
│   └── UI/                        # Ícones e identidade visual
├── Oculus/                        # Integração Meta/Oculus
├── XR/                            # Configurações de XR (loaders e settings)
└── Scenes/
    └── SampleScene.unity
```

## Como funciona

### `BugaVideoPlayer.cs`

1. No `Start()`, cria (se necessário) a pasta `Application.persistentDataPath/360Videos` no dispositivo, carrega o `config.json` (criando um modelo se não existir) e inicializa o log de uso.
2. Escaneia a pasta e monta a **playlist** com todos os vídeos suportados (`.mp4`, `.m4v`, `.webm`, `.mkv`), em ordem alfabética.
3. Para cada vídeo, detecta o formato (config &gt; nome do arquivo &gt; padrão do Inspector) e configura o skybox `Skybox/Panoramic` em runtime (`_ImageType`, `_Layout`).
4. Quando o vídeo está pronto, cria/reaproveita uma `RenderTexture` na resolução do vídeo, aplica ao skybox e inicia a reprodução com **fade de entrada**. Na troca de vídeos há fade de saída/entrada; erros de reprodução pulam para o próximo arquivo.
5. Sem vídeo na pasta (ou se todos falharem), exibe uma **mensagem de aviso** no mundo com o caminho da pasta.

### `HMDController.cs`

Escuta os eventos `HMDMounted`/`HMDUnmounted` do `OVRManager`. Se o usuário ficar **3 segundos ou mais** sem o headset (configurável via `restartSeconds` do `config.json`), ao recolocá-lo o player **reescaneia a pasta e reinicia do primeiro vídeo** — o próximo usuário sempre começa do início, e vídeos trocados via USB entram sem fechar o app. Cada sessão (colocar/tirar o headset) é registrada no `usage_log.txt` com duração.

### Convenção de nomes dos arquivos

O formato de projeção é detectado pelo nome do arquivo (sem diferenciar maiúsculas):

| Sufixo no nome | Formato |
|---|---|
| `360M` ou apenas `360` | 360° Mono |
| `360S` | 360° Estéreo (over-under) |
| `180M` ou apenas `180` | 180° Mono |
| `180S` | 180° Estéreo (side-by-side) |

Exemplos: `tour_virtual_360M.mp4`, `showroom_180S.mp4`. Sem sufixo, vale o `videoType` do `config.json` ou o padrão configurado no Inspector.

### Configuração via `config.json`

Na primeira execução, o app cria um `config.json` dentro da pasta `360Videos`. Edite-o pelo mesmo USB usado para trocar os vídeos — nenhum rebuild é necessário:

```json
{
    "volume": 1.0,
    "loop": true,
    "restartSeconds": 3.0,
    "fadeSeconds": 0.5,
    "videoType": ""
}
```

| Campo | Descrição |
|---|---|
| `volume` | Volume do áudio (0 a 1) |
| `loop` | Repete o vídeo único / recomeça a playlist ao terminar |
| `restartSeconds` | Segundos sem o headset para reiniciar ao recolocar |
| `fadeSeconds` | Duração do fade de entrada/saída |
| `videoType` | Força o formato (`Mono360`, `Stereo360`, `Mono180`, `Stereo180`); vazio = automático |

### Log de uso

O arquivo `usage_log.txt` (na pasta `360Videos`) registra em cada linha `data hora;evento;detalhes`: início do app, vídeos carregados/reproduzidos, início e fim de sessão com duração (`session_start` / `session_end;duration=...`), pasta vazia e erros. Útil para relatar ao cliente quantas pessoas usaram a experiência num evento.

## Como usar

**Passo 1:** Instale o app no Oculus (Meta Quest).

**Passo 2:** Abra o app uma vez, para ele criar uma pasta chamada `360Videos`.

**Passo 3:** Coloque o vídeo 360 Mono na pasta:

```
/sdcard/Android/data/com.BugabooStudio.BugaPlayer360Mono/files/360Videos
```

Você pode copiar o vídeo via cabo USB (explorador de arquivos ou ADB):

```bash
adb push meu_video.mp4 /sdcard/Android/data/com.BugabooStudio.BugaPlayer360Mono/files/360Videos/
```

> O player reproduz **todos os vídeos da pasta em ordem alfabética** (playlist). Com um único arquivo, ele fica em loop. Para trocar o vídeo com o app aberto, basta substituir o arquivo e tirar/recolocar o headset — a pasta é reescaneada automaticamente.
>
> **Observação:** o caminho da pasta varia conforme o identificador do build (ex.: `com.BugabooStudio.BugaPlayer180Stereo` para a variante 180 Estéreo).

### Para desenvolvedores

#### Configurar a cena

1. Abra a cena `Assets/_Core/Scenes/360Play.unity`.
2. No componente `BugaVideoPlayer`, selecione o **tipo de vídeo** (`Mono360`, `Stereo360`, `Mono180` ou `Stereo180`).
3. Confirme que os quatro materiais de skybox e o `VideoPlayer` estão atribuídos no Inspector.

#### Build para o Quest

1. Em **File > Build Settings**, selecione a plataforma **Android**.
2. Certifique-se de que o **Oculus** está habilitado em **Project Settings > XR Plug-in Management**.
3. Gere o APK e instale no headset (via cabo USB com ADB, ou Meta Quest Developer Hub).

### Formatos de vídeo suportados

| Formato | Layout esperado |
|---|---|
| 360° Mono | Equiretangular 360° |
| 360° Estéreo | Equiretangular 360°, olhos empilhados (top/bottom) |
| 180° Mono | Equiretangular 180° |
| 180° Estéreo | Equiretangular 180°, lado a lado (side-by-side) |

## CI/CD (GameCI + GitHub Actions)

O projeto usa [GameCI](https://game.ci/) para testes e builds automatizados no GitHub Actions. Os workflows ficam em `.github/workflows/`:

| Workflow | Gatilho | O que faz |
|---|---|---|
| `ci.yml` | Push/PR na `main` (ou manual) | Roda os testes EditMode e, se passarem, gera o APK Android e publica como artefato (retenção de 14 dias) |
| `release.yml` | Push de tag `v*` (ex.: `v1.2.0`) | Gera o APK com a versão da tag e cria uma **GitHub Release** com o APK anexado e release notes automáticas |
| `activation.yml` | Manual (aba Actions) | Gera o arquivo `.alf` para ativar a licença Unity no CI (usado uma única vez) |

### Configuração inicial (uma vez)

O CI precisa de uma licença Unity ativada. Para **licença Personal**:

1. Na aba **Actions**, rode o workflow **"Acquire activation file"** e baixe o artefato `.alf`.
2. Acesse [license.unity3d.com/manual](https://license.unity3d.com/manual), envie o `.alf` e baixe o `.ulf`.
3. Em **Settings > Secrets and variables > Actions**, crie os secrets:
   - `UNITY_LICENSE` — conteúdo completo do arquivo `.ulf`
   - `UNITY_EMAIL` — e-mail da conta Unity
   - `UNITY_PASSWORD` — senha da conta Unity

Para **licença Pro/Plus**, pule o workflow de ativação e configure `UNITY_SERIAL`, `UNITY_EMAIL` e `UNITY_PASSWORD` (ajustando os workflows para passar `UNITY_SERIAL` no lugar de `UNITY_LICENSE`).

### Publicar uma versão

```bash
git tag v1.0.0
git push origin v1.0.0
```

O workflow de release gera o APK (`BugaPlayer360-v1.0.0.apk`) e cria a release automaticamente.

### Testes

Os testes ficam em `Assets/Tests/EditMode` (assembly `EditModeTests`) e rodam no CI e localmente via **Window > General > Test Runner**. Os testes de fumaça atuais validam a configuração mínima de build (cena no Build Settings, identificador Android e Product Name). Novos testes EditMode devem ser adicionados nessa pasta.

> **Nota:** testes PlayMode não rodam no CI por enquanto — o Meta XR SDK não inicializa em modo headless. O build Android usa IL2CPP + ARM64, conforme exigido pela Meta Store.

## Utilitário de editor

O menu **Assets > Create > Directory Structure** cria a estrutura padrão de pastas do projeto dentro de `Assets/_Core` (Scenes, Scripts, Materials, Prefabs, etc.).

## Contribuindo

Contribuições são bem-vindas! Leia o [CONTRIBUTING.md](CONTRIBUTING.md) para saber como preparar o ambiente, rodar os testes e abrir pull requests. Ao participar, você concorda com o nosso [Código de Conduta](CODE_OF_CONDUCT.md). Para reportar vulnerabilidades, veja a [política de segurança](SECURITY.md).

## Licença

Este projeto é licenciado sob a [Licença Apache 2.0](LICENSE) — veja também o arquivo [NOTICE](NOTICE).

> **Marca:** a licença cobre o código-fonte e os assets deste repositório, mas **não** concede direitos sobre o nome e a marca "Bugaboo Studio", que permanecem propriedade da Bugaboo Studio.

## Créditos

Criado originalmente por [Bugaboo Studio](https://bugaboostudio.com) — especialistas em VR, RA e 3D em tempo real.

Mantido pela [XRBR — Associação Brasileira de Realidade Estendida](https://xrbr.com.br), associação setorial sem fins lucrativos que reúne empresas e profissionais de XR no Brasil.
