using UnityEngine;

/// <summary>
/// Mensagem de aviso em world-space criada em runtime (sem depender de
/// prefabs ou de objetos na cena). Exibida quando não há vídeo na pasta
/// ou quando ocorre um erro de reprodução.
/// </summary>
public class FallbackMessage
{
    GameObject root;
    TextMesh text;

    public static FallbackMessage Create()
    {
        var msg = new FallbackMessage();
        msg.root = new GameObject("FallbackMessage");
        msg.text = msg.root.AddComponent<TextMesh>();
        msg.text.alignment = TextAlignment.Center;
        msg.text.anchor = TextAnchor.MiddleCenter;
        msg.text.fontSize = 64;
        msg.text.characterSize = 0.02f;
        msg.text.color = Color.white;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null)
        {
            msg.text.font = font;
            msg.root.GetComponent<MeshRenderer>().material = font.material;
        }

        msg.root.SetActive(false);
        return msg;
    }

    public void Show(string message)
    {
        if (root == null) return;
        text.text = message;
        PlaceInFrontOfCamera();
        root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }

    void PlaceInFrontOfCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            root.transform.position = new Vector3(0f, 1.6f, 2.5f);
            return;
        }

        Vector3 forward = cam.transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;
        forward.Normalize();

        root.transform.position = cam.transform.position + forward * 2.5f;
        root.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}
