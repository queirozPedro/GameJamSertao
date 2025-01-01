using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class lobisomen : MonoBehaviour
{
    private int estado_pacifico = 0;
    private int estado_agro = 0;
    [SerializeField] private float tempo_parado, tempo_andando, distancia_pacifico, distancia_agro, velocidade_agro;
    [SerializeField] private float distancia_ataque_distante, espera_ataque_distante, distancia_ataque_proximo, espera_ataque_proximo;
    [SerializeField] private float espera_parado_curto;
    private float tempo_espera, delay_ataque_distante, delay_ataque_proximo;
    private bool pacifico = true;
    Animator lobo_anim;
    SpriteRenderer sr;
    public GameObject player;
    private string animacao_atual;
    void Start()
    {
        tempo_espera = Time.time + tempo_parado;
        lobo_anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 antiga_posicao = transform.position;
        if (pacifico){
            /*Estado 0 = esta descançando, fica x periodo parado
            Estado 1 = esta andando, fica y periodo andando*/
            if (estado_pacifico == 0){
                if(Time.time > tempo_espera){
                    estado_pacifico = 1;
                    tempo_espera = Time.time + tempo_andando;
                } else{
                    animacao("parado_anim");
                }
            } else{
                if(Time.time > tempo_espera){
                    estado_pacifico = 0;
                    tempo_espera = Time.time + tempo_parado;
                } else{
                    animacao("andando_anim");
                }
            }
            if(distancia_entre_objetos(transform.position, player.transform.position, distancia_agro)){
                pacifico = false;
            }
        } else {

            /*Estado 0 = estar perseuindo o personagem ativado apos ele chegar perto
            Estado 1 = ataque distante ele corre em direção ao personagem, distancia media, com tempo de recarga
            Estado 2 = ataque perto, distancia curta com periodo de recarga
            Estado 3 = descançando curto, um curto periodo entre o ataque e o estado normal(0)
            Estado 4 = descançando, apos os dois ataques serem gastos ou depois de perserguir o persongem durante um tempo sem realizar ataque*/
            if (estado_agro == 0){
                animacao("correndo_anim");
                if(!distancia_entre_objetos(transform.position, player.transform.position, distancia_pacifico)){
                    pacifico = true;
                }/*
                if (distancia_entre_objetos(transform.position, player.transform.position, distancia_ataque_distante) && delay_ataque_distante <= Time.time){
                    estado_agro = 1;
                }*/
                if (distancia_entre_objetos(transform.position, player.transform.position, distancia_ataque_proximo) && delay_ataque_proximo <= Time.time){
                    estado_agro = 2;
                }
                transform.position = new(Mathf.MoveTowards(transform.position.x, player.transform.position.x, velocidade_agro * Time.deltaTime), transform.position.y, transform.position.z);
            } else if(estado_agro == 2){
                animacao("ataque1_anim");
                animacao_atual = "ataque1_anim";
                
                if(fim_animacao()){
                    estado_agro = 3;
                    tempo_espera = Time.time + espera_parado_curto;
                }
            } else if(estado_agro == 3){
                animacao("parado_anim");
                if(Time.time > tempo_espera){
                    estado_agro = 0;
                }
            }            
        }
        animar(antiga_posicao);
    }
    private void animacao(string N_animacao){
 
        if(animacao_atual == N_animacao){
            return;
        }
        animacao_atual = N_animacao;
        lobo_anim.Play(animacao_atual);
    }

    private void animar(Vector3 antiga_posicao){
        if(transform.position.x > antiga_posicao.x){
            sr.flipX = false;
        } else if(transform.position.x < antiga_posicao.x){
            sr.flipX = true;
        }
    }

    private bool distancia_entre_objetos(Vector3 objeto1, Vector3 objeto2, float distancia){

        if((objeto1 - objeto2).sqrMagnitude <= distancia*distancia) return true;
        else return false;
    }

    private bool fim_animacao() {
 
        if (lobo_anim.GetCurrentAnimatorStateInfo(0).IsName(animacao_atual) && lobo_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            return true;
        }
 
        return false;
    }
}
