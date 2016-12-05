using UnityEngine;
using System.Collections;

public class KeyScript : MonoBehaviour {
    [SerializeField]
    private float rotateSpeed = 1.0f;

    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag == "Player")
        {
            PickUp();
        }
    }
	
    private IEnumerator Spin(){
        while(true){
            transform.Rotate(transform.up, (Time.deltaTime * rotateSpeed) * 360);
            yield return 0;
        }
    }

    private void PickUp(){
        GameManager.instance.NumKeys++;
        Destroy(this.gameObject);
    }

    void Start () {
        StartCoroutine(Spin());
    }
	void Update () {
	
	}
}
