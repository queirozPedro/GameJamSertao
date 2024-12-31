using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobisomen : MonoBehaviour
{
    private int estado_pacifico = 0;
    private int estado_agro = 0;
    [SerializeField] private float tempo_parado, tempo_andando, distancia_pacifico, distancia_agro;
    private float tempo_espera;
    private bool pacifico = true;
    Animator lobo_anim;
    public GameObject player;
    private string animacao_atual;
    void Start()
    {
        tempo_espera = Time.time + tempo_parado;
        lobo_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pacifico){
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
            animacao("correndo_anim");
            if(!distancia_entre_objetos(transform.position, player.transform.position, distancia_pacifico)){
                pacifico = true;
            }
        }
    }
    private void animacao(string N_animacao){
 
        if(animacao_atual == N_animacao){
            return;
        }
        animacao_atual = N_animacao;
        lobo_anim.Play(animacao_atual);
    }

    private bool distancia_entre_objetos(Vector3 objeto1, Vector3 objeto2, float distancia){

        if((objeto1 - objeto2).sqrMagnitude <= distancia*distancia) return true;
        else return false;
    }
}
