using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    Collider col;
    Renderer rend;
    Color myCol;
    Color grayCol = new Color(0.5f, 0.5f, 0.5f, 0.5f,);
    // Start is called before the first frame update

    void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        col.enabled = true;
        myCol = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        transfor.Rotate(new Vector3(0.4f, -0.4f, 0.4f,));

    }
    public IEnumerator UseFuel()
    {
        col.enebled = false;
        rend.material.color = grayCol;
        yield return new WaitForSeconds(10);
        col.enebled = true;
        rend.material.color = myCol;
    }
    void OnTriggerEnter(Collider other)
    {
        AddFuel auto = other.GetComponent<AddFuel>();
        if(auto!= null && auto.Add())
        {
            StartCoroutine(UseFuel());
        }
    }
}
