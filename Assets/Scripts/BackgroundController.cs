using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    public GameObject bgImage;
    public GameObject clouds;
    public GameObject sol;
    private GameObject player;
    public List<GameObject> layers;


    // Start is called before the first frame update
    void Start(){
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {   
        // CloudsMovement();
        bgImageMovement();
        // layersMovement();
    }

    public void CloudsMovement(){
        // As nuvens se movem para a direita
        clouds.transform.position += new Vector3(Time.deltaTime * 0.1f, 0, 0);
    }

    public void bgImageMovement(){
        bgImage.transform.position = player.transform.position;
    }

    public void layersMovement(){
        foreach(GameObject layer in layers){
            layer.transform.position = new Vector3(player.transform.position.x, 0, 0);
        }
    }
}
