using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionInformer : MonoBehaviour
{
    // Start is called before the first frame update

    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer iconRenderer;
    private TextMeshPro textMesh;

    void Start()
    {
        backgroundRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
        iconRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
        textMesh = transform.Find("Text").GetComponent<TextMeshPro>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void Show(Vector2 position, string text)
    {
        textMesh.text = text;
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
