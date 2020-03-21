using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DropMine : MonoBehaviour
{
    public float timeBeforeExplode;
    public ParticleSystem explosionParticle;
    public int damage;
    public int roomNumber;


    void Start()
    {
        StartCoroutine("CoExplode"); // start timer before this mine explodes
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerCombat>())             // Check if colliders has player combat component
        {
            if (other.GetComponentInParent<PlayerCombat>().roomNumber != roomNumber) // check if that room number of that player does not equal the number of the player who placed this mine
            {
                other.GetComponentInParent<PlayerCombat>().TakeDamage(damage, false); // if so, the damage that player and play explosion particle
                ExplosionParticle();
            }
        }
    }

    IEnumerator CoExplode ()
    {
        yield return new WaitForSeconds(timeBeforeExplode);

        ExplosionParticle();
    }

    void ExplosionParticle ()
    {
        if (explosionParticle != null)
        {
            ParticleSystem particle = Instantiate(explosionParticle, transform.position,Quaternion.identity);
            particle.Play();
            Destroy(particle, 1);
        }
        Destroy(gameObject);
    }


}
