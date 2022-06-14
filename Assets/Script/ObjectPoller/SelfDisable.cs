using UnityEngine;

public class SelfDisable : MonoBehaviour
{
    private void Start() {
        this.gameObject.SetActive(false);
    }
}
