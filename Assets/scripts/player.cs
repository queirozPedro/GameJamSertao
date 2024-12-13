using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class player : MonoBehaviour
{

    [SerializeField] private float vel, impulso;
    SpriteRenderer sr;
    Animator plyer_anim;
    private AnimatorStateInfo animStateInfo;
    Rigidbody2D rb;
    string animacao_atual;
    bool flip = false, jump = false, atacando = false, pro_ataque = false, espada = false;
    private short ataque = 1;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        plyer_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (atacando){
            
            if(ataque == 1){
                animacao("Ataque_anim");
            } else if (ataque == 2){
                animacao("Ataque_2_anim");
            } else{
                animacao("Ataque_3_anim");
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter)){
                pro_ataque = true;
            }

            if (fim_animacao()){

                if (pro_ataque && ataque < 3)
                {
                    ataque++;
                    Debug.Log(ataque);
                    pro_ataque = false;
                } else{
                    atacando = false;
                    pro_ataque = false;
                    ataque = 1;
                }
            }
            
        } else{
            Vector3 pos_passada = transform.position;
            movimentacao();
            animar(pos_passada);
        }
        
    }

    private void movimentacao(){

        Vector3 move = new(0,0,0);
        if (Input.GetKeyDown(KeyCode.Space) && !jump){
            jump = true;
            rb.AddForce(transform.up * impulso, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            move.x += 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            move.x -= 1;
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && !atacando){
            atacando = true;
        }
        transform.position += (Time.deltaTime * vel) * move;
    }

    private void animar(Vector3 pos_passada){

        
        if (transform.position.x > pos_passada.x){
            if(flip){
                flip = false;
                sr.flipX = false;
            }
        } else if (transform.position.x < pos_passada.x){
            if(!flip){
                flip = true;
                sr.flipX = true;
            }
        }

        if(!espada){
            if (rb.velocity.y > 0){
                animacao("J_anim");
                return;
            } else if (rb.velocity.y < 0){
                animacao("Caindo_anim");
                return;
            } else if (transform.position.x != pos_passada.x){
                animacao("C_anim");
                jump = false;
                return;
            }
            jump = false;
            animacao("P_anim");
        } else{
            
            if (rb.velocity.y > 0){
                animacao("J_anim");
                return;
            } else if (rb.velocity.y < 0){
                animacao("Caindo_anim");
                return;
            } else if (transform.position.x != pos_passada.x){
                animacao("C_anim");
                jump = false;
                return;
            }
            jump = false;
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

    private bool fim_animacao() {
        animStateInfo = plyer_anim.GetCurrentAnimatorStateInfo(0);
        return animStateInfo.IsName(animacao_atual) && animStateInfo.normalizedTime >= 1.0f;
    }
}