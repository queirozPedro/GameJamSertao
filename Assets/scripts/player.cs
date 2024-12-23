using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private string animacao_atual;
    private bool flip = false, jump = false, estar_no_chao, atacando = false, prox_ataque = false, deslizar = false;
    private bool espada = false, troca_ataque_espada = false;
    private short ataque = 1;
 
 
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        plyer_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
 
    void Update()
    {
 
        if(deslizar){
            if (fim_animacao()){
                deslizar = false;
            }
        } else if (troca_ataque_espada){
            Trocar_Ataque();
        } else if (atacando){
            atacar();
        } else{
            Vector3 pos_passada = transform.position;
            animar(pos_passada);
            KeyboardControl();
        }
    }
 
    private void KeyboardControl(){
 
        Vector2 move = new(0, rb.velocity.y);
        if (Input.GetKeyDown(KeyCode.W) && !jump){
            jump = true;
            move.y = impulso;
        }
        if (Input.GetKey(KeyCode.D)){
            move.x = vel;
        }
        if (Input.GetKey(KeyCode.A)){
            move.x = -vel;
        }
        if (Input.GetKeyDown(KeyCode.J) && !atacando){
            atacando = true;
            move.x = 0;
        }
        if(Input.GetKeyDown(KeyCode.I) && estar_no_chao){
            troca_ataque_espada = true;
            if (!espada){
                animacao("SC_espada_anim");
            } else{
                animacao("GD_espada_anim");
            }
            move.x = 0;
        }
        if(Input.GetKeyDown(KeyCode.S) && estar_no_chao){
            deslizar = true;
            if (!flip){
                move.x = vel * 1.5f;
            } else {
                move.x = -vel * 1.5f;
            }
 
            animacao("Deslizar_anim");
        }
        rb.velocity = move;
    }
 
 
    private void animar(Vector3 pos_passada){
 
        string parado, correndo;
 
        if(espada){
            parado = "P_espada_anim";
            correndo = "C_espada_anim";
        } else{
            parado = "P_anim";
            correndo = "C_anim";
        }
 
        if (rb.velocity.x > 0){
            if(flip){
                flip = false;
                sr.flipX = false;
            }
        } else if (rb.velocity.x < 0){
            if(!flip){
                flip = true;
                sr.flipX = true;
            }
        }
 
        if (rb.velocity.y > 0){
            animacao("J_anim");
            estar_no_chao = false;
            return;
        } else if (rb.velocity.y < 0){
            animacao("Caindo_anim");
            estar_no_chao = false;
            return;
        } else if (rb.velocity.x != 0){
            animacao(correndo);
            estar_no_chao = true;
            jump = false;
            return;
        }
        estar_no_chao = true;
        jump = false;
        animacao(parado);
    }
 
    private void animacao(string N_animacao){
 
        if(animacao_atual == N_animacao){
            return;
        }
        animacao_atual = N_animacao;
        plyer_anim.Play(animacao_atual);
    }
 
    private void atacar(){
 
        string ataque_1, ataque_2, ataque_3;
 
        if(espada){
            ataque_1 = "Ataque_espada_1_anim";
            ataque_2 = "Ataque_espada_2_anim";
            ataque_3 = "Ataque_espada_3_anim";
        } else{
            ataque_1 = "Ataque_1_anim";
            ataque_2 = "Ataque_2_anim";
            ataque_3 = "Ataque_3_anim";
        }
 
        if(ataque == 1){
            animacao(ataque_1);
            Debug.Log(animacao_atual);
        } else if (ataque == 2){
            animacao(ataque_2);
        } else{
            animacao(ataque_3);
        }
 
        if (Input.GetKeyDown(KeyCode.J)){
            prox_ataque = true;
        }
 
        if (fim_animacao()){
 
            if (prox_ataque && ataque < 3)
            {
                ataque++;
                prox_ataque = false;
            } else{
                atacando = false;
                prox_ataque = false;
                ataque = 1;
            }
        }
    }
 
    private void Trocar_Ataque(){
        if(!espada){
            animacao_atual = "SC_espada_anim";
        } else{
            animacao_atual = "GD_espada_anim";
        }
        if(fim_animacao()){
            troca_ataque_espada = false;
            espada = !espada;
        }
    }
 
    private bool fim_animacao() {
        animStateInfo = plyer_anim.GetCurrentAnimatorStateInfo(0);
 
        if (animStateInfo.IsName(animacao_atual) && animStateInfo.normalizedTime >= 1.0f) {
            return true;
        }
 
        return false;
    }
}