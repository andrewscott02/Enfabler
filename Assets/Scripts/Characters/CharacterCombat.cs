using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

public class CharacterCombat : MonoBehaviour, ICanDealDamage
{
    #region Setup

    protected Animator animator;
    [HideInInspector]
    protected Health health;
    protected BaseCharacterController controller;
    protected CharacterMovement characterMovement;
    protected Attacks attacks;

    [Header("Movement")]
    public bool canMove = true;
    public bool sprinting = false;
    public bool cameraZoom = false;

    #region Attack Data

    #region Attack Input Data

    [Header("Basic Attack Data")]
    public bool canAttack = true;

    float currentChargeTime = 0;
    int additionalDamage;
    public float chargeUnblockableTime = 0.6f;
    bool unblockable = false;
    public float chargeMaxTime = 2;

    [Header("Switch Attack Data")]
    public bool canSwitchAttack = false;
    public float slideInputDelay = 0.2f;
    bool switchAttack = false;

    [Header("Save Input Data")]
    public bool canSaveAttackInput = false;
    E_AttackType lastAttackType = E_AttackType.None;
    public E_AttackType savingAttackInput = E_AttackType.None;
    public E_AttackType savingChargeInput = E_AttackType.None;

    #endregion

    #region Attack Logic Data

    [Header("Target Snap Data")]
    public LayerMask hitLayerMask, snapLayerMask;
    public float targetSphereRadius = 3f;
    public Vector2 moveDistanceThreshold = new Vector2(0.5f, 5f);
    public float targetSnapSpeed = 0.02f, targetSnapInterval = 0.001f;

    [Header("Attack Hit Detection Data")]
    public float currentTargetCastInterval = 0.6f;
    public float currentTargetCastRadius = 1.5f;
    public float currentTargetCastDistance = 10;
    [HideInInspector]
    public List<BaseCharacterController> currentTargets;
    List<BaseCharacterController> lastHit = new List<BaseCharacterController>();

    [Header("Attack Hit Data")]
    public float hitFreezeTime = 0.15f;
    public bool rumbleOnHit = false;

    #endregion

    #region Weapon Data

    [Header("Weapon Data")]
    public bool useGenerousHitDetection = false;
    public float weaponSphereRadius = 0.45f;
    public Weapon weapon { get; private set; }
    protected SetWeapon setWeapon;

    public int maxArrows = 5;
    int m_CurrentArrows = 0;
    int currentArrows
    {
        get
        {
            return m_CurrentArrows;
        }

        set
        {
            m_CurrentArrows = value;

            if (arrowCapacity != null)
            {
                arrowCapacity.SetArrows(m_CurrentArrows);
            }
        }
    }
    ArrowCapacityUI arrowCapacity;
    public float regenArrowDelay = 4f;
    public float regenArrowInterval = 1f;

    #endregion

    #region Animation Data

    public float baseAnimationSpeed { get; private set; }
    public float currentAnimationSpeed { get; private set; }

    public void SetSpeed(float newSpeed)
    {
        currentAnimationSpeed = newSpeed;
        animator.speed = currentAnimationSpeed;
    }

    public void ResetAnimSpeed()
    {
        currentAnimationSpeed = baseAnimationSpeed;
        animator.speed = currentAnimationSpeed;
    }

    float currentAttackSpeed = 1;
    bool baseUseRootMotion;

    #endregion

    #endregion

    #region Methods

    private void Start()
    {
        controller = GetComponent<BaseCharacterController>();
        characterMovement = GetComponent<CharacterMovement>();
        animator = GetComponent<Animator>();
        animator.SetBool("UseFastRoll", fastdodge);
        baseUseRootMotion = animator.applyRootMotion;
        baseAnimationSpeed = animator.speed;
        ResetAnimSpeed();
        health = GetComponent<Health>();
        ignore.Add(health);
        InvokeRepeating("CurrentTarget", 0, currentTargetCastInterval);
        ForceEndAttack();
        attacks = GetComponent<Attacks>();

        setWeapon = GetComponentInChildren<SetWeapon>();
        SetupWeapon(0);

        arrowCapacity = GetComponentInChildren<ArrowCapacityUI>();
        SetupArrows();

        onAttackHit += OnAttackHit;
        hitParry += HitParry;

        canBlockDelegate += CanBlockDelegateCheck;
        blockingDelegate += Blocking;
        blockedDelegate += BlockedHit;
        parriedDelegate += ParrySuccess;

        canDodgeDelegate += CanDodgeDelegateCheck;
        dodgeStartDelegate += OnDodgeStart;
        dodgeEndDelegate += OnDodgeEnd;
        phaseDelegate += Phase;
    }

    private void Update()
    {
        if (chargingAttack != E_AttackType.None)
        {
            currentChargeTime += Time.deltaTime;

            if (currentChargeTime >= chargeUnblockableTime)
            {
                SetUnblockable();
            }
        }

        if (attackCheck)
        {
            AttackCheck();
        }
    }

