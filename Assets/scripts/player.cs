using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class player : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float vel, impulso;
    SpriteRenderer sr;
    Animator plyer_anim;
    Rigidbody2D rb;
    string animacao_atual;
    bool flip = false;
    bool jump = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        plyer_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new(0,0,0);
        Vector3 pos_passada = transform.position;
        if(Input.GetKeyDown(KeyCode.Space)){
            rb.AddForce(transform.up * impulso, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            move.x += 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            move.x -= 1;
        }
        transform.position += (Time.deltaTime * vel) * move;
        animar(pos_passada);
    }

    private void animar(Vector3 pos_passada){

        if (pos_passada != transform.position){
            
            if (transform.position.y > pos_passada.y){
                animacao("J_anim");
            } else if (transform.position.y < pos_passada.y){
                animacao("Caindo_anim");
            } else{
                animacao("C_anim");
            }

            if (transform.position.x > pos_passada.x){
                if(flip){
                    flip = false;
                    sr.flipX = false;
                }
            } else{
                if(!flip){
                    flip = true;
                    sr.flipX = true;
                }
            }
        }else {
            animacao("P_anim");
        }
    }

    private void animacao(string N_animacao){

        if(animacao_atual == N_animacao){
            return;
        }

        plyer_anim.Play(N_animacao);

        animacao_atual = N_animacao;
    }
}
