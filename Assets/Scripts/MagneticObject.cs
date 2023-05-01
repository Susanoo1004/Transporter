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

    [HideInInspector]
    public bool DestroyOnCollision;

    private int m_DefaultLayer;

    public Polarity polarity;

    public bool AffectPlayer;
    public bool AffectSelf;

    private void Awake()
    {
        m_DefaultLayer = gameObject.layer;
    }

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
            if (TryGetComponent(out Rigidbody rb) && rb.velocity.magnitude < 0.1f)
                gameObject.layer = m_DefaultLayer;
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player Projectiles") && other.gameObject.layer == LayerMask.NameToLayer("Enemies") && other.gameObject.TryGetComponent(out EnemyBehaviour enemyBehaviour))
        {
            enemyBehaviour.TakeDamage(m_Damage);
        }
        if (gameObject.layer == LayerMask.NameToLayer("Enemy Projectiles") && other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject.TryGetComponent(out PlayerBehaviour playerBehaviour))
        {
            playerBehaviour.TakeDamage(m_Damage);
        }
    }
}
