using UnityEngine;
using UnityEngine.UI;

public class ResultListItem : MonoBehaviour
{

    [SerializeField] Text Label;

    public string Text
    {
        get
        {
            return Label.text;
        }
        set
        {
            Label.text = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
    }
}
