using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System;
//using System;

public class GunAndMeleeSystem : MonoBehaviour
{

    private Animator animator;
    private GameObject hands;
    private Transform tr;
    private GameManager gameManager;
    public AudioSource pistolSoundSource;
    public AudioSource swordSoundSource;
    public GameObject mainCamera;
    public GameObject handCamera;
    public LayerMask layermask;
    private FirstPersonController fpController;
    private PlayerStats stats;
    private MeleeCollisionDetection meleeDetection;
    private GameObject muzzleFlash;

    public AudioClip shootSound;
    public AudioClip emptySound;
    public AudioClip reloadSound;
    public AudioClip pistolSpinSound;
    public AudioClip slash1Sound;
    public AudioClip slash2Sound;



    public GameObject bulletDecal;

    private Transform canvas;
    public Text ammoText;
    public Text ammoLoadedText;

    private bool isAiming = false;
    private bool justShot = false;
    public bool isSlashing = false;
    private bool isInComboWindow = false;

    private bool isIdlingRotate = false;
    private bool isIdlingInspect = false;
    public bool allowShot = true;
    public float pistolRange = 16;
    public float pistolDmgMin = 4;
    public float pistolDmgMax = 7;
    public float headshotMult = 4;
    public float slashDamage = 14.3f;
    public float backstabMult = 2.35f;
    public float backstabAngle = 90;

    private int ammoReserve = 30;
    private int ammoLoaded = 7;
    private int ammoLoadedMax = 7;
    private int ammoReserveMax = 80;
    // Use this for initialization
    void Start()
    {
        PlayerDependencyManager pdm = GetComponent<PlayerDependencyManager>();
        stats = GetComponent<PlayerStats>();
        fpController = GetComponent<FirstPersonController>();
        tr = transform;
        hands = transform.Find("Body/Hands").gameObject;
        muzzleFlash = transform.Find("Body/Hands/handSocket_r/GunMesh/MuzzleFlash").gameObject;
        muzzleFlash.SetActive(false);
        animator = hands.GetComponent<Animator>();
        canvas = pdm.canvas;
        ammoText = canvas.Find("HUD/GunPanel/Max").GetComponent<Text>();
        ammoLoadedText = canvas.Find("HUD/GunPanel/Magazine").GetComponent<Text>();
        gameManager = pdm.gameManager;
        meleeDetection = transform.Find("Body/Camera/PunchArea").GetComponent<MeleeCollisionDetection>();

        //lastPos = transform.position;
    }


    float tempMin=0;
    float tempMax=0;
    float slashTemp=0;
    private float idlingTimeStamp= 30;
    bool cheat = false;
    private bool hittingEnemy = false;

