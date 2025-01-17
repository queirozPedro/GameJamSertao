using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class tatu : inimigos
{

    private float tempo_espera;
    private Vector3 posicao_player;
    private int direcao_x;
    void Start()
    {
        direcao = false;
        rb = GetComponent<Rigidbody2D>();
        Anim_controler = GetComponent<Animator>();
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    void Update()
    {
        maquina_de_estados();
    }

    public override void estado1_agrecivo()
    {
        float distancia_Y = player.transform.position.y - transform.position.y;
        if(!distancia_entre_objetos(transform.position, player.transform.position, distancia_pacifico) || (distancia_Y > 4 || distancia_Y < -4)){
            estado = 0;
        } else if (distancia_entre_objetos(transform.position, player.transform.position, distancia_ataque) && tempo_espera <= Time.time){
            estado = 2;
            if(transform.position.x < player.transform.position.x) direcao_x = 1; else direcao_x = -1;
        } else if (distancia_entre_objetos(transform.position, player.transform.position, distancia_ataque)){
            estado = 0;
        } else {
            animacao("andando_anim");
            transform.position = new(Mathf.MoveTowards(transform.position.x, ((transform.position.x - player.transform.position.x)/(transform.position.x - player.transform.position.x)) + player.transform.position.x, velocidade * Time.deltaTime), transform.position.y, transform.position.z);
        }
    }

    public override void estado2_ataque()
    {
        imune = true;
        animacao("ataque_anim");
        animacao_atual = "ataque_anim";
        transform.position = new(transform.position.x + velocidade  * 6 * Time.deltaTime * direcao_x, transform.position.y, transform.position.z);
        if(fim_animacao()){
            imune = false;
            estado = 1;
            tempo_espera = Time.time + delay_ataque;
        }
    }
}

