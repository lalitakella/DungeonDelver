﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    [Header("Set in Inspector: Enemy")]
    public float maxHealth = 1;
    public float knockbackSpeed = 10; 
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;

    [Header("Set Dynamically: Enemy")]
    public float health;
    public bool invincible = false; 
    public bool knockback = false;
    private float invincibleDone = 0; 
    private float knockbackDone = 0;
    private Vector3 knockbackVel;

    protected Animator anim; 
    protected Rigidbody rigid; 
    protected SpriteRenderer sRend;

    protected virtual void Awake()
    { 
        health = maxHealth;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        sRend = GetComponent<SpriteRenderer>();
    }
    protected virtual void Update()
    { 
        if (invincible && Time.time > invincibleDone) invincible = false;
        sRend.color = invincible ? Color.red : Color.white;
        if (knockback)
        {
            rigid.velocity = knockbackVel;
            if (Time.time < knockbackDone) return;
        }
        anim.speed = 1; 
        knockback = false;
    }
    void OnTriggerEnter(Collider colld)
    { 
        if (invincible) return; 
        DamageEffect dEf = colld.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return; 
        health -= dEf.damage; 
        if (health <= 0) Die(); 
        invincible = true; 
        invincibleDone = Time.time + invincibleDuration;
        if (dEf.knockback)
        { 
            Vector3 delta = transform.position - colld.transform.root.position;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            }
            else
            {
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }
            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;
            knockback = true;
            knockbackDone = Time.time + knockbackDuration;
            anim.speed = 0;
        }
    }
    void Die()
    { 
        Destroy(gameObject);
    }
}