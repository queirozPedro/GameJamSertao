using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class lobisomen : MonoBehaviour
{
    private int estado_pacifico = 0;
    private int estado_agro = 0;
    [SerializeField] private float tempo_parado, tempo_andando, distancia_pacifico, distancia_agro, velocidade_agro, velocidade_pacifico;
    [SerializeField] private float distancia_ataque_distante, espera_ataque_distante, distancia_ataque_proximo, espera_ataque_proximo;
    [SerializeField] private float espera_parado_curto, tempo_vuneravel;
    [SerializeField] private float dano, vida_vuneravel, vida_total;
    private float tempo_espera, delay_ataque_distante, delay_ataque_proximo;
    private bool pacifico = true, flip = false, vuneravel = false, imune = false;
    private int ataque_seguencia = 1, sinal = 1;
    Animator lobo_anim;
    public GameObject player;
    private Vector3 posicao_player;
    private string animacao_atual;
    void Start()
    {
        tempo_espera = Time.time + tempo_parado;
        lobo_anim = GetComponent<Animator>();
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
                    sinal *= -1;
                } else{
                    animacao("andando_anim");
                    transform.position += new Vector3(Time.deltaTime * velocidade_pacifico * sinal, 0, 0);
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
                } else if (distancia_entre_objetos(transform.position, player.transform.position, distancia_ataque_distante) && delay_ataque_distante <= Time.time){
                    estado_agro = 1;
                    posicao_player = player.transform.position;
                    
                }else if (distancia_entre_objetos(transform.position, player.transform.position, distancia_ataque_proximo) && delay_ataque_proximo <= Time.time){
                    estado_agro = 2;
                } /*else {
                    estado_agro = 4;
                }*/
                transform.position = new(Mathf.MoveTowards(transform.position.x, player.transform.position.x, velocidade_agro * Time.deltaTime), transform.position.y, transform.position.z);
            } else if(estado_agro == 1){
                animacao("ataque_correndo_anim");
                animacao_atual = "ataque_correndo_anim";
                transform.position = new(Mathf.MoveTowards(transform.position.x, posicao_player.x, (velocidade_agro * 1.6f) * Time.deltaTime), transform.position.y, transform.position.z);
                if(fim_animacao()){
                    estado_agro = 3;
                    tempo_espera = Time.time + espera_parado_curto;
                    delay_ataque_distante = Time.time + espera_ataque_distante + espera_parado_curto;
                }

            }else if(estado_agro == 2){
                if(ataque_seguencia == 1){
                    animacao("ataque1_anim");
                    animacao_atual = "ataque1_anim";
                } else if(ataque_seguencia == 2){
                    animacao("ataque2_anim");
                    animacao_atual = "ataque2_anim";
                } else if(ataque_seguencia == 3){
                    animacao("ataque3_anim");
                    animacao_atual = "ataque3_anim";
                } 
                
                if(fim_animacao()){
                    if (distancia_entre_objetos(transform.position, player.transform.position, distancia_ataque_proximo * 1.5f) && ataque_seguencia < 3){
                        ataque_seguencia++;
                    }else {
                        estado_agro = 3;
                        ataque_seguencia = 1;
                        tempo_espera = Time.time + espera_parado_curto;
                        delay_ataque_proximo = Time.time + espera_ataque_proximo + espera_parado_curto;
                    }  
                }
            } else if(estado_agro == 3){
                animacao("parado_anim");
                if(Time.time > tempo_espera){
                    estado_agro = 0;
                }
            } else if(estado_agro == 4){

                animacao("hit_anim");
                vuneravel = true;
                if(Time.time > tempo_espera){
                    estado_agro = 0;
                    vida_vuneravel = 3;
                    vuneravel = false;
                }

            } else if(estado_agro == 5){
                imune = true;
                animacao("morte_anim");
                animacao_atual = "morte_anim";
                if(fim_animacao()){
                    Destroy(gameObject);
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

    private void OnTriggerEnter2D(Collider2D collision) { 
        if (collision.CompareTag("player_ataque") && !imune) {
            AnimatorStateInfo animacao_presente = player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0); 
            if (animacao_presente.IsName("ataque_1_anim") || animacao_presente.IsName("ataque_2_anim") || animacao_presente.IsName("ataque_3_anim")) { 
                if(vuneravel)
                vida_total -= player.GetComponent<player>().dano_soco;
                else
                vida_vuneravel -= player.GetComponent<player>().dano_soco;

            } else { 
                if(vuneravel)
                vida_total -= player.GetComponent<player>().dano_espada;
                else
                vida_vuneravel -= player.GetComponent<player>().dano_espada;
            } 
            if(vuneravel){
                if(vida_total <= 0){
                    estado_agro = 5;
                }
            }else{
                if(vida_vuneravel <= 0){
                    tempo_espera = Time.time + tempo_vuneravel;
                    estado_agro = 4;
                }
            }
        } 
    }
}
