# Player360 — Buga Player 360

Player de vídeos imersivos **360° e 180° (mono e estéreo)** desenvolvido em Unity pela **Bugaboo Studio**, com foco em headsets **Meta Quest**. O aplicativo carrega automaticamente vídeos armazenados no dispositivo e os exibe como skybox imersivo, sem necessidade de interação do usuário.

## Visão geral

O projeto funciona como um player "kiosk": ao iniciar, ele procura o primeiro vídeo `.mp4` na pasta local do dispositivo e o reproduz no formato configurado. É ideal para ativações, showrooms e experiências guiadas em VR, onde o headset é entregue ao usuário já com o conteúdo rodando.

### Principais recursos

- **Suporte a 4 formatos de vídeo imersivo:**
  - 360° Mono
  - 360° Estéreo
  - 180° Mono
  - 180° Estéreo
- **Carregamento automático** do primeiro vídeo `.mp4` encontrado na pasta local do dispositivo (`persistentDataPath/360Videos`).
- **Renderização via skybox**: o vídeo é projetado em uma `RenderTexture` aplicada ao material de skybox correspondente ao formato escolhido.
- **Reinício automático por HMD**: ao tirar o headset por mais de 3 segundos e recolocá-lo, a experiência reinicia do zero (recarrega a cena inicial).

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
│   │   ├── BugaVideoPlayer.cs     # Carregamento e reprodução do vídeo no skybox
│   │   └── HMDController.cs       # Reinício da experiência ao recolocar o headset
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

1. No `Start()`, cria (se necessário) a pasta `Application.persistentDataPath/360Videos` no dispositivo.
2. Busca o primeiro arquivo `.mp4` na pasta e o prepara no `VideoPlayer`.
3. Quando o vídeo está pronto, cria uma `RenderTexture` na resolução do vídeo, aplica ao material de skybox correspondente ao `VideoType` selecionado no Inspector e inicia a reprodução.

### `HMDController.cs`

Escuta os eventos `HMDMounted`/`HMDUnmounted` do `OVRManager`. Se o usuário ficar **3 segundos ou mais** sem o headset, a cena inicial é recarregada ao recolocá-lo — garantindo que o próximo usuário sempre comece do início.

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

> O player reproduz o **primeiro** arquivo `.mp4` encontrado na pasta. Ao abrir o app novamente, o vídeo será exibido automaticamente.
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

## Utilitário de editor

O menu **Assets > Create > Directory Structure** cria a estrutura padrão de pastas do projeto dentro de `Assets/_Core` (Scenes, Scripts, Materials, Prefabs, etc.).

## Créditos

Desenvolvido por [Bugaboo Studio](https://bugaboostudio.com) — especialistas em VR, RA e 3D em tempo real.
