using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f; // The speed in m/s
    public float fireRate = 0.3f; // Seconds/shot (Unused)
    public float health = 10;
    public int score = 100; // Points earned for destroying this
    public float showDamageDuration = 0.1f; // # seconds to show damage // a
    public float powerUpDropChance = 1f; // Chance to drop a power-up // a
    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;// All the Materials of this & its children
    public bool showingDamage = false;
    public float damageDoneTime; // Time to stop showing damage
    public bool notifiedOfDestruction = false; // Will be used later
                                               // This is a Property: A method that acts like a field
    protected BoundsCheck bndCheck; // a
    void Awake()
    { // b
        bndCheck = GetComponent<BoundsCheck>();
        // Get materials and colors for this GameObject and its children
        materials = Utils.GetAllMaterials(gameObject); // b
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    public Vector3 pos
    { // a
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
    void Update()
    {
        Move();
        if (showingDamage && Time.time > damageDoneTime)
        { // c
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.offDown)
        { // c
          // Check to make sure it's gone off the bottom of the screen
            
              // We're off the bottom, so destroy this GameObject
                Destroy(gameObject);
            
        }

    }
    public virtual void Move()
    { // b
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    /*void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject; // a
        if (otherGO.tag == "ProjectileHero")
        { // b
            Destroy(otherGO); // Destroy the Projectile
            Destroy(gameObject); // Destroy this Enemy GameObject
        }
        else
        {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name); // c
        }
    }*/

    void OnCollisionEnter(Collision coll)
    { // a
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero": // b
                Projectile p = otherGO.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.
                if (!bndCheck.isOnScreen)
                { // c
                    Destroy(otherGO);
                    break;
                }
                // Hurt this Enemy
                
                ShowDamage(); // d
                // Get the damage amount from the Main WEAP_DICT.
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                { // d
                  // Tell the Main singleton that this ship was destroyed // b
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                }
                Destroy(otherGO); // e
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name); // f
                break;
        }
    }

    void ShowDamage()
    { // e
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    { // f
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }


}
