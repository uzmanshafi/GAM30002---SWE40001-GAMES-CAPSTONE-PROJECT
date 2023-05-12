using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelChangeTest : MonoBehaviour
{
    public GameObject newPlayer;
    //public GameObject newFireball;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    graphicalChange();
        //}
    }

    public void graphicalChange()
    {
        PlayerMovement playerMvmnt = GetComponent<PlayerMovement>();
        //FireballController FBController = GetComponent<FireballController>();

        GameObject newP = Instantiate(newPlayer, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);

    }
}
