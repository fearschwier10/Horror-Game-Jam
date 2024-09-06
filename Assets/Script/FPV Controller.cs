using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVController : MonoBehaviour
{
    public float timer;
    int counter;
    public float moveSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {

    }
    //Hello
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        Vector3 movement = Vector3.zero;
        //forward

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movement += new Vector3(0, 0, 1);
        }
        //Left

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movement += new Vector3(-1, 0, 0);
        }
        //Backward
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movement += new Vector3(0, 0, -1);
        }
        //Right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movement += new Vector3(1, 0, 0);
        }

        //Lets move
        transform.Translate(movement * moveSpeed);
    }

    
}