    private void FixedUpdate()
    {
        if (attackCheck)
        {
            AttackCheck();
        }
    }

    public MonoBehaviour GetScript()
    {
        return this;
    }

    public void SetupAllies(List<BaseCharacterController> allies)
    {
        foreach (var item in allies)
        {
            ignore.Add(item.GetHealth());
        }
    }

    public void SetupWeapon(int weaponIndex)
    {
        if (setWeapon.currentWeapon == weaponIndex) return;

        //Main Hand
        this.weapon = setWeapon.CreateWeapon(weaponIndex, 0, setWeapon.weapons);
        //Offhand
        setWeapon.CreateWeapon(weaponIndex, 1, setWeapon.offhandWeapons);
    }

    public void ForceSetupWeapon(int weaponIndex)
    {
        //Main Hand
        this.weapon = setWeapon.CreateWeapon(weaponIndex, 0, setWeapon.weapons);
        //Offhand
        setWeapon.CreateWeapon(weaponIndex, 1, setWeapon.offhandWeapons);
    }

    #endregion

    #endregion

    #region Attacking

    BaseCharacterController[] lastAttacked;

    AttackTypes currentAttack = new AttackTypes() { attackType = E_AttackType.None};
    int currentAttackIndex = 0;

    #region Attacking -> Attack Inputs

    public E_AttackType chargingAttack { get; private set; } = E_AttackType.None;

    GameObject overrideTarget = null;

    public void Attack(bool canCharge = true, E_AttackType attackType = E_AttackType.PrimaryAttack, GameObject target = null, bool enableModifiers = true, bool interupt = false)
    {
        if (attackType == E_AttackType.None) return;

        #region Ammo Checks

        if (switchAttack && canSwitchAttack)
        {
            E_AttackType attack = (E_AttackType)System.Enum.Parse(typeof(E_AttackType), "Switch" + attackType.ToString());
            if (attacks.GetAttackData(attack).arrowCost)
            {
                if (!CanShoot()) return;
            }
        }
        else
        {
            E_AttackType attack = (E_AttackType)System.Enum.Parse(typeof(E_AttackType), sprinting && enableModifiers ? "Sprint" + attackType.ToString() : attackType.ToString());
            if (attacks.GetAttackData(attack).arrowCost)
            {
                if (!CanShoot()) return;
            }
        }

        #endregion

        #region Slide Input

        if (enableModifiers)
        {
            switchAttack = performedSlideInput || (GetSlideInput(attackType) && canSlideInput);

            performedSlideInput = GetSlideInput(attackType) && canSlideInput;

            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);
            slideCoroutine = StartCoroutine(ISlideInput(slideInputDelay));
        }

        #endregion

        savingAttackInput = E_AttackType.None;

