using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rainbow effect for provided image
/// </summary>
public class Rainbow : MonoBehaviour {
    private Image image;

    private byte r = 255, g = 0, b = 0;
    void Start() {
        image = GetComponent<Image>();

    }

    // Update is called once per frame
    void Update() {
        if (r == 255 && g == 0 && b < 255) { b++; }
        else if (b == 255 && g == 0 && r > 0) { r--; }
        else if (b == 255 && r == 0 && g < 255) { g++; }
        else if (g == 255 && r == 0 && b > 0) { b--; }
        else if (g == 255 && b == 0 && r < 255) { r++; }
        else if (r == 255 && b == 0 && g > 0) { g--; }

        image.color = new Color32(r, g, b, 255);
    }
}
