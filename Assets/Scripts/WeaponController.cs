using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weapon;
    public float withdrawDelay;
    public Animator weaponAnimator;
    public float chainAttackWindow;
    public WeaponHitbox weaponHitbox;

    private bool drewWeapon = false;
    private bool canAttack = true;
    private Coroutine withdrawCoroutine;
    private Coroutine chainAttackCoroutine;
    private bool canChainAttack = false;
    private int currentAttackIndex = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!drewWeapon) DrawWeapon();
            else if (drewWeapon && (canAttack || canChainAttack)) PerformAttack();
        }
        else if (Input.GetMouseButtonUp(0)) StopAttackAnimation();
    }


    private void DrawWeapon()
    {
        weaponAnimator.SetTrigger("Draw");
        drewWeapon = true;
        canAttack = true;
        ResetWithdrawTimer();
    }

    private void PerformAttack()
    {
        if (canAttack || canChainAttack)
        {
            currentAttackIndex = (currentAttackIndex % 3) + 1;
            ExecuteAttack();
        }
    }

    private void ExecuteAttack()
    {
        switch (currentAttackIndex)
        {
            case 1:
                weaponAnimator.SetTrigger("Attack1");
                break;
            case 2:
                weaponAnimator.SetTrigger("Attack2");
                break;
            case 3:
                weaponAnimator.SetTrigger("Attack3");
                break;
        }
        weaponHitbox.StartDeflectionWindow();
        ResetWithdrawTimer();

        if (chainAttackCoroutine != null) StopCoroutine(chainAttackCoroutine);
        chainAttackCoroutine = StartCoroutine(HandleAttackChain());
    }

    private void StopAttackAnimation()
    {
        if (currentAttackIndex != 0)
            weaponAnimator.ResetTrigger("Attack" + currentAttackIndex);
        canAttack = true;
    }

    public void OnSuccessfulDeflection() => StartCoroutine(HitStop());

    IEnumerator HitStop()
    {
        float previousTimeScale = Time.timeScale;
        Time.timeScale = 0.1f;
        float normalSpeed = weaponAnimator.speed;
        weaponAnimator.speed = 0f;
        yield return new WaitForSeconds(.01f);
        weaponAnimator.speed = normalSpeed;
        Time.timeScale = previousTimeScale;
    }

    private void WithdrawWeapon()
    {
        weaponAnimator.SetTrigger("Withdraw");
        drewWeapon = false;
        canAttack = true;
        canChainAttack = false;
        currentAttackIndex = 0;
    }

    private void ResetWithdrawTimer()
    {
        if (withdrawCoroutine != null) StopCoroutine(withdrawCoroutine);
        withdrawCoroutine = StartCoroutine(WithdrawAfterDelay());
    }

    IEnumerator WithdrawAfterDelay()
    {
        yield return new WaitForSeconds(withdrawDelay);
        if (drewWeapon) WithdrawWeapon();
    }

    IEnumerator HandleAttackChain()
    {
        canChainAttack = true;
        canAttack = false;
        yield return new WaitForSeconds(chainAttackWindow);
        canChainAttack = false;
        canAttack = true;
    }

    public bool IsWeaponDrawn() => drewWeapon;
}