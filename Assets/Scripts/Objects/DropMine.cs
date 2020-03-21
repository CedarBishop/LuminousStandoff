using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DropMine : MonoBehaviour
{
    public float timeBeforeExplode;
    public ParticleSystem explosionParticle;
    public int damage;


    void Start()
    {
        StartCoroutine("CoExplode");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerCombat>())
        {
            if (other.GetComponentInParent<PhotonView>().IsMine)
            {
                return;
            }
            else
            {
                other.GetComponentInParent<PlayerCombat>().TakeDamage(damage, false);
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
        }

        Destroy(gameObject);
    }


}
