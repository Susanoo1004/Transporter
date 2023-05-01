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
    [SerializeField]
    private AudioSource m_DamageHeavy;
    [SerializeField]
    private AudioSource m_DamageLight;
    [SerializeField]
    private AudioClip[] m_DamageHeavyList;
    [SerializeField]
    private AudioClip[] m_DamageLightList;


   


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

            //son quand l'ennemi re�oit des d�gats de caisse Damage_Heavy
            play_DamageHeavy();
            //son quand l'ennemi re�oit des d�gats de pi�ces Damage_Light.  play_DamageLight();
                        
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            //play_DamageLight();
            play_DamageHeavy();
        }
        if (gameObject.layer == LayerMask.NameToLayer("Enemy Projectiles") && other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject.TryGetComponent(out PlayerBehaviour playerBehaviour))
        {
            playerBehaviour.TakeDamage(m_Damage);
        }
    }

    public void play_DamageHeavy()
    {

        int index = UnityEngine.Random.Range(0, m_DamageHeavyList.Length);
        m_DamageHeavy.PlayOneShot(m_DamageHeavyList[index]);
    }

    public void play_DamageLight()
    {

        int index = UnityEngine.Random.Range(0, m_DamageLightList.Length);
        m_DamageLight.PlayOneShot(m_DamageLightList[index]);
    }
}
