using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidEnabler : MonoBehaviour {

    public WheelCollider wheelCollider;
    public GameObject skidTrailRenderer;
    public float f_skidLife = 100f;
    private TrailRenderer skidMark;
    public ParticleSystem skidParticles;

    void Start () {
        skidMark = skidTrailRenderer.GetComponent<TrailRenderer>();
        skidParticles = transform.root.GetComponentInChildren<ParticleSystem>();
        skidMark.time = f_skidLife;
	}
	

	void Update () {

        if (wheelCollider.forwardFriction.stiffness < 0.04 && wheelCollider.isGrounded)
        {
            FindObjectOfType<AudioManager>().Play("skid");
       
            skidParticles.transform.position = wheelCollider.center + ((wheelCollider.radius - 1.05f) * wheelCollider.transform.up / 2);
            skidParticles.Play();
            if (skidMark.time == 0)
            {
                
                skidMark.time = f_skidLife;
                skidTrailRenderer.transform.parent = wheelCollider.transform;
                skidTrailRenderer.transform.localPosition = wheelCollider.center + ((wheelCollider.radius - 1.05f) * wheelCollider.transform.up);

            }

            if (skidTrailRenderer.transform.parent == null)
            {
                
                skidMark.time = 0;
            }
        }
        else
        {
            skidTrailRenderer.transform.parent = null;
            FindObjectOfType<AudioManager>().Stop("skid");
            //skidParticles.Stop();
        }
	}
}
