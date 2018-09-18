using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DustMinerGame : MonoBehaviour
{
    public static float rocketFill;

    public SpawnRocks spawner;

    public Conveyor[] conveyor;
    public float conveySpeed = 4;

    public Renderer conveyorRender;
    public float renderSpeed;

    public ExcursionFunnel funnel;

    public int maxRocks = 3;

    public Collider beltCol;
    public PhysicMaterial p_move;
    public PhysicMaterial p_stop;

    public GameObject[] tutStages;
    public MoonRock tutRock;

    public VaccuumSuck vax;

    public GameObject tuberPrefab;

    public GameObject rocket;
    public Animator rocketAnim;

    public Image rocketInd;
    public Canvas rocketCanvas;

    public GameObject lvlSwitch;

    public ParticleSystem gelSprayer;


    private bool launched = false;
    private float filltime = 70;

    private float speeder;
    private float conveyOff;

    private void Start()
    {
        StartCoroutine(DoTutorial());

        if (rocketFill >= 1)
            rocketFill = 0;
    }

    private void Update()
    {
        rocketInd.fillAmount = Mathf.Lerp(rocketInd.fillAmount, rocketFill, Time.deltaTime * 9);

        if (!launched && rocketFill >= 0.95f)
        {
            rocketFill = 1;
            launched = true;
            rocketAnim.SetTrigger("Launch");
            StartCoroutine(DoRocketLaunch());
        }

        bool go = funnel.numRocks < maxRocks;
        speeder = Mathf.Lerp(speeder, go ? 1 : 0, Time.deltaTime * 2);

        foreach (Conveyor conveyorPart in conveyor)
        {
            conveyorPart.speed = conveySpeed * speeder;
        }

        conveyOff += Time.deltaTime / 20 * speeder * renderSpeed;
        conveyorRender.material.SetFloat("_Move", conveyOff);

        bool stopped = speeder < 0.3f;
        beltCol.material = stopped ? p_stop : p_move;

        spawner.go = go;
    }

    private IEnumerator DoRocketLaunch()
    {
        yield return new WaitForSeconds(1);
        rocketCanvas.gameObject.SetActive(false);

        yield return new WaitForSeconds(6);
        lvlSwitch.SetActive(true);
    }

    private IEnumerator DoTutorial()
    {
        tutRock.crushable = false;
        SetTut(0);
        yield return null;
        while (!tutRock.interactable.attachedToHand)
        {
            yield return null;
        }
        SetTut(1);
        while (!vax.rockIn)
        {
            yield return null;
        }
        SetTut(2);
        tutRock.crushable = true;
    }

    public void SuckDust()
    {
        StartCoroutine(DoDustSuck());
    }

    private IEnumerator DoDustSuck()
    {
        GameObject tube = Instantiate(tuberPrefab, tuberPrefab.transform.position, tuberPrefab.transform.rotation, tuberPrefab.transform.parent);
        yield return new WaitForSeconds(0.2f);
        tube.SetActive(true);
        Renderer tubeRenderer = tube.GetComponent<Renderer>();

        float pos = 0;
        while (pos < 1)
        {
            pos += Time.deltaTime / 1.5f;
            tubeRenderer.material.SetFloat("_Pos", pos);
            yield return null;
        }

        Destroy(tube);
        bool rocketThere = rocket != null && rocket.activeInHierarchy;
        if (rocketThere) rocketThere = rocket.transform.localPosition.y < 1;

        if (rocketThere)
        {
            rocketFill += 1 / filltime;
        }
        else
        {
            gelSprayer.Play();
        }
    }

    private void SetTut(int activeStageIndex)
    {
        for (int stageIndex = 0; stageIndex < tutStages.Length; stageIndex++)
        {
            tutStages[stageIndex].SetActive(stageIndex == activeStageIndex);
        }
    }
}