    float turnSway = 0;
    float swayRestoreSpeed = 30;
    void Update()
    {



        ammoText.text = ammoReserve.ToString();
        ammoLoadedText.text = ammoLoaded.ToString();

        if(Input.GetKeyDown(KeyCode.P))
        {
            cheat = !cheat;
            fpController.CheatMode(cheat);
            stats.cheatMode = cheat;
            if (cheat)
            {
                tempMin = pistolDmgMin;
                tempMax = pistolDmgMax;
                pistolDmgMin = 999;
                pistolDmgMax = 999;
                slashTemp = slashDamage;
                slashDamage = 999;
            }
            else
            {
                pistolDmgMin = tempMin;
                pistolDmgMax = tempMax;
                slashDamage = slashTemp;
            }
            
        }

        bool isWalking = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        animator.SetBool("isWalking", isWalking);
        //animator.SetLayerWeight(animator.GetLayerIndex("Walk"), isWalking ? Mathf.Lerp(0, 1, 0.1f*Time.deltaTime) : Mathf.Lerp(1, 0, 0.1f * Time.deltaTime));

        if (turnSway > 25)
            turnSway = 25;
        if (turnSway < -25)
            turnSway = -25;
        if (turnSway < 0)
            turnSway += swayRestoreSpeed * Time.deltaTime;
        if (turnSway > 0)
            turnSway -= swayRestoreSpeed * Time.deltaTime;
        if (Mathf.Approximately(turnSway, 0))
            turnSway = 0;

        turnSway += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1);
        animator.SetFloat("TurningSway", turnSway);
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isSlashing && allowShot)
            {
                Shoot();
            }

        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (!isSlashing)
                Slash("normal");
            
        }
        if (Input.GetButtonDown("Reload"))
        {
            if (!isSlashing && allowShot)
                Reload(true);
        }
    }


    private int slashChoice = 1;


    public void Slash(string type)
    {
        
        animator.SetInteger("SlashChoice", slashChoice);
        slashChoice++;
        Debug.Log(slashChoice);
        if (slashChoice > 3)
            slashChoice = 1;
        Debug.Log("After: " + slashChoice);
        RaycastHit raycastHit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out raycastHit, 3.5f, layermask.value, QueryTriggerInteraction.Ignore))
        {
            if (raycastHit.collider.CompareTag("Enemy"))
                hittingEnemy = true;
            else
                hittingEnemy = false;
        }
        else
            hittingEnemy = false;
        animator.SetBool("SlashContact", hittingEnemy);
        animator.SetTrigger("Slash");
        isSlashing = true;
    }



    bool toPlay = true;
    bool firstHit = true;



    public void DoSlashDamage()
    {
        if (toPlay)
            swordSoundSource.PlayOneShot(slash1Sound);
        else
            swordSoundSource.PlayOneShot(slash2Sound);
        toPlay = !toPlay;
        List<Damageable> targets = new List<Damageable>();
        if (!meleeDetection.GetTargetsInMelee(ref targets))
        {

            return;
        }
       



        //if (slashChoice != 1 || !firstHit)
        //    animator.SetBool("SlashContact",true);
        if (slashChoice == 1)
        {
            firstHit = !firstHit;
        }
        float dmgMod = (slashChoice == 1) ? 0.5f : 1;
        
        foreach (Damageable i in targets)
        {
            float backstabMod = IsBehindTarget(i.transform, backstabAngle) ? backstabMult : 1;

            if (backstabMod != 1)
                Debug.Log("BACKSTAB!");

            i.CauseDamage(slashDamage * dmgMod * backstabMod, "slash"+ (slashChoice-1));
        }
        

    }

    //Soon to implement, combos.

    //public void SetInComboWindow(bool state)
    //{
    //    isInComboWindow = state;
    //}




    public void EndSlash()
    {
        
        isSlashing = false;
    }




    int ammoNeeded = 0;

    public void Reload(bool state)
    {
        
        if (state)
        {
            ammoNeeded = ammoLoadedMax - ammoLoaded;

            if (ammoNeeded > 0 && ammoReserve > 0)
            {
                animator.SetTrigger("Reload");
                allowShot = false;  
            }
            
        }
        else
        {
            if (ammoReserve >= ammoNeeded)
                ammoReserve -= ammoNeeded;
            else
            {
                ammoNeeded = ammoReserve;
                ammoReserve = 0;
            }

            ammoLoaded += ammoNeeded;
        }
    }

    //void Aim(bool state)
    //{
    //    isAiming = state;
    //    fpController.canRun = !state;
    //    animator.SetBool("Aim", state);
    //}

    void Idle_SpinPistol(bool state)
    {
        isIdlingRotate = state;
        animator.SetBool("Rotate", state);
        if (state)
        {
            animator.SetTrigger("Idle2");
        }
    }

    public void Idle_InspectPistol(bool state)
    {
        isIdlingInspect = state;
        if (state)
        {
            animator.SetTrigger("Idle1");
        }
        else
            animator.SetTrigger("InterruptIdle");
    }


    private RaycastHit hit;

    IEnumerator MuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        muzzleFlash.transform.Rotate(new Vector3(0, 0, UnityEngine.Random.Range(0, 359)), Space.Self);
        yield return new WaitForSeconds(0.03f);
        muzzleFlash.SetActive(false);
    }

    //IEnumerator FailsafeReset()
    //{
    //    yield return new WaitForSeconds(2);
    //    allowShot = true;
    //    isSlashing = false;
    //    justShot = false;
    //    followUpSpecial = false;
    //    yield return null;
    //}

    void Shoot()
    {
        if (ammoLoaded > 0)
        {
            animator.SetTrigger("Fire");
            pistolSoundSource.PlayOneShot(shootSound);
            StartCoroutine(MuzzleFlash());
            ammoLoaded--;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, pistolRange, layermask.value, QueryTriggerInteraction.Ignore))
            {
                float randomDamage = UnityEngine.Random.Range(pistolDmgMin, pistolDmgMax);
                Damageable damageableObj = hit.collider.GetComponent<Damageable>();
                if(damageableObj != null)
                    damageableObj.CauseDamage(randomDamage, "pistol");

            }
            allowShot = false;
        }
        else
        {
            pistolSoundSource.PlayOneShot(emptySound);
        }

    }



    public void AddAmmo(int ammogot)
    {
        ammoReserve += ammogot;
    }


    private bool IsBehindTarget(Transform target, float tolerance)
    {
        float dotProduct = Vector3.Dot(target.forward, (transform.position - target.position).normalized);
        bool bIsEnemyFacingAway = (dotProduct < 0);
        float angle = Vector3.Angle(transform.forward, (target.position - transform.position).normalized);
        bool bIsInBackstabAngle = ((angle * 2) <= tolerance);
        return bIsEnemyFacingAway && bIsInBackstabAngle;
    }

    bool RandomBool()
    {
        float rng = 0;
        rng = UnityEngine.Random.value;
        if (rng >= 0.5f)
        {
            return true;
        }
        else
            return false;
    }
}
