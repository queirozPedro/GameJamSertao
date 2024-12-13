using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class player : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float vel, impulso;
    private float NTime;
    SpriteRenderer sr;
    Animator plyer_anim;
    private AnimatorStateInfo animStateInfo;
    Rigidbody2D rb;
    string animacao_atual;
    bool flip = false, jump = false, ataque_1 = false, animationFinished;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        plyer_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ataque_1){
            animacao("Ataque_anim");
            animStateInfo = plyer_anim.GetCurrentAnimatorStateInfo(0);
            NTime = animStateInfo.normalizedTime;

            if(NTime > 1.0f) ataque_1 = false;
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
        if (Input.GetKeyDown(KeyCode.RightShift) && !ataque_1){
            ataque_1 = true;
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

    private void animacao(string N_animacao){

        if(animacao_atual == N_animacao){
            return;
        }

        plyer_anim.Play(N_animacao);

        animacao_atual = N_animacao;
    }
}
