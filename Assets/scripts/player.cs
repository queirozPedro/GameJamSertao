using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class player : MonoBehaviour
{

    [SerializeField] private float vel, impulso, tempo_espera;
    public float vida, dano_soco, dano_espada, knok, tempo_imune;
    Animator plyer_anim;
    private AnimatorStateInfo animStateInfo;
    Rigidbody2D rb;
    private string animacao_atual;
    private bool flip = false, estar_no_chao, atacando = false, prox_ataque = false, deslizar = false;
    private bool espada = false, troca_ataque_espada = false, imune = false, hit = false, morte = false;
    private short ataque = 1;
    public GameObject game_controler;
    public AudioClip musica_pricipal;
 
 
    void Start()
    {
        plyer_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        game_controler.GetComponent<game_controler>().play_audio(musica_pricipal);
    }
 
    void Update()
    {

        if(morte){
            rb.velocity = Vector2.zero;
            animacao("morte_anim");
            imune = true;
        } else if(hit){
            animacao("hit_anim");
            animacao_atual = "hit_anim";
            if(fim_animacao()){
                hit = false;
                imune = false;
                tempo_espera = Time.time + tempo_imune;
            }
        }else if(deslizar){
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
        if (Input.GetKeyDown(KeyCode.W) && estar_no_chao){
        
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
        if(Input.GetKeyDown(KeyCode.K) && estar_no_chao){
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
                transform.localScale = new ((-1)*transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        } else if (rb.velocity.x < 0){
            if(!flip){
                flip = true;
                transform.localScale = new ((-1)*transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
            return;
        }
        estar_no_chao = true;
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


    private void OnTriggerEnter2D(Collider2D collision) { 
        if (collision.CompareTag("ataque_inimigo") && !imune && tempo_espera < Time.time) { 
            Transform parentTransform = collision.transform.parent; 
            if (parentTransform != null) { 
                vida -= parentTransform.GetComponent<inimigos>().dano; 
            } 
            if(transform.position.x > collision.transform.parent.position.x){
                rb.AddForce(new Vector2(1, 0.3f).normalized * 40, ForceMode2D.Impulse);
            } else{
                rb.AddForce(new Vector2(-1, 0.3f).normalized * 40, ForceMode2D.Impulse);
            }
            hit = true;
            imune = true;

            if(vida <= 0){
                morte = true;
            }
        } else if(collision.CompareTag("ataque_lobisomen") && !imune && tempo_espera < Time.time){

            Transform parentTransform = collision.transform.parent; 
            if (parentTransform != null) { 
                vida -= parentTransform.GetComponent<lobisomen>().dano; 
            } 
            if(transform.position.x > collision.transform.parent.position.x){
                rb.AddForce(new Vector2(1, 0.3f).normalized * knok, ForceMode2D.Impulse);
            } else{
                rb.AddForce(new Vector2(-1, 0.3f).normalized * knok, ForceMode2D.Impulse);
            }
            hit = true;
            imune = true;

            if(vida <= 0){
                morte = true;
            }
        }
    }
}