        if (canAttack || interupt)
        {
            overrideTarget = target;

            //Debug.Log(switchAttack ? "Switch" + attackType.ToString() : attackType.ToString());

            //Debug.Log(attackType + " " + animator.GetInteger("MeleeAttackCount") + (sprinting?" Sprint":" Standard") + " " + attackSpeed);
            Block(false);
            EndDodge();
            ForceEndAttack();

            if (canCharge && savingChargeInput == attackType)
                StartCharge(attackType);

            Target(attackType);

            blocking = false;
            canMove = false;
            canAttack = false;
            canSaveAttackInput = true;
            canDodge = false;
            animator.applyRootMotion = true;

            if (riposteCoroutine != null)
            {
                StopCoroutine(riposteCoroutine);
                riposteCoroutine = null;
            }

            //Debug.Log(switchAttack ? "Switch" + attackType.ToString() : attackType.ToString());

            if (switchAttack && canSwitchAttack)
            {
                E_AttackType attack = (E_AttackType)System.Enum.Parse(typeof(E_AttackType), "Switch" + attackType.ToString());
                InitiateAttack(attacks.GetAttackData(attack));
            }
            else
            {
                E_AttackType attack = (E_AttackType)System.Enum.Parse(typeof(E_AttackType), sprinting && enableModifiers ? "Sprint" + attackType.ToString() : attackType.ToString());
                InitiateAttack(attacks.GetAttackData(attack));
            }
            sprinting = false;

            lastAttackType = attackType;
            //RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
        }
        else if (canSaveAttackInput)
        {
            savingAttackInput = attackType;
        }
    }

    public void InitiateAttack(AttackTypes attackData)
    {
        currentAttack = attackData;
        currentAttackIndex = attacks.GetVariation(currentAttack.attackType, currentAttackIndex);
        SetupWeapon(currentAttack.weaponIndex);

        currentAttackSpeed = currentAttack.variations[currentAttackIndex].attackSpeed;
        animator.speed = currentAttackSpeed * currentAnimationSpeed;

        animator.SetInteger("MeleeAttackCount", currentAttack.variations[currentAttackIndex].currentAttackAnimModifier);
        animator.SetTrigger(currentAttack.variations[currentAttackIndex].animation.ToString());
    }

    void StartCharge(E_AttackType attackType)
    {
        if (chargeCoroutine != null)
            StopCoroutine(chargeCoroutine);
        chargingAttack = attackType;
    }

    public void ReleaseAttack()
    {
        if (currentAttack.attackType == E_AttackType.None || currentAttackIndex >= currentAttack.variations.Length) return;

        additionalDamage = (int)(currentChargeTime * currentAttack.variations[currentAttackIndex].chargeDamageScaling);

        chargingAttack = E_AttackType.None;
        savingChargeInput = E_AttackType.None;
        animator.speed = currentAttackSpeed * currentAnimationSpeed;

        currentChargeTime = 0;

        if (chargeCoroutine != null)
            StopCoroutine(chargeCoroutine);
    }

    public void CheckCharge()
    {
        if (chargingAttack == E_AttackType.None || animator == null || currentAttack.variations[currentAttackIndex].canCharge == false)
        {
            animator.speed = currentAnimationSpeed;

            ReleaseAttack();
            return;
        }

        if (chargeCoroutine != null)
            StopCoroutine(chargeCoroutine);
        currentChargeTime = 0;

        chargeCoroutine = StartCoroutine(IReleaseAttack(chargeMaxTime));
    }

    Coroutine chargeCoroutine;

    IEnumerator IReleaseAttack(float delay)
    {
        animator.speed = 0;

        yield return new WaitForSecondsRealtime(delay);

        animator.speed = currentAnimationSpeed;
        ReleaseAttack();
    }

    void SetUnblockable()
    {
        if (unblockable || currentAttack.variations[currentAttackIndex].canCharge == false) { return; }

        if (currentAttack.variations[currentAttackIndex].overideChargeAttack)
        {
            EndAttack();
            currentAttackIndex = currentAttack.variations[currentAttackIndex].chargedAttackIndex;
            InitiateAttack(currentAttack);
        }
        else
        {
            PlaySoundEffect(weapon.chargeClip, weapon.chargeVolume);

            weapon.unblockableTrail.SetActive(true);
            unblockable = true;
        }
    }

    public void NextAttack()
    {
        //Debug.Log("Next attack + " + attack);
        if (currentAttack.attackType == E_AttackType.None || currentAttackIndex >= currentAttack.variations.Length)
            currentAttackIndex = 0;
        else
        {
            if (lastAttackEvent == E_DamageEvents.Hit && currentAttack.variations[currentAttackIndex].overrideNextAttackOnHit)
            {
                currentAttackIndex = currentAttack.variations[currentAttackIndex].nextAttackHitIndex;
            }
            else
            {
                currentAttackIndex = currentAttack.variations[currentAttackIndex].nextAttackIndex;
            }
        }

        lastAttackEvent = E_DamageEvents.Dodge;

        canDodge = true;
        canAttack = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        if (savingAttackInput != E_AttackType.None)
        {
            //Debug.Log("Saved attack input");
            Attack(attackType: savingAttackInput);
            savingAttackInput = E_AttackType.None;
        }
    }

    public void ResetAttack()
    {
        //Debug.Log("Reset Attack");
        if (animator == null)
            Debug.LogWarning("Animator of " + gameObject.name + " is null");
        currentAttackIndex = 0;
        
        animator.SetBool("InHitReaction", false);
        animator.speed = currentAnimationSpeed;
        lastAttackType = E_AttackType.None;
        switchAttack = false;
        canMove = true;
        canDodge = true;
        canAttack = true;
        performedSlideInput = false;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        animator.applyRootMotion = baseUseRootMotion;

        Untarget();
    }

    Coroutine slideCoroutine;
    bool performedSlideInput = false;
    bool canSlideInput = false;

    bool GetSlideInput(E_AttackType attackType)
    {
        bool validAttacks = attacks.GetAttackData(lastAttackType).rangedInput! | attacks.GetAttackData(attackType).rangedInput;

        if (!validAttacks)
            return false;

        bool slideInput = (lastAttackType != attackType && lastAttackType != E_AttackType.None);

        return slideInput;
    }

    IEnumerator ISlideInput(float delay)
    {
        canSlideInput = true;

        yield return new WaitForSeconds(delay);

        canSlideInput = false;
    }

    #endregion

    #region Attacking -> Hit Logic

    List<IDamageable> hitTargets = new List<IDamageable>();
    public List<IDamageable> ignore = new List<IDamageable>();
    
    int damage;

    #region Start and End Attacks

    bool attackCheck = false;

    public void StartAttack()
    {
        if (weapon != null)
        {
            if (weapon.weaponTrail != null)
                weapon.weaponTrail.SetActive(true);

            if (currentAttack.variations[currentAttackIndex].autoUnblockable)
            {
                PlaySoundEffect(weapon.chargeClip, weapon.chargeVolume);

                weapon.unblockableTrail.SetActive(true);
                unblockable = true;
            }
        }

        //Clear damage and list of enemies hit
        hitTargets.Clear();
        damage = currentAttack.variations[currentAttackIndex].damage + additionalDamage;
        //Debug.Log(damage + " from " + currentDamage + " and " + additionalDamage);

        CheckMoveToTarget(transform.position + (transform.forward * targetSphereRadius), transform.forward, snapLayerMask, moveDistanceThreshold.y);

        //InvokeRepeating("AttackCheck", 0f, 0.0001f);
        attackCheck = true;

        PlaySoundEffect(weapon.attackClip, weapon.soundVolume);
    }

    public void ForceEndAttack()
    {
        animator.speed = currentAnimationSpeed;
        animator.applyRootMotion = baseUseRootMotion;
        unblockable = false;
        chargingAttack = E_AttackType.None;
        additionalDamage = 0;

        //Clear damage and list of enemies hit
        if (weapon != null)
        {
            if (weapon.weaponTrail != null)
                weapon.weaponTrail.SetActive(false);

            if (weapon.bloodTrail != null)
                weapon.bloodTrail.SetActive(false);

            if (weapon.unblockableTrail != null)
                weapon.unblockableTrail.SetActive(false);
        }

        hitTargets.Clear();
        damage = 0;

        //CancelInvoke("AttackCheck");
        attackCheck = false;
        canSaveAttackInput = false;
    }

    public void EndAttack()
    {
        Untarget();
        ForceEndAttack();
    }

    public void CheckMoveToTarget(Vector3 origin, Vector3 dir, LayerMask layerMask, float maxDistance = 5)
    {
        RaycastHit hit;

        Debug.Log("Combat - Checking move to target");

        if (Physics.SphereCast(origin, radius: targetSphereRadius, direction: dir, out hit, maxDistance: maxDistance, layerMask))
        {
            //Debug.Log("Combat - Hit Object " + hit.collider.gameObject.name);
            float distance = Vector3.Distance(origin, hit.point);

            RotateTowardsTarget(hit.point);

            if (distance > moveDistanceThreshold.x)
            {
                //Debug.Log("Combat - Moving to target");
                MoveToTarget(hit.point);
            }
        }
        else
        {
            Collider[] cols = Physics.OverlapSphere(origin, targetSphereRadius, layerMask);
            if (cols.Length > 0)
            {
                foreach (var item in cols)
                {
                    if (item.gameObject != gameObject)
                    {
                        //Debug.Log("Combat - Hit Object " + item.gameObject.name);
                        float distance = Vector3.Distance(origin, item.gameObject.transform.position);

                        RotateTowardsTarget(item.gameObject.transform.position);

                        if (distance > moveDistanceThreshold.x)
                        {
                            //Debug.Log("Combat - Moving to target");
                            MoveToTarget(item.gameObject.transform.position);
                        }

                        return;
                    }
                }
            }

            MoveToTarget(transform.position - (dir * moveDistanceThreshold.x));
        }
    }

    public void RotateTowardsTarget(Vector3 target)
    {
        //Debug.Log("Combat - Rotating");
        //Rotate towards direction
        Vector3 desiredRot = new Vector3(target.x, transform.position.y, target.z);
        Quaternion newRot = Quaternion.LookRotation(desiredRot - transform.position, Vector3.up);
        characterMovement.targetRotation = newRot;
    }

    void MoveToTarget(Vector3 target)
    {
        if (moveDistanceThreshold.y <= 0) return;

        Vector3 targetPos = HelperFunctions.GetFlankingPoint(transform.position, target, -1f);

        if (transform.position != targetPos)
            ISnap = StartCoroutine(ISnapToTarget(transform.position, targetPos, targetSnapSpeed, targetSnapInterval));
        else if (ISnap != null)
            StopCoroutine(ISnap);
    }

    Coroutine ISnap;

    IEnumerator ISnapToTarget(Vector3 originalPos, Vector3 targetPos, float lerpSpeed, float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.position = HelperFunctions.LerpVector3(originalPos, targetPos, lerpSpeed);
        //Debug.Log("OP:" + originalPos + "CP:" + transform.position + "TP:" + targetPos);

        if (transform.position != targetPos)
            ISnap = StartCoroutine(ISnapToTarget(originalPos, targetPos, lerpSpeed + lerpSpeed, delay));
        else if (ISnap != null)
            StopCoroutine(ISnap);
    }

    private void OnDrawGizmos()
    {
        if (firePos != null)
        {
            Gizmos.DrawWireSphere(firePos, targetSphereRadius);
        }
    }

    #endregion

    #region Hit Checks

    void AttackCheck()
    {
        if (weapon == null)
            return;

        AttackOverlap(useGenerousHitDetection ? weapon.weaponBaseHit.transform.position : weapon.weaponBase.transform.position);
        AttackTrace(useGenerousHitDetection ? weapon.weaponBaseHit.transform.position : weapon.weaponBase.transform.position, useGenerousHitDetection ? weapon.weaponTipHit.transform.position : weapon.weaponTip.transform.position);
    }

    void AttackOverlap(Vector3 origin)
    {
        Collider[] cols = Physics.OverlapSphere(origin, weaponSphereRadius, hitLayerMask);
        foreach (var item in cols)
        {
            if (item.gameObject != gameObject)
            {
                CallDamage(item, item.transform.position, Vector3.zero);
            }
        }
    }

    void AttackTrace(Vector3 start, Vector3 end)
    {
        //Raycast between sword base and tip
        RaycastHit hit;

        Vector3 origin = useGenerousHitDetection ? start : end;
        float distance = Vector3.Distance(start, end);
        Vector3 dir = end - start;

        if (Physics.SphereCast(origin, radius: weaponSphereRadius, direction: dir, out hit, maxDistance: distance, hitLayerMask))
        {
            CallDamage(hit.collider, hit.point, hit.normal);
        }
    }

    void CallDamage(Collider collider, Vector3 hitPos, Vector3 hitNormal)
    {
        IDamageable hitDamageable = collider.GetComponent<IDamageable>();

        if (hitDamageable == null)
        {
            hitDamageable = collider.GetComponentInParent<IDamageable>();
        }

        #region Guard Clauses

        //Return if collided object has no health component
        if (hitDamageable == null)
        {
            //Debug.LogWarning("No interface");
            //AttackTrace(hit.point, end);
            return;
        }

        //Return if it has already been hit or if it should be ignored
        if (hitTargets.Contains(hitDamageable) || ignore.Contains(hitDamageable) || hitDamageable.IsDead())
        {
            //Debug.LogWarning("Ignore " + hitDamageable.GetScript().gameObject.name);
            //AttackTrace(hit.point, end);
            return;
        }

        #endregion

        //If it can be hit, deal damage to target and add it to the hit targets list
        hitTargets.Add(hitDamageable);
        E_DamageEvents damageEvent = DealDamage(hitDamageable, damage, hitPos, hitNormal, currentAttack.attackType);

        onAttackHit(damageEvent);
    }

    public E_DamageEvents DealDamage(IDamageable target, int damage, Vector3 spawnPos, Vector3 spawnRot, E_AttackType attackType = E_AttackType.None)
    {
        return target.Damage(this, damage, spawnPos, spawnRot, attackType);
    }

    public delegate void AttackDeletate(E_DamageEvents damageEvents);
    public AttackDeletate onAttackHit;

    E_DamageEvents lastAttackEvent = E_DamageEvents.Dodge;

    void OnAttackHit(E_DamageEvents damageEvent)
    {
        Freeze();
        RumbleManager.instance.ControllerRumble(0.2f, 0.85f, 0.25f);

        lastAttackEvent = damageEvent;
        bool hit = damageEvent == E_DamageEvents.Hit;

        PlaySoundEffect(hit ? weapon.hitClip : weapon.blockClip, weapon.soundVolume);

        if (hit)
        {
            weapon.bloodTrail.SetActive(true);
        }
    }

    #endregion

    #region Hit Responses

    public delegate void HitResponseDelegates(Vector3 dir);
    public HitResponseDelegates hitBlock, hitParry;

    public bool HitDodged()
    {
        return true;
    }

    public bool HitBlocked(IDamageable other)
    {
        return !unblockable;
    }

    public bool HitParried(IDamageable other)
    {
        canAttack = false;
        animator.SetTrigger("HitReactLight");

        Vector3 dir = transform.position - other.GetScript().transform.position;
        dir.Normalize();

        hitParry(dir);

        return true;
    }

    void HitParry(Vector3 dir)
    {
        //Empty delegate
    }

    #endregion

    #region Hit Feedback

    void Freeze()
    {
        if (animator == null) return;

        StartCoroutine(IFreeze(hitFreezeTime));
    }

    IEnumerator IFreeze(float delay)
    {
        //Debug.Log("Freeze");
        animator.speed = 0;
        yield return new WaitForSecondsRealtime(delay);
        //Debug.Log("Unfreeze");
        animator.speed = currentAnimationSpeed;
    }

    #endregion

    #endregion

    #region Attacking -> Target Logic

    #region Targetting

    void CurrentTarget()
    {
        List<BaseCharacterController> hitCharacters = new List<BaseCharacterController>();

        RaycastHit[] hit = Physics.SphereCastAll(transform.position, currentTargetCastRadius, transform.forward, currentTargetCastDistance, snapLayerMask);
        foreach (RaycastHit item in hit)
        {
            //Debug.Log("Ray hit " + item.collider.gameObject.name);
            BaseCharacterController character = item.collider.transform.gameObject.GetComponent<BaseCharacterController>();

            if (character != null)
            {
                if (AIManager.instance == null)
                {
                    hitCharacters.Add(character);
                }
                else if (AIManager.instance.OnSameTeam(GetComponent<BaseCharacterController>(), character) == false)
                    hitCharacters.Add(character);
            }
        }

        currentTargets = hitCharacters;
    }

    void Target(E_AttackType attackType)
    {
        float distance = 0;

        switch (attackType)
        {
            case E_AttackType.PrimaryAttack:
                distance = moveDistanceThreshold.y;
                break;
            case E_AttackType.SecondaryAttack:
                distance = 20f;
                break;
            default:
                break;
        }

        lastHit.Clear();
        foreach (var item in currentTargets)
        {
            if (item == null) break;

            if (Vector3.Distance(transform.position, item.transform.position) < distance)
            {
                if (item.GetCharacterCombat() != null)
                {
                    item.GetCharacterCombat().StartBeingAttacked();
                }
                lastHit.Add(item);
            }
        }
    }

    void Untarget()
    {
        foreach (var item in lastHit)
        {
            if (item.GetCharacterCombat() != null)
            {
                item.GetCharacterCombat().StopBeingAttacked();
            }
        }

        lastHit.Clear();
    }

    #endregion

    #region Targetted

    int attackers = 0; public bool GetTargetted() { return attackers > 0; }

    public void StartBeingAttacked()
    {
        attackers++;
    }

    public void StopBeingAttacked()
    {
        attackers--;
    }

    #endregion

    #endregion

    #region Attacking -> Ranged Attacks

    #region Fire Projectile

    Vector3 firePos;

    public void FireProjectile()
    {
        if (currentAttack.variations[currentAttackIndex].projectileData == null)
            return;

        ConsumeArrow();

        PlaySoundEffect(weapon.attackClip, weapon.soundVolume);

        int projectileDamage = currentAttack.variations[currentAttackIndex].damage;
        projectileDamage += additionalDamage;
        //Debug.Log("Fire projectile for " + projectileDamage);

        //Clear damage and list of enemies hit
        if (weapon != null)
        {
            if (weapon.weaponTrail != null)
                weapon.weaponTrail.SetActive(false);

            if (weapon.bloodTrail != null)
                weapon.bloodTrail.SetActive(false);

            if (weapon.unblockableTrail != null)
                weapon.unblockableTrail.SetActive(false);
        }

        //Raycast to target
        RaycastHit hit;

        Vector3 origin = weapon.weaponBaseHit.transform.position;
        float distance = 10f;
        Vector3 dir = transform.forward;

        firePos = origin + (dir * 15f);

        if (overrideTarget == null)
        {
            if (Physics.SphereCast(origin, radius: targetSphereRadius, direction: dir, out hit, maxDistance: 20f, snapLayerMask))
            {
                //Debug.Log("Hit: " + hit.collider.gameObject);
                firePos = hit.point;
                distance = Vector3.Distance(origin, firePos);
            }
            else if (Physics.SphereCast(origin, radius: targetSphereRadius, direction: dir, out hit, maxDistance: 20f, hitLayerMask))
            {
                //Debug.Log("Hit: " + hit.collider.gameObject);
                firePos = hit.point;
                distance = Vector3.Distance(origin, firePos);
            }
        }
        else
        {
            Collider col = overrideTarget.GetComponent<Collider>();
            Vector3 hitPos = overrideTarget.transform.position;

            if (col != null)
                hitPos = col.bounds.center;

            firePos = hitPos;
            distance = Vector3.Distance(origin, firePos);
        }

        SpawnProjectile(firePos, projectileDamage);

        if (unblockable && currentAttack.variations[currentAttackIndex].additionalShotAngle != null)
        {
            foreach (var angle in currentAttack.variations[currentAttackIndex].additionalShotAngle)
            {
                //Calculate the length of the opposing angle
                float oppositeLength = Mathf.Tan(angle) * distance;

                //Spawn additional projectiles
                SpawnProjectile(firePos + (transform.right * oppositeLength), (int)(projectileDamage * currentAttack.variations[currentAttackIndex].additionalProjectileDamageMultiplier));
                SpawnProjectile(firePos + (transform.right * -oppositeLength), (int)(projectileDamage * currentAttack.variations[currentAttackIndex].additionalProjectileDamageMultiplier));
            }

        }

        unblockable = false;
        chargingAttack = E_AttackType.None;
        additionalDamage = 0;
    }

    void SpawnProjectile(Vector3 targetPos, int projectileDamage)
    {
        Instantiate(currentAttack.variations[currentAttackIndex].projectileFX, weapon.transform);

        GameObject projectileObj = Instantiate(currentAttack.variations[currentAttackIndex].projectileData.projectile, weapon.transform.position, transform.rotation) as GameObject;

        ProjectileHit projectileHit = projectileObj.GetComponentInChildren<ProjectileHit>();
        projectileHit.attackType = currentAttack.attackType;

        ProjectileMovement projectileMove = projectileObj.GetComponent<ProjectileMovement>();
        projectileMove.Fire(targetPos, currentAttack.variations[currentAttackIndex].projectileData, this.gameObject, projectileDamage);
    }

    #endregion

    #region Arrow Capacity

    void SetupArrows()
    {
        currentArrows = maxArrows;
        regenArrowsCoroutine = StartCoroutine(IRegenArrows(regenArrowInterval));
    }

    bool CanShoot()
    {
        return currentArrows > 0;
    }

    void ConsumeArrow()
    {
        if (!currentAttack.arrowCost)
            return;

        currentArrows = Mathf.Clamp(currentArrows - 1, 0, maxArrows);
        StopCoroutine(regenArrowsCoroutine);
        regenArrowsCoroutine = StartCoroutine(IRegenArrows(regenArrowDelay));
    }

    void RegenArrows()
    {
        currentArrows = Mathf.Clamp(currentArrows + 1, 0, maxArrows);
        StopCoroutine(regenArrowsCoroutine);
        regenArrowsCoroutine = StartCoroutine(IRegenArrows(regenArrowInterval));
    }

    Coroutine regenArrowsCoroutine;

    IEnumerator IRegenArrows(float delay)
    {
        yield return new WaitForSeconds(delay);
        RegenArrows();
    }

    #endregion

    #endregion

    #endregion

    #region Blocking

    public delegate void BlockDelegate(bool blocking);
    public BlockDelegate blockingDelegate;

    public delegate void BlockedDelegate();
    public BlockedDelegate blockedDelegate;

    Coroutine armourRegenCoroutine;

    public virtual void Block(bool blocking, bool parryAvailable = true)
    {
        if (this.blocking == blocking)
            return;

        //Debug.Log("Block changed");
        EndDodge();
        ForceEndAttack();
        SetupWeapon(0);
        if (blocking && parryAvailable)
        {
            StartParryWindow();
        }
        else
        {
            EndParryWindow();
        }
        this.blocking = blocking;

        animator.SetInteger("MeleeAttackCount", 0);

        if (animator != null) { animator.SetBool("Blocking", blocking); }

        blockingDelegate(blocking);
    }

    public void Blocking(bool blocking)
    {
        //Empty delegate for activating blocking
    }

    public void BlockedHit()
    {
        //Empty delegate for blocking hits
    }

    public delegate bool CanBlockDelegate();
    public CanBlockDelegate canBlockDelegate;

    public bool CanBlock()
    {
        var invocations = canBlockDelegate.GetInvocationList();

        bool canBlock = true;

        for (int i = 0; i < invocations.Length; i++)
        {
            bool current = ((CanBlockDelegate)invocations[i]).Invoke();

            //Debug.Log("Invocation + " + i + " is " + current);

            if (current == false)
                canBlock = false;
        }

        return canBlock;
    }

    public bool CanBlockDelegateCheck()
    {
        return blocking;
    }

    #endregion

    #region Parrying

    public float parryWindowDuration = 0.2f;
    public bool blocking { get; protected set; }
    public bool parrying { get; protected set; }

    public void StartParryWindow()
    {
        parrying = true;
        weapon.ParryEffect(true);

        if (endParryCoroutine != null)
            StopCoroutine(endParryCoroutine);
        endParryCoroutine = StartCoroutine(IDelayEndParryWindow());
    }

    Coroutine endParryCoroutine;

    IEnumerator IDelayEndParryWindow()
    {
        yield return new WaitForSeconds(parryWindowDuration);

        EndParryWindow();
    }

    public void EndParryWindow()
    {
        parrying = false;
        weapon.ParryEffect(false);
    }

    public delegate void ParryDelegate(ICanDealDamage attacker);
    public ParryDelegate parriedDelegate;

    public void ParrySuccess(ICanDealDamage attacker)
    {
        currentArrows = Mathf.Clamp(currentArrows + 1, 0, maxArrows);

        //Empty delegate
        if (cameraZoom)
            CameraManager.instance.CombatZoom();

        if (riposteCoroutine != null)
        {
            StopCoroutine(riposteCoroutine);
            riposteCoroutine = null;
        }

        riposteCoroutine = StartCoroutine(IEnableRiposte(1f));

        animator.SetBool("InHitReaction", true);
        animator.SetTrigger("ParryReact");

        Vector3 attackerPos = attacker.GetScript().gameObject.transform.position;

        Vector3 desiredRot = new Vector3(attackerPos.x, transform.position.y, attackerPos.z);
        Quaternion newRot = Quaternion.LookRotation(desiredRot - transform.position, Vector3.up);
        transform.rotation = newRot;
        RotateTowardsTarget(attackerPos);

        MoveToTarget(transform.position + ((attackerPos - transform.position).normalized * moveDistanceThreshold.y));
    }

    Coroutine riposteCoroutine;

    IEnumerator IEnableRiposte(float delay)
    {
        currentAttackIndex = 3;
        yield return new WaitForSeconds(delay);
        currentAttackIndex = 0;
    }

    #endregion

    #region Dodging

    public bool canDodge = true;
    public bool fastdodge = false;

    public delegate void DodgeDelegate();
    public DodgeDelegate dodgeStartDelegate, dodgeEndDelegate;

    public void OnDodgeStart()
    {
        //Empty delegate
    }

    public void OnDodgeEnd()
    {
        //Empty delegate
    }

    public delegate bool CanDodgeDelegate();
    public CanDodgeDelegate canDodgeDelegate;

    public bool CanDodge()
    {
        var invocations = canDodgeDelegate.GetInvocationList();

        bool canDodgeCheck = true;

        for (int i = 0; i < invocations.Length; i++)
        {
            bool current = ((CanDodgeDelegate)invocations[i]).Invoke();

            //Debug.Log("Invocation + " + i + " is " + current);

            if (current == false)
                canDodgeCheck = false;
        }

        return canDodgeCheck;
    }

    public bool CanDodgeDelegateCheck()
    {
        return canDodge || canSlideInput;
    }

    public virtual void Dodge()
    {
        if (!CanDodge())
            return;

        if (canSlideInput)
        {
            Debug.Log("Lunge input");
            dodgeStartDelegate();
            Block(false);
            ForceEndAttack();
            Attack(attackType: E_AttackType.LungeAttack, enableModifiers: false, interupt: true);
            return;
        }

        if (canDodge)
        {
            dodgeStartDelegate();
            Block(false);
            ForceEndAttack();

            dodging = false;
            canMove = false;
            canAttack = false;
            canSaveAttackInput = true;
            canDodge = false;
            if (animator != null)
            {
                animator.applyRootMotion = true;
                animator.SetTrigger("Dodge"); 
            }
        }
    }

    #region Dodge -> Logic

    protected bool dodging;

    public bool GetDodging() { return dodging; }

    public void StartDodge()
    {
        dodging = true;
    }

    public void EndDodge()
    {
        dodging = false;
        canMove = true;
        canDodge = true;
        canAttack = true;
        canSaveAttackInput = false;

        dodgeEndDelegate();
    }

    public void ResetDodge()
    {
        canDodge = true;
        ResetAttack();
        animator.applyRootMotion = baseUseRootMotion;
    }

    public void EnableDodgeAttack()
    {
        //Debug.Log("Next attack + " + attack);
        currentAttackIndex = 0;

        canDodge = true;
        canAttack = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        if (savingAttackInput != E_AttackType.None)
        {
            //Debug.Log("Saved attack input");
            E_AttackType attackType = (E_AttackType)System.Enum.Parse(typeof(E_AttackType), "Dodge" + savingAttackInput.ToString());
            Attack(attackType: attackType, enableModifiers: false);
            savingAttackInput = E_AttackType.None;
        }
    }

    #endregion

    #endregion

    #region Phasing

    public delegate void PhaseDelegate(bool activate);
    public PhaseDelegate phaseDelegate;

    public void StartPhase()
    {
        dodging = true;
        phaseDelegate(true);

        if (cameraZoom)
            CameraManager.instance.CombatZoom();
    }

    public void EndPhase()
    {
        dodging = false;
        phaseDelegate(false);
    }

    void Phase(bool activate)
    {
        //Empty delegate function for phasing
    }

    #endregion

    #region Taking Damage

    public void GotHit()
    {
        //Debug.Log("Got hit, end attack");
        canAttack = false;
        animator.speed = currentAnimationSpeed;
        if (rumbleOnHit)
            RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
    }

    #endregion

    #region CastSpell

    [HideInInspector]
    public SpellStats currentSpell;

    public void CastSpell(SpellStats prepareSpell, GameObject overrideTarget)
    {
        if (prepareSpell == null && !canAttack) return;

        //Debug.Log("Casting + " + prepareSpell.spellName);
        Block(false);
        EndDodge();
        ForceEndAttack();

        blocking = false;
        canMove = false;
        canAttack = false;
        canSaveAttackInput = false;
        canDodge = false;
        animator.speed = currentAnimationSpeed;
        animator.applyRootMotion = baseUseRootMotion;

        currentSpell = prepareSpell;

        animator.SetTrigger("CastSpell");

        if (overrideTarget != null)
        {
            spellTarget = overrideTarget;
        }
        else
        {
            //TODO: Get target here
        }
    }

    GameObject spellTarget;

    public void ActivateSpell()
    {
        if (currentSpell == null) return;

        currentSpell.CastSpell(controller, spellTarget);
    }

    #endregion

    #region Sound Effects

    void PlaySoundEffect(AudioClip clip, float volume)
    {
        if (AudioManager.instance == null) return;

        AudioManager.instance.PlaySoundEffect(clip, volume);
    }

    #endregion
}