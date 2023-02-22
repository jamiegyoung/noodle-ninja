using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionInformer : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshPro inputPromptText;
    public TextMeshPro actionPromptText;

    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void Show(Vector2 position, string text)
    {
        inputPromptText.text = text;
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
