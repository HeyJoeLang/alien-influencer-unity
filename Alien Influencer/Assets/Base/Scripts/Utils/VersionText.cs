using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class VersionText : MonoBehaviour
{
    void Start()
    {
        GetComponent<TMP_Text>().text = "v" + Application.version;
    }
}
