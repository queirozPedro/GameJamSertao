using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class urubu : inimigos
{

    private float tempo_espera;
    void Start()
    {
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
        if(!distancia_entre_objetos(transform.position, player.transform.position, distancia_pacifico)){
            estado = 0;
            rb.gravityScale = 1;
        } else if (distancia_entre_objetos(transform.position, player.transform.position, distancia_ataque) && tempo_espera <= Time.time){
            estado = 2;  
        } else if (distancia_entre_objetos(transform.position, player.transform.position, 1)){
            animacao("andando_anim");
        } else{
            rb.gravityScale = 0;
            animacao("andando_anim");
            transform.position = new(Mathf.MoveTowards(transform.position.x, player.transform.position.x, velocidade * Time.deltaTime), Mathf.MoveTowards(transform.position.y, player.transform.position.y, velocidade * Time.deltaTime), transform.position.z);
        }
    }

    public override void estado2_ataque()
    {
        animacao("ataque_anim");
        animacao_atual = "ataque_anim";
        transform.position = new(Mathf.MoveTowards(transform.position.x, player.transform.position.x, (velocidade * 1.6f) * Time.deltaTime), transform.position.y, transform.position.z);
        if(fim_animacao()){
            estado = 1;
            tempo_espera = Time.time + delay_ataque;
        }
    }
}
