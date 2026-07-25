using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
     
     [Header("Particles")]
     public ParticleSystem jumpParticles;
     public ParticleSystem landingParticles;
     public ParticleSystem hitParticles;
     public ParticleSystem swapParticles;

     public void PlayJumpParticles()
     {
          jumpParticles.Play();
     }

     public void PlayLandingParticles(Vector2 position, Quaternion normal)
     {
          landingParticles.transform.position = position;
          landingParticles.transform.rotation = normal;
          
          landingParticles.Play();
     }

     public void PlaySwapParticles(Vector2 position)
     {
          swapParticles.transform.position = position;
          swapParticles.Play();
     }

     public void PlayHitParticles(Vector2 position)
     {
          hitParticles.transform.position = position;
          hitParticles.Play();
     }
}
