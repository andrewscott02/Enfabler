using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour, ICanDealDamage
{
    #region Setup

    protected Animator animator;
    [HideInInspector]
    public ConstructPlayerModel modelConstructor;
    protected Health health;

    public Weapon weapon { get; private set; }
    public Object projectile;
    public Object projectileFX;
    public float projectileSpeed = 40;
    public TrapStats projectileData;

    public bool canMove = true;
    public bool sprinting = false;

    
    public bool canSaveAttackInput = false;
    AttackType lastAttackType = AttackType.None;
    public AttackType savingAttackInput = AttackType.None;
    public AttackType savingChargeInput = AttackType.None;
    public float savedAttackAnimSpeed = 1;
    public enum AttackType
    {
        None, PrimaryAttack, SecondaryAttack, SwitchPrimaryAttack, SwitchSecondaryAttack
    }

    public float chargePrimaryDamageScaling = 2f;
    public float chargeSecondaryDamageScaling = 1f;
    public bool canSwitchAttack = false;
    bool switchAttack = false;
    float currentChargeTime = 0;
    int additionalDamage;
    public float chargeUnblockableTime = 0.6f;
    bool unblockable = false;
    public float chargeMaxTime = 2;

    float baseAnimationSpeed;
    float currentAttackSpeed = 1;
    bool baseUseRootMotion;

    protected SetWeapon setWeapon;

    private void Start()
    {
        animator = GetComponent<Animator>();
        baseUseRootMotion = animator.applyRootMotion;
        baseAnimationSpeed = animator.speed;
        health = GetComponent<Health>();
        ignore.Add(health);
        InvokeRepeating("CurrentTarget", 0, currentTargetCastInterval);
        currentArmour = maxArmour;
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourRegen));
        ForceEndAttack();

        setWeapon = GetComponentInChildren<SetWeapon>();
        SetupWeapon(0);
    }

    private void Update()
    {
        if (chargingAttack != AttackType.None)
        {
            currentChargeTime += Time.deltaTime;

            if (currentChargeTime >= chargeUnblockableTime)
            {
                SetUnblockable();
            }
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

        setWeapon.currentWeapon = weaponIndex;
        this.weapon = setWeapon.CreateWeapon();
    }

    public void ChooseWeapon(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.PrimaryAttack:
                SetupWeapon(0);
                break;
            case AttackType.SwitchPrimaryAttack:
                SetupWeapon(0);
                break;
            case AttackType.SecondaryAttack:
                SetupWeapon(1);
                break;
            case AttackType.SwitchSecondaryAttack:
                SetupWeapon(1);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Attacking

    BaseCharacterController[] lastAttacked;
    public bool canAttack = true;

    #region Attacking -> Attack Inputs

    public AttackType chargingAttack { get; private set; } = AttackType.None;

    GameObject overrideTarget = null;

    public void Attack(float attackSpeed = 1f, bool canCharge = true, AttackType attackType = AttackType.PrimaryAttack, GameObject target = null)
    {
        if (attackType == AttackType.None) return;

        savingAttackInput = AttackType.None;

        if (canAttack)
        {
            overrideTarget = target;

            switchAttack = (lastAttackType != attackType && lastAttackType != AttackType.None);

            ChooseWeapon(attackType);

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
            currentAttackSpeed = attackSpeed;
            animator.speed = attackSpeed;
            animator.applyRootMotion = true;

            if (switchAttack && canSwitchAttack)
            {
                animator.SetTrigger("Switch" + attackType.ToString());
            }
            else
            {
                //Debug.Log(switchAttack ? "Switch" + attackType.ToString() : attackType.ToString());
                animator.SetTrigger(sprinting ? "Sprint" + attackType.ToString() : attackType.ToString());
            }
            sprinting = false;

            lastAttackType = attackType;
            //RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
        }
        else if (canSaveAttackInput)
        {
            savingAttackInput = attackType;
            savedAttackAnimSpeed = attackSpeed;
        }
    }

    void StartCharge(AttackType attackType)
    {
        if (chargeCoroutine != null)
            StopCoroutine(chargeCoroutine);
        chargingAttack = attackType;
    }

    public void ReleaseAttack(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.PrimaryAttack:
                additionalDamage = (int)(currentChargeTime * chargePrimaryDamageScaling);
                //Debug.Log("Primary damage scaling " + additionalDamage);
                break;
            case AttackType.SecondaryAttack:
                additionalDamage = (int)(currentChargeTime * chargeSecondaryDamageScaling);
                //Debug.Log("Secondary damage scaling " + additionalDamage);
                break;
            default:
                break;
        }

        chargingAttack = AttackType.None;
        savingChargeInput = AttackType.None;
        animator.speed = currentAttackSpeed;

        currentChargeTime = 0;

        if (chargeCoroutine != null)
            StopCoroutine(chargeCoroutine);
    }

    public void CheckCharge()
    {
        if (chargingAttack == AttackType.None || animator == null) return;

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

        animator.speed = baseAnimationSpeed;
        ReleaseAttack(chargingAttack);
    }

    void SetUnblockable()
    {
        if (unblockable) { return; }

        weapon.unblockableTrail.SetActive(true);
        unblockable = true;
    }

    public void NextAttack(int attack)
    {
        //Debug.Log("Next attack + " + attack);
        animator.SetInteger("MeleeAttackCount", attack);

        canDodge = true;
        canAttack = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        if (savingAttackInput != AttackType.None)
        {
            //Debug.Log("Saved attack input");
            Attack(attackSpeed: savedAttackAnimSpeed, attackType: savingAttackInput);
            savingAttackInput = AttackType.None;
        }
    }

    public void ResetAttack()
    {
        //Debug.Log("Reset Attack");
        if (animator == null)
            Debug.LogWarning("Animator of " + gameObject.name + " is null");
        animator.SetInteger("MeleeAttackCount", 0);
        animator.SetBool("InHitReaction", false);
        animator.speed = (baseAnimationSpeed);
        lastAttackType = AttackType.None;
        switchAttack = false;
        canMove = true;
        canDodge = true;
        canAttack = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        animator.applyRootMotion = baseUseRootMotion;

        Untarget();
    }

    #endregion

    #region Attacking -> Hit Logic

    List<IDamageable> hitTargets = new List<IDamageable>();
    public List<IDamageable> ignore = new List<IDamageable>();
    public float targetSphereRadius = 3f;

    int damage;

    #region Start and End Attacks

    public void StartAttack(int currentDamage)
    {
        if (weapon != null)
        {
            if (weapon.weaponTrail != null)
                weapon.weaponTrail.SetActive(true);

        }

        //Clear damage and list of enemies hit
        hitTargets.Clear();
        damage = currentDamage + additionalDamage;
        //Debug.Log(damage + " from " + currentDamage + " and " + additionalDamage);

        CheckMoveToTarget(transform.position, transform.forward, layerMask, moveDistanceThreshold.y);

        InvokeRepeating("AttackCheck", 0f, 0.004f);
    }

    public void ForceEndAttack()
    {
        animator.speed = baseAnimationSpeed;
        animator.applyRootMotion = baseUseRootMotion;
        unblockable = false;
        chargingAttack = AttackType.None;
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

        CancelInvoke("AttackCheck");
        canSaveAttackInput = false;
    }

    public void EndAttack()
    {
        if (modelConstructor != null)
        {
            modelConstructor.PlayerAttack(hitTargets.Count > 0);
        }

        Untarget();
        ForceEndAttack();
    }

    Vector3 firePos;
    public float[] additionalShotAngle;
    public float additionalProjectileDamageMultiplier = 0.5f;

    public void FireProjectile(int projectileDamage)
    {
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
            if (Physics.SphereCast(origin, radius: targetSphereRadius, direction: dir, out hit, maxDistance: 20f, layerMask))
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

        if (unblockable && additionalShotAngle != null)
        {
            foreach (var angle in additionalShotAngle)
            {
                //Calculate the length of the opposing angle
                float oppositeLength = Mathf.Tan(angle) * distance;

                //Spawn additional projectiles
                SpawnProjectile(firePos + (transform.right * oppositeLength), (int)(projectileDamage * additionalProjectileDamageMultiplier));
                SpawnProjectile(firePos + (transform.right * -oppositeLength), (int)(projectileDamage * additionalProjectileDamageMultiplier));
            }
            
        }

        unblockable = false;
        chargingAttack = AttackType.None;
        additionalDamage = 0;
    }

    void SpawnProjectile(Vector3 targetPos, int projectileDamage)
    {
        Instantiate(projectileFX, weapon.transform);

        GameObject projectileObj = Instantiate(projectile, weapon.transform.position, transform.rotation) as GameObject;
        ProjectileMovement projectileMove = projectileObj.GetComponent<ProjectileMovement>();
        projectileMove.Fire(targetPos, projectileData, this.gameObject, projectileDamage, projectileSpeed);
    }

    public Vector2 moveDistanceThreshold = new Vector2(0.5f, 5f);
    public float targetSnapSpeed = 0.02f, targetSnapInterval = 0.001f;

    public void CheckMoveToTarget(Vector3 origin, Vector3 dir, LayerMask layerMask, float maxDistance = 5)
    {
        RaycastHit hit;

        if (Physics.SphereCast(origin, radius: targetSphereRadius, direction: dir, out hit, maxDistance: maxDistance, layerMask))
        {
            float distance = Vector3.Distance(origin, hit.point);

            if (distance > moveDistanceThreshold.x)
            {
                MoveToTarget(hit.point);
            }
        }
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

    public float weaponSphereRadius = 0.45f;

    void AttackCheck()
    {
        if (weapon == null)
            return;

        //Raycast between sword base and tip
        RaycastHit hit;

        Vector3 origin = weapon.weaponBaseHit.transform.position;
        float distance = Vector3.Distance(weapon.weaponBaseHit.transform.position, weapon.weaponTipHit.transform.position);
        Vector3 dir = weapon.weaponTipHit.transform.position - weapon.weaponBaseHit.transform.position;

        if (Physics.SphereCast(origin, radius: weaponSphereRadius, direction: dir, out hit, maxDistance: distance, layerMask))
        {
            IDamageable hitDamageable = hit.collider.GetComponent<IDamageable>();

            if (hitDamageable == null)
            {
                hitDamageable = hit.collider.GetComponentInParent<IDamageable>();
            }

            #region Guard Clauses

            //Return if collided object has no health component
            if (hitDamageable == null)
            {
                //Debug.LogWarning("No interface");
                return;
            }

            //Return if it has already been hit or if it should be ignored
            if (hitTargets.Contains(hitDamageable) || ignore.Contains(hitDamageable) || hitDamageable.IsDead())
            {
                //Debug.LogWarning("Ignore");
                return;
            }

            #endregion

            //If it can be hit, deal damage to target and add it to the hit targets list
            hitTargets.Add(hitDamageable);
            DealDamage(hitDamageable, damage, hit.point, hit.normal);

            OnAttackHit();
        }
    }

    public E_DamageEvents DealDamage(IDamageable target, int damage, Vector3 spawnPos, Vector3 spawnRot)
    {
        return target.Damage(this, damage, spawnPos, spawnRot);
    }

    void OnAttackHit()
    {
        Freeze();
        RumbleManager.instance.ControllerRumble(0.2f, 0.85f, 0.25f);
        weapon.bloodTrail.SetActive(true);
        GainArmour(1);
        //TODO: Sound effects
    }

    #endregion

    #region Hit Responses

    public bool HitDodged()
    {
        return true;
    }

    public bool HitBlocked()
    {
        return !unblockable;
    }

    public bool HitParried()
    {
        canAttack = false;
        animator.SetTrigger("HitReactLight");
        return true;
    }

    #endregion

    #region Hit Feedback

    public float hitFreezeTime = 0.15f;

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
        animator.speed = baseAnimationSpeed;
    }

    #endregion

    #endregion

    #region Attacking -> Target Logic

    #region Targetting

    [Header("Targeting")]
    public List<BaseCharacterController> currentTargets;
    List<BaseCharacterController> lastHit = new List<BaseCharacterController>();
    public LayerMask layerMask;
    public float currentTargetCastInterval = 0.6f;
    public float currentTargetCastRadius = 1.5f;
    public float currentTargetCastDistance = 10;

    void CurrentTarget()
    {
        List<BaseCharacterController> hitCharacters = new List<BaseCharacterController>();

        RaycastHit[] hit = Physics.SphereCastAll(transform.position, currentTargetCastRadius, transform.forward, currentTargetCastDistance, layerMask);
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

    void Target(AttackType attackType)
    {
        float distance = 0;

        switch (attackType)
        {
            case AttackType.PrimaryAttack:
                distance = moveDistanceThreshold.y;
                break;
            case AttackType.SecondaryAttack:
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

    #endregion

    #region Blocking

    public SliderScript armourSlider;
    public int maxArmour = 5;
    public int currentArmour { get; private set; }
    public float armourCooldown = 6f;
    public float armourRegen = 3f;

    Coroutine armourRegenCoroutine;

    public virtual void Block(bool blocking, bool parryAvailable = true)
    {
        if (this.blocking == blocking)
            return;

        //Debug.Log("Block changed");
        EndDodge();
        ForceEndAttack();
        this.parrying = blocking && parryAvailable;
        this.blocking = blocking;

        SetupWeapon(0);

        if (modelConstructor != null)
        {
            modelConstructor.PlayerParry(attackers > 0);
        }

        animator.SetInteger("MeleeAttackCount", 0);

        if (animator != null) { animator.SetBool("Blocking", blocking); }
    }

    public void ConsumeArmour()
    {
        if (armourRegenCoroutine != null)
            StopCoroutine(armourRegenCoroutine);

        currentArmour--;
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourCooldown));
        ChangeArmourUI();
    }

    public void GainArmour(int armour)
    {
        currentArmour = Mathf.Clamp(currentArmour + armour, 0, maxArmour);
        ChangeArmourUI();
    }

    public bool CanBlock()
    {
        return blocking && currentArmour > 0;
    }

    IEnumerator IResetArmour(float delay)
    {
        yield return new WaitForSeconds(delay);
        GainArmour(1);
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourRegen));
    }

    void ChangeArmourUI()
    {
        if (armourSlider != null)
            armourSlider.ChangeSliderValue(currentArmour, maxArmour);
    }

    #endregion

    #region Parrying

    public bool blocking { get; protected set; }
    public bool parrying { get; protected set; }

    public void EndParryWindow()
    {
        parrying = false;
    }

    public void ParrySuccess()
    {
        GainArmour(1);
    }

    #endregion

    #region Dodging

    public bool canDodge = true;

    public virtual void Dodge()
    {
        if (canDodge)
        {
            Block(false);
            EndDodge();
            ForceEndAttack();

            if (modelConstructor != null)
            {
                modelConstructor.PlayerDodge(attackers > 0);
            }

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
    }

    public void ResetDodge()
    {
        canDodge = true;
        ResetAttack();
        animator.applyRootMotion = baseUseRootMotion;
    }

    #endregion

    #endregion

    #region Taking Damage

    public bool rumbleOnHit = false;

    public void GotHit()
    {
        //Debug.Log("Got hit, end attack");
        canAttack = false;
        animator.speed = baseAnimationSpeed;
        if (rumbleOnHit)
            RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
    }

    #endregion

    #region Spawn Allies

    public Object allies;

    public void SpawnAllies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos;
            if (!HelperFunctions.GetRandomPointOnNavmesh(transform.position, 10f, 0.5f, 100, out spawnPos))
            {
                spawnPos = transform.position;
            }

            Instantiate(allies, spawnPos, new Quaternion(0, 0, 0, 0));
        }
    }

    #endregion
}