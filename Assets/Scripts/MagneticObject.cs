using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    public enum Polarity
    {
        POSITIVE,
        NEGATIVE,
        BOTH_ATTRACTIVE,
        BOTH_REPULSIVE
    }
    [SerializeField]
    private byte m_Damage = 1;


    private int m_DefaultLayer;

    public Polarity polarity;

    public bool AffectPlayer;
    public bool AffectSelf;

    // Start is called before the first frame update
    void Start()
    {
        m_DefaultLayer = gameObject.layer;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionStay(Collision other)
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player Projectiles") || gameObject.layer == LayerMask.NameToLayer("Enemy Projectiles"))
        {
            if (TryGetComponent(out Rigidbody rb) && rb.velocity == Vector3.zero)
                gameObject.layer = m_DefaultLayer;
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemies") && other.gameObject.TryGetComponent(out EnemyBehaviour enemyBehaviour))
        {
            enemyBehaviour.TakeDamage(m_Damage);

            //son quand l'ennemi reçoit des dégats de caisse Damage_Heavy
            //son quand l'ennemi reçoit des dégats de pièces Damage_Light
        }
    }
}
