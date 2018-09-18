using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DustCloud : MonoBehaviour
{
    public SphereCollider col;

    public ParticleSystem ps;

    public Renderer r;

    // Update is called once per frame
    private void Update()
    {
        if (col)
        {
            col.radius = r.bounds.extents.magnitude;
            col.radius -= ps.main.startSize.constantMax * 2;
            col.center = transform.InverseTransformPoint(r.bounds.center);
        }
    }

    private void MoveParticles(VaccuumSuck vacuum)
    {
        vacuum.Suck();
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particleArray);

        List<ParticleSystem.Particle> particleList = particleArray.ToList();
        for (int i = ps.particleCount - 1; i >= 0; i--)
        {
            float strength = vacuum.GetStrength(particleList[i].position);
            //pts[i].position = vax.GetAxisTarget(pts[i].position);

            float distance = vacuum.distance(particleList[i].position);

            Vector3 velocity = (vacuum.GetAxisTarget(particleList[i].position) - particleList[i].position) + (vacuum.transform.position - particleList[i].position).normalized * strength * 2;

            ParticleSystem.Particle mod = particleList[i];
            mod.velocity += velocity * strength * Time.deltaTime * 6;
            mod.startSize = Mathf.Lerp(mod.startSize, Mathf.Lerp(ps.main.startSize.constantMax, ps.main.startSize.constantMax * 0.1f, 1 - distance), Time.deltaTime * 6);

            particleList[i] = mod;

            if (distance < 0.1)
            {
                particleList.Remove(particleList[i]);//suck in particle
                vacuum.SuckIn();
            }
        }

        ParticleSystem.Particle[] ptsNew = particleList.ToArray();
        ps.SetParticles(ptsNew, ptsNew.Length);
        if (ps.particleCount == 0)
        {
            Destroy(GetComponent<Collider>());
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.GetComponent<VaccuumSuck>())
        {
            MoveParticles(collider.GetComponent<VaccuumSuck>());
        }
    }
}