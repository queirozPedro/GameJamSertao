using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class inimigos : MonoBehaviour
{
    [NonSerialized] public int estado;
    public float velocidade, distancia_pacifico, distacia_agro, distancia_ataque, dano, vida, delay_ataque;
    public GameObject player;
    [NonSerialized] public SpriteRenderer sr;
    [NonSerialized] public Animator Anim_controler;
    [NonSerialized] public string animacao_atual;
    [NonSerialized] public bool direcao;
    [NonSerialized] public Rigidbody2D rb;
    private bool imune = false, flip;
    /*
    Estado 0 = pacifico
    Estado 1 = perseguindo
    Estado 2 = atacando
    Estado 3 = hit
    Estado 4 = morte
    */

    public void maquina_de_estados(){

        Vector3 antiga_posicao = transform.position;
        if(estado == 0){
            estado0_parado();
        } else if(estado == 1){
            estado1_agrecivo();
        } else if(estado == 2){
            estado2_ataque();
        } else if(estado == 3){
            imune = true;
            estado3_hit();
        } else if(estado == 4){
            imune = true;
            estado4_morte();
        }
        animar(antiga_posicao);
    }

    private void animar(Vector3 antiga_posicao){
        if (transform.position.x > antiga_posicao.x){
            if(flip){
                flip = false;
                transform.localScale = new ((-1)*transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        } else if (transform.position.x < antiga_posicao.x){
            if(!flip){
                flip = true;
                transform.localScale = new ((-1)*transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void estado0_parado(){
        animacao("parado_anim");
        if(distancia_entre_objetos(transform.position, player.transform.position, distacia_agro)){
            estado = 1;
        }
    }

    public abstract void estado1_agrecivo();
    public abstract void estado2_ataque();
    private void estado3_hit(){
        animacao("hit_anim");
        animacao_atual = "hit_anim";
        if(fim_animacao()){
            imune = false;
            estado = 1;
        }
    }
    private void estado4_morte(){
        animacao("morte_anim");
        animacao_atual = "morte_anim";
        if(fim_animacao()){
            Destroy(gameObject);
        }
    }

    public void animacao(string N_animacao){
 
        if(animacao_atual == N_animacao){
            return;
        }
        animacao_atual = N_animacao;
        Anim_controler.Play(animacao_atual);
    }

    public bool distancia_entre_objetos(Vector3 objeto1, Vector3 objeto2, float distancia){

        if((objeto1 - objeto2).sqrMagnitude <= distancia*distancia) return true;
        else return false;
    }

    public bool fim_animacao() {
 
        if (Anim_controler.GetCurrentAnimatorStateInfo(0).IsName(animacao_atual) && Anim_controler.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            return true;
        }
 
        return false;
    }

    private void Oncollider2D(Collider2D collider) {

        if (collider.tag == "player_ataque" && !imune){
            AnimatorStateInfo animacao_presente = player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            if(animacao_presente.IsName("ataque_1_anim")  || animacao_presente.IsName("ataque_2_anim") || animacao_presente.IsName("ataque_3_anim")){
                vida -= player.GetComponent<player>().dano_soco;
            } else{
                vida -= player.GetComponent<player>().dano_espada;
            }
            estado = 3;
            rb.AddForce(new Vector2(transform.position.x - player.transform.position.x, 0.5F).normalized * 50, ForceMode2D.Impulse);
        }
    }
}
