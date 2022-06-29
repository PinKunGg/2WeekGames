using UnityEngine;

public class SelfDisable : MonoBehaviour
{
    public int time_to_disable = 5;
    private void Start() {
        Invoke("Disable", time_to_disable);
    }

    private void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
