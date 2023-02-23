using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionInformer : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject inputPrompt;
    public GameObject lockGameObject;
    private TextMeshPro inputPromptText;

    void Start()
    {
        gameObject.SetActive(false);
        inputPromptText = inputPrompt.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    public void Show(Vector2 position, string text)
    {

        inputPromptText.text = text;
        transform.position = position;
        gameObject.SetActive(true);
        inputPrompt.SetActive(true);
        lockGameObject.SetActive(false);

    }

    public void ShowLock(Vector2 position)
    {
        transform.position = position;
        inputPrompt.SetActive(false);
        gameObject.SetActive(true);
        lockGameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
