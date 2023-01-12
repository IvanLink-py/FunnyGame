using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageInfo : MonoBehaviour
{
    [SerializeField] private Text myText;
    [SerializeField] private int timer;
    [SerializeField] private int ttl;
    [SerializeField] private float amount;

    public float Amount
    {
        get => amount;
        set
        {
            amount = value;
            myText.text = Mathf.Round(value).ToString(CultureInfo.CurrentCulture);
        }
    }


    private void FixedUpdate()
    {
        transform.position += new Vector3(Mathf.Sin(timer / 8f) / 20f, 0.07f);

        timer++;
        if (timer >= ttl) UiManager.DamageInfoDisappear(this);
    }
}