using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private float writeSpeed = 3f;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(TypeText( "BENVENUTO IN NOME_GIOCO!\nIl tuo compito è quello di aiutare NOME_PROTAGONISTA a superare le prove presenti nelle 3 stanze davanti a lui.", textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            t +=Time.deltaTime;
            t= t*writeSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);
            textLabel.text = textToType.Substring(0,charIndex);

            yield return null;
        }
        
        textLabel.text = textToType;
    }

